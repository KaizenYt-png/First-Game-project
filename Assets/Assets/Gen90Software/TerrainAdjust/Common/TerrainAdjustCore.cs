using UnityEngine;

namespace Gen90Software.Tools
{
	public static class TerrainAdjustCore
	{
		#region Data Structures

		public enum BlendType
		{
			None,
			Min,
			Max
		}

		public enum OverMaxAngleType
		{
			None,
			Clamp,
			Flat
		}

		private static readonly RaycastHit[] rayHits = new RaycastHit[64];
		private static AnimationCurve brushCurve;
		private static float cachedFalloff = -1f;

		#endregion

		#region Terrain Modification

		public static void AdjustHeight(Vector3 point, Vector3 normal, float radius, float falloff, float offsetY, BlendType blend, ref Terrain tRender, ref TerrainData tData)
		{
			Vector3 localPosMin = tRender.transform.InverseTransformPoint(point + new Vector3(-radius, 0.0f, -radius));
			Vector3 localPosMax = tRender.transform.InverseTransformPoint(point + new Vector3(radius, 0.0f, radius));

			Vector3 ratePosMin = new Vector3(
				Mathf.Clamp01(localPosMin.x / tData.size.x),
				Mathf.Clamp01(localPosMin.y / tData.size.y),
				Mathf.Clamp01(localPosMin.z / tData.size.z)
			);
			Vector3 ratePosMax = new Vector3(
				Mathf.Clamp01(localPosMax.x / tData.size.x),
				Mathf.Clamp01(localPosMax.y / tData.size.y),
				Mathf.Clamp01(localPosMax.z / tData.size.z)
			);

			int tBorder = tData.heightmapResolution - 1;

			Vector2Int coordMin = new Vector2Int(
				ClampTerrainCoord(Mathf.FloorToInt(ratePosMin.x * tBorder), tBorder),
				ClampTerrainCoord(Mathf.FloorToInt(ratePosMin.z * tBorder), tBorder)
			);
			Vector2Int coordMax = new Vector2Int(
				ClampTerrainCoord(Mathf.FloorToInt(ratePosMax.x * tBorder), tBorder),
				ClampTerrainCoord(Mathf.FloorToInt(ratePosMax.z * tBorder), tBorder)
			);

			int sizeX = Mathf.Max(1, coordMax.x - coordMin.x + 1);
			int sizeY = Mathf.Max(1, coordMax.y - coordMin.y + 1);
			float[,] heights = tData.GetHeights(coordMin.x, coordMin.y, sizeX, sizeY);
			PrepareBrushCurve(falloff);

			for (int x = 0; x < sizeX; x++)
			{
				for (int y = 0; y < sizeY; y++)
				{
					heights[y, x] = EvaluateBrush(
						(coordMin.x + x) / (float)tBorder,
						(coordMin.y + y) / (float)tBorder,
						heights[y, x],
						point,
						normal,
						radius,
						offsetY,
						blend,
						ref tRender,
						ref tData
					);
				}
			}

			tData.SetHeightsDelayLOD(coordMin.x, coordMin.y, heights);
		}

		public static void ApplyHeightModifications(ref TerrainData tData)
		{
			tData.SyncHeightmap();
		}

		public static bool RaycastIncludeTerrain(Ray ray, out RaycastHit hit, float maxDistance, LayerMask targetMask)
		{
			return Physics.Raycast(ray, out hit, maxDistance, targetMask);
		}

		public static bool RaycastExcludeTerrain(Ray ray, out RaycastHit hit, float maxDistance, LayerMask targetMask, TerrainCollider tCollider)
		{
			int count = Physics.RaycastNonAlloc(ray, rayHits, maxDistance, targetMask);
			if (count == 0)
			{
				hit = default;
				return false;
			}

			int nearestIndex = -1;
			float nearestDistance = maxDistance;
			for (int i = 0; i < count; i++)
			{
				RaycastHit oneHit = rayHits[i];
				if (!oneHit.collider || oneHit.collider == tCollider)
					continue;

				if (oneHit.distance < nearestDistance)
				{
					nearestIndex = i;
					nearestDistance = oneHit.distance;
				}
			}

			if (nearestIndex == -1)
			{
				hit = default;
				return false;
			}

			hit = rayHits[nearestIndex];
			return true;
		}

		public static bool OverAngleCorrection(Vector3 currentNormal, out Vector3 correctedNormal, float maxAngle, OverMaxAngleType overMaxAngle)
		{
			float currentAngle = Vector3.Angle(currentNormal, Vector3.up);

			if (overMaxAngle == OverMaxAngleType.None)
			{
				correctedNormal = currentNormal;
				return currentAngle <= maxAngle;
			}

			if (currentAngle <= maxAngle)
			{
				correctedNormal = currentNormal;
				return true;
			}

			if (overMaxAngle == OverMaxAngleType.Flat)
			{
				correctedNormal = Vector3.up;
				return true;
			}

			Vector3 axis = Vector3.Cross(Vector3.up, currentNormal);
			if (axis.sqrMagnitude < Mathf.Epsilon)
			{
				correctedNormal = Vector3.up;
				return true;
			}
			correctedNormal = Quaternion.AngleAxis(maxAngle, axis.normalized) * Vector3.up;
			return true;
		}

		private static float EvaluateBrush(float rateX, float rateY, float lastHeight, Vector3 point, Vector3 normal, float radius, float offsetY, BlendType blend, ref Terrain tRender, ref TerrainData tData)
		{
			Vector3 realPos = tRender.transform.TransformPoint(new Vector3(rateX * tData.size.x, lastHeight * tData.size.y, rateY * tData.size.z));
			Vector3 vOriginal = new Vector3(realPos.x, 0.0f, realPos.z) - new Vector3(point.x, 0.0f, point.z);

			if (normal.y < Mathf.Epsilon)
				return lastHeight;

			float heightX = Mathf.Tan(Mathf.Atan2(-normal.x, normal.y)) * vOriginal.x;
			float heightZ = Mathf.Tan(Mathf.Atan2(normal.z, normal.y)) * vOriginal.z;
			Vector3 vProjected = new Vector3(vOriginal.x, heightX - heightZ, vOriginal.z);

			float targetHeight = (point.y + vProjected.y + offsetY - tRender.transform.position.y) / tData.size.y;

			targetHeight = Mathf.Lerp(lastHeight, targetHeight, brushCurve.Evaluate(Mathf.Clamp01(vProjected.magnitude / radius)));

			if (blend == BlendType.Min)
				return Mathf.Min(lastHeight, targetHeight);

			if (blend == BlendType.Max)
				return Mathf.Max(lastHeight, targetHeight);

			return targetHeight;
		}

		#endregion

		#region Utilities

		private static void PrepareBrushCurve(float falloff)
		{
			if (brushCurve != null && Mathf.Approximately(cachedFalloff, falloff))
				return;

			brushCurve = new AnimationCurve(
				new Keyframe(0f, 1f),
				new Keyframe(falloff, 1f),
				new Keyframe(1f, 0f)
			);
			cachedFalloff = falloff;
		}

		private static int ClampTerrainCoord(int value, int tBorder)
		{
			return Mathf.Clamp(value, 0, tBorder);
		}

		#endregion
	}
}
