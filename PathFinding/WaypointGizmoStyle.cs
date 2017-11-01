#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DUCK.PathFinding
{
	public static class WaypointGizmoStyle
	{
		public const string NODE_RADIS_KEY = "WPS_nodeRadius";
		public const string GLOWING_SPEED_KEY = "WPS_glowingSpeed";
		public const string ARROW_SIZE_KEY = "WPS_arrowSize";

		public static float nodeRadius = 0.25f;
		public static float glowingSpeed = 0.5f;
		public static float arrowSize = 0.35f;

		public static float sphereRadiusCurrentNode = nodeRadius * 1.15f;
		public static float wireSphereRadius = nodeRadius * 1.2f;
		public static float dynamicNodeRadius = nodeRadius * 0.5f;
		public static float radiusGrowFrom = nodeRadius * 0.8f;
		public static float radiusGrowTo = nodeRadius * 1.2f;
		public static float doubleLineWidth = nodeRadius * 0.8f;

		public static Vector3 cubeSize = new Vector3(nodeRadius, nodeRadius, nodeRadius) * 1.8f;
		public static Vector3 cubeSizeFrom = cubeSize * 0.85f;
		public static Vector3 cubeSizeTo = cubeSize * 1.3f;
		public static Vector3 cubeSizeCurrentNode = cubeSize * 1.15f;
		public static Vector3 wireCubeSize = cubeSize * 1.2f;

		// This can actually make the gizmos brighter than normal since it takes no lights on it
		public static readonly Color SphereColor = new Color(1.3f, 3.5f, 1.3f, 0.8f);
		public static readonly Color SphereIgnoredColor = new Color(1.0f, 3.0f, 1.0f, 0.1f);
		public static readonly Color CubeColor = new Color(3.5f, 0.6f, 0.5f, 0.8f);
		public static readonly Color CubeIgnoredColor = new Color(3.0f, 1.0f, 1.0f, 0.1f);
		public static readonly Color DynamicNodeColor = new Color(0.6f, 1.2f, 1.9f, 0.8f);
		public static readonly Color ColorCurrentNode = new Color(2.8f, 1.1f, 0.2f, 0.9f);
		public static readonly Color ColorFaulty = new Color(3.0f, 0.8f, 2.5f, 0.95f);
		public static readonly Color ColorGlowFrom = new Color(0.7f, 0.4f, 1.5f, 0.4f);
		public static readonly Color ColorGlowTo = new Color(3.8f, 5.0f, 3.8f, 0.9f);
		public static readonly Color LineColor = new Color(0.1f, 1.0f, 0.6f, 1.0f);
		public static readonly Color LineBlockedColor = new Color(1.0f, 0.3f, 0.2f, 1.0f);
		public static readonly Color PathColor = new Color(2.5f, 1.5f, 0.2f, 1.0f);
		public static readonly Color ArrowColor = new Color(1.0f, 0.8f, 0.4f, 1.0f);

		[InitializeOnLoadMethod]
		public static void UpdateSettings()
		{
			nodeRadius = EditorPrefs.GetFloat(NODE_RADIS_KEY, 0.25f);
			glowingSpeed = EditorPrefs.GetFloat(GLOWING_SPEED_KEY, 0.5f);
			arrowSize = EditorPrefs.GetFloat(ARROW_SIZE_KEY, 0.35f);

			sphereRadiusCurrentNode = nodeRadius * 1.15f;
			wireSphereRadius = nodeRadius * 1.2f;
			dynamicNodeRadius = nodeRadius * 0.5f;
			radiusGrowFrom = nodeRadius * 0.8f;
			radiusGrowTo = nodeRadius * 1.2f;
			doubleLineWidth = nodeRadius * 0.8f;

			cubeSize = new Vector3(nodeRadius, nodeRadius, nodeRadius) * 1.8f;
			cubeSizeFrom = cubeSize * 0.85f;
			cubeSizeTo = cubeSize * 1.3f;
			cubeSizeCurrentNode = cubeSize * 1.15f;
			wireCubeSize = cubeSize * 1.2f;
		}
	}
}
#endif
