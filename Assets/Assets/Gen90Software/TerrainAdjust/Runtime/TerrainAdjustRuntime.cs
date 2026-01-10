using UnityEngine;

using Core = Gen90Software.Tools.TerrainAdjustCore;

namespace Gen90Software.Tools
{
	[AddComponentMenu("Gen90Software/TerrainAdjust", 100)]
	[DisallowMultipleComponent]
	[HelpURL("https://gen90software.com/terrainadjust-documentation.pdf")]
	public class TerrainAdjustRuntime : MonoBehaviour
	{
		#region Data Structures

		public bool IsAvailable => isAvailable;
		public Vector3 AdjustPos => adjustPos;
		public Vector3 AdjustNormal => adjustNormal;

		[Header("Settings")]
		public float radius = 3.0f;
		public float falloff = 0.6f;
		public float spacing = 0.5f;
		public float maxAngle = 60.0f;
		public float offsetY = -0.01f;
		public Core.BlendType blend = Core.BlendType.None;
		public Core.OverMaxAngleType overMaxAngle = Core.OverMaxAngleType.None;

		[Header("Target")]
		public Terrain tRender;
		public TerrainData tData;
		public TerrainCollider tCollider;

		[Header("Usage")]
		public bool paintActive = false;
		public LayerMask targetMask = ~0;
		public float maxDistance = 1000.0f;
		public bool excludeTerrains = true;

		private bool isAvailable;
		private Vector3 adjustPos;
		private Vector3 adjustNormal;
		private Vector3 lastAdjustPos;

		#endregion

		#region Mono Methods

		public void OnValidate()
		{
			if (tRender == null)
			{
				tRender = FindObjectOfType<Terrain>();
			}

			if (tRender == null)
				return;

			tData = tRender.terrainData;
			tCollider = tRender.GetComponent<TerrainCollider>();
		}

		public void Update()
		{
			if (!tRender || !tData || !tCollider)
				return;

			if (!paintActive)
				return;

			Camera cam = Camera.main;
			if (cam == null)
				return;

			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if (excludeTerrains)
			{
				if (!Core.RaycastExcludeTerrain(ray, out hit, maxDistance, targetMask, tCollider))
					return;
			}
			else
			{
				if (!Core.RaycastIncludeTerrain(ray, out hit, maxDistance, targetMask))
					return;
			}

			adjustPos = hit.point;
			isAvailable = Core.OverAngleCorrection(hit.normal, out adjustNormal, maxAngle, overMaxAngle);

			if (!isAvailable)
				return;

			if (Input.GetMouseButtonDown(0))
			{
				Core.AdjustHeight(adjustPos, adjustNormal, radius, falloff, offsetY, blend, ref tRender, ref tData);
				lastAdjustPos = adjustPos;
			}

			if (Input.GetMouseButton(0))
			{
				if (Vector3.Distance(adjustPos, lastAdjustPos) < spacing)
				{
					return;
				}

				Core.AdjustHeight(adjustPos, adjustNormal, radius, falloff, offsetY, blend, ref tRender, ref tData);
				lastAdjustPos = adjustPos;
			}

			if (Input.GetMouseButtonUp(0))
			{
				Core.ApplyHeightModifications(ref tData);
			}
		}

		#endregion
	}
}
