using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Core = Gen90Software.Tools.TerrainAdjustCore;

namespace Gen90Software.Tools
{
	public class TerrainAdjust : EditorWindow
	{
		#region Data Structures

		private const float radiusMin = 0.1f;
		private const float radiusMax = 50.0f;
		private const float falloffMin = 0.01f;
		private const float falloffMax = 0.99f;
		private const float spacingMin = 0.01f;
		private const float spacingMax = 2.0f;
		private const float maxAngleMin = 10.0f;
		private const float maxAngleMax = 80.0f;
		private const float offsetYMin = -1.0f;
		private const float offsetYMax = 1.0f;
		private const string version = "1.2.0";

		[SerializeField] private float radius = 3.0f;
		[SerializeField] private float falloff = 0.6f;
		[SerializeField] private float spacing = 0.5f;
		[SerializeField] private float maxAngle = 60.0f;
		[SerializeField] private float offsetY = -0.01f;
		[SerializeField] private Core.BlendType blend = Core.BlendType.None;
		[SerializeField] private Core.OverMaxAngleType overMaxAngle = Core.OverMaxAngleType.None;
		[SerializeField] private LayerMask targetMask = ~0;
		[SerializeField] private float maxDistance = 1000.0f;
		[SerializeField] private bool excludeTerrains = true;

		[SerializeField] private Terrain tRender;
		[SerializeField] private TerrainData tData;
		[SerializeField] private TerrainCollider tCollider;

		private bool paintActive;
		private Vector3 adjustPos;
		private Vector3 adjustNormal;
		private Vector3 lastAdjustPos;

		private Vector2 viewScroll;
		private bool foldoutBrush;
		private bool foldoutTarget;
		private bool foldoutSettings;
		private Vector2 shortcutMousePos;

		#endregion

		#region Cache & Optimalization

		private GUIStyle bigButton;
		private GUIStyle BigButton => bigButton ??= new GUIStyle("Button")
		{
			font = EditorStyles.boldFont,
			fontSize = 14
		};

		#endregion

		#region Initialization

		[MenuItem("Tools/Gen90Software/Terrain Adjust")]
		public static void InitTerrainAdjustWindow()
		{
			TerrainAdjust ta = GetWindow<TerrainAdjust>();
			ta.titleContent.text = "Terrain Adjust";
			ta.Show();

			ta.viewScroll = Vector2.zero;
			ta.foldoutBrush = true;
			ta.foldoutTarget = true;
			ta.foldoutSettings = false;
		}

		public void OnEnable()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			Undo.undoRedoPerformed += OnUndoRedo;
			InitTerrainConnect();
		}

		public void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		public void OnUndoRedo()
		{
			if (tData)
			{
				tData.SyncHeightmap();
			}

			Repaint();
		}

		#endregion

		#region Main

		public void OnGUI()
		{
			if (!tRender || !tData || !tCollider)
			{
				EditorGUILayout.Space(10);
				tRender = (Terrain)EditorGUILayout.ObjectField("Terrain", tRender, typeof(Terrain), true);

				if (tRender == null)
				{
					EditorGUILayout.Space(10);
					EditorGUILayout.HelpBox("No Terrain connected!", MessageType.Warning);
					return;
				}

				InitTerrainConnect();

				if (tData == null)
				{
					EditorGUILayout.Space(10);
					EditorGUILayout.HelpBox("Terrain Data not available on the target Terrain! Try to fix your Terrain in Debug mode, or select another one!", MessageType.Error);
					return;
				}

				if (tCollider == null)
				{
					EditorGUILayout.Space(10);
					EditorGUILayout.HelpBox("No Terrain Collider component attached to the target Terrain!", MessageType.Warning);
					if (GUILayout.Button("Add Terrain Collider"))
					{
						FixTerrainConnect();
					}
					return;
				}
			}

			//PAINT AREA
			EditorGUILayout.Space(10);
			paintActive = GUILayout.Toggle(paintActive, "Paint", BigButton, GUILayout.MinHeight(30));

			//SCROLL VIEW START
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			viewScroll = EditorGUILayout.BeginScrollView(viewScroll);

			//BRUSH AREA
			foldoutBrush = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutBrush, "Brush");
			if (foldoutBrush)
			{
				EditorGUI.BeginChangeCheck();
				float _radius = EditorGUILayout.Slider(new GUIContent("        Radius", "Set the brush radius in meters."), radius, radiusMin, radiusMax);
				float _falloff = EditorGUILayout.Slider(new GUIContent("        Falloff", "Set the fall of the brush relative to the Radius."), falloff, falloffMin, falloffMax);
				float _spacing = EditorGUILayout.Slider(new GUIContent("        Spacing", "Set the brush stroke frequency in meters."), spacing, spacingMin, spacingMax);
				float _maxAngle = EditorGUILayout.Slider(new GUIContent("        Max Angle", "Limits the maximum working angle of the brush."), maxAngle, maxAngleMin, maxAngleMax);
				float _offsetY = EditorGUILayout.Slider(new GUIContent("        Vertical Offset", "Offset the brush adjust height in meters."), offsetY, offsetYMin, offsetYMax);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this, "Terrain Adjust Inspector");
					radius = _radius;
					falloff = _falloff;
					spacing = _spacing;
					maxAngle = _maxAngle;
					offsetY = _offsetY;
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//BLEND AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.BeginChangeCheck();
			GUILayout.Label(new GUIContent("Blend", "Limit the blending of height adjust.\n\nNone:\tOverride height.\nMin:\tSet height if original higher.\nMax:\tSet height if original lower."), EditorStyles.boldLabel);
			Core.BlendType _blend = (Core.BlendType)GUILayout.Toolbar((int)blend, System.Enum.GetNames(typeof(Core.BlendType)));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, "Terrain Adjust Inspector");
				blend = _blend;
			}

			//ANGLE AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.BeginChangeCheck();
			GUILayout.Label(new GUIContent("Over Max Angle", "Set behavior of angle limitation.\n\nNone:\tForbid the brush usage.\nClamp:\tChange angle to maximum allowed.\nFlat:\tChange angle to zero."), EditorStyles.boldLabel);
			Core.OverMaxAngleType _overMaxAngle = (Core.OverMaxAngleType)GUILayout.Toolbar((int)overMaxAngle, System.Enum.GetNames(typeof(Core.OverMaxAngleType)));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, "Terrain Adjust Inspector");
				overMaxAngle = _overMaxAngle;
			}

			//TARGET AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			foldoutTarget = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutTarget, "Target");
			if (foldoutTarget)
			{
				EditorGUI.BeginChangeCheck();
				LayerMask _targetMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUILayout.MaskField(new GUIContent("        Target Mask", "Masking the target collider selection."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(targetMask), InternalEditorUtility.layers));
				float _maxDistance = EditorGUILayout.FloatField(new GUIContent("        Max Distance", "Limit the distance of the target collider selection in meters."), maxDistance);
				bool _excludeTerrains = EditorGUILayout.Toggle(new GUIContent("        Exclude Terrains", "Exclude Terrain Collider type from selection."), excludeTerrains);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this, "Terrain Adjust Inspector");
					targetMask = _targetMask;
					maxDistance = Mathf.Max(_maxDistance, 1.0f);
					excludeTerrains = _excludeTerrains;
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//SETTINGS AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			foldoutSettings = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutSettings, "Settings");
			if (foldoutSettings)
			{
				EditorGUI.BeginChangeCheck();
				Terrain _tRender = (Terrain)EditorGUILayout.ObjectField("        Terrain", tRender, typeof(Terrain), true);
				TerrainData _tData = (TerrainData)EditorGUILayout.ObjectField("        Data", tData, typeof(TerrainData), false);
				TerrainCollider _tCollider = (TerrainCollider)EditorGUILayout.ObjectField("        Collider", tCollider, typeof(TerrainCollider), true);
				EditorGUILayout.Space(20);
				EditorGUILayout.LabelField("        Version: " + version);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this, "Terrain Adjust Inspector");
					tRender = _tRender;
					tData = _tData;
					tCollider = _tCollider;
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//SCROLL VIEW END
			EditorGUILayout.Space(20);
			EditorGUILayout.EndScrollView();

			//HELP AREA
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			float cellSize = Mathf.Max(tData.size.x, tData.size.z) / tData.heightmapResolution - 1;
			if (radius < 1.5f * cellSize)
			{
				EditorGUILayout.HelpBox("Terrain heightmap resolution too small\nfor this brush radius! Maybe the brush has no effect.", MessageType.Warning);
			}
			EditorGUILayout.HelpBox("Radius:\tCtrl + Right Mouse + Horizontal move\nFalloff:\tCtrl + Right Mouse + Vertical move", MessageType.None);
			EditorGUILayout.Space(10);
		}

		public void OnSceneGUI(SceneView sceneView)
		{
			if (!paintActive)
				return;

			Camera cam = sceneView != null ? sceneView.camera : null;
			if (cam == null)
				return;

			if (Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
			}

			Rect viewRect = new Rect(0, 0, cam.pixelWidth, cam.pixelHeight);
			if (!viewRect.Contains(HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition)) || Event.current.modifiers == EventModifiers.Alt)
				return;

			if (Event.current.modifiers == EventModifiers.Control)
			{
				if (Event.current.type == EventType.MouseDown)
				{
					shortcutMousePos = Event.current.mousePosition;
					Undo.RegisterCompleteObjectUndo(this, "Terrain Adjust Inspector Shortcut");
				}

				if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag)
				{
					Vector2 delta = ConstrainShortcutVector(shortcutMousePos, Event.current.mousePosition, Event.current.delta);
					radius = Mathf.Clamp(radius + delta.x * 0.02f, radiusMin, radiusMax);
					falloff = Mathf.Clamp(falloff - delta.y * 0.01f, falloffMin, falloffMax);
					Repaint();
					Event.current.Use();
				}

				DrawGizmo(true, adjustPos, adjustNormal);
				sceneView.Repaint();
				return;
			}

			RaycastHit hit;
			if (excludeTerrains)
			{
				if (!Core.RaycastExcludeTerrain(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, maxDistance, targetMask, tCollider))
					return;
			}
			else
			{
				if (!Core.RaycastIncludeTerrain(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, maxDistance, targetMask))
					return;
			}

			adjustPos = hit.point;
			bool isAvailable = Core.OverAngleCorrection(hit.normal, out adjustNormal, maxAngle, overMaxAngle);

			DrawGizmo(isAvailable, adjustPos, adjustNormal);
			sceneView.Repaint();

			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			{
				Undo.RegisterCompleteObjectUndo(tData, "Terrain Adjust");

				if (!isAvailable)
				{
					Event.current.Use();
					return;
				}

				Core.AdjustHeight(adjustPos, adjustNormal, radius, falloff, offsetY, blend, ref tRender, ref tData);
				lastAdjustPos = adjustPos;
				Event.current.Use();
			}

			if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag)
			{
				if (!isAvailable || Vector3.Distance(adjustPos, lastAdjustPos) < spacing)
				{
					Event.current.Use();
					return;
				}

				Core.AdjustHeight(adjustPos, adjustNormal, radius, falloff, offsetY, blend, ref tRender, ref tData);
				lastAdjustPos = adjustPos;
				Event.current.Use();
			}

			if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
			{
				if (!isAvailable)
				{
					Event.current.Use();
					return;
				}

				Core.ApplyHeightModifications(ref tData);
				Event.current.Use();
			}
		}

		#endregion

		#region Gizmos

		private void DrawGizmo(bool enabled, Vector3 pos, Vector3 up)
		{
			Handles.color = enabled ? new Color(0.0f, 1.0f, 1.0f, 0.1f) : new Color(1.0f, 0.0f, 0.0f, 0.1f);
			Handles.DrawSolidDisc(pos, up, radius * falloff);
			Handles.color = enabled ? Color.cyan : Color.red;
			Handles.DrawWireDisc(pos, up, radius);
		}

		#endregion

		#region Utilities

		private void InitTerrainConnect()
		{
			//Terrain
			if (tRender == null)
			{
				tRender = FindObjectOfType<Terrain>();
				if (tRender == null)
					return;
			}

			//Terrain data
			tData = tRender.terrainData;
			if (tData == null)
				return;

			//Terrain collider
			tCollider = tRender.GetComponent<TerrainCollider>();
			if (tCollider == null)
				return;

			tCollider.terrainData = tData;
			tCollider.enabled = true;
		}

		private void FixTerrainConnect()
		{
			if (!tRender || !tData)
				return;

			tCollider = tRender.gameObject.AddComponent<TerrainCollider>();
			tCollider.terrainData = tData;
			tCollider.enabled = true;
		}

		private Vector2 ConstrainShortcutVector(Vector2 start, Vector2 current, Vector2 delta)
		{
			Vector2 move = current - start;
			if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
			{
				return new Vector2(delta.x, 0.0f);
			}
			else
			{
				return new Vector2(0.0f, delta.y);
			}
		}

		#endregion
	}
}
