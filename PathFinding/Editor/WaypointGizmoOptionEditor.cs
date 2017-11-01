using UnityEditor;
using UnityEngine;

namespace DUCK.PathFinding.Editor
{
	public class WaypointGizmoOptionEditor : EditorWindow
	{
		private float nodeRadius = WaypointGizmoStyle.nodeRadius;
		private float glowingSpeed = WaypointGizmoStyle.glowingSpeed;
		private float arrowSize = WaypointGizmoStyle.arrowSize;

		/// <summary>
		/// Show the window.
		/// </summary>
		[MenuItem("DUCK/Waypoint System/Gizmo Style", false)]
		public static void ShowOptionsWindow()
		{
			GetWindow<WaypointGizmoOptionEditor>(false, "Waypoint System Options", true);
		}

		private void OnEnable()
		{
			Reset();
		}

		private void Reset()
		{
			nodeRadius = EditorPrefs.GetFloat(WaypointGizmoStyle.NODE_RADIS_KEY, 0.25f);
			glowingSpeed = EditorPrefs.GetFloat(WaypointGizmoStyle.GLOWING_SPEED_KEY, 0.5f);
			arrowSize = EditorPrefs.GetFloat(WaypointGizmoStyle.ARROW_SIZE_KEY, 0.35f);
		}

		private void ApplySettings()
		{
			EditorPrefs.SetFloat(WaypointGizmoStyle.NODE_RADIS_KEY, nodeRadius);
			EditorPrefs.SetFloat(WaypointGizmoStyle.GLOWING_SPEED_KEY, glowingSpeed);
			EditorPrefs.SetFloat(WaypointGizmoStyle.ARROW_SIZE_KEY, arrowSize);

			WaypointGizmoStyle.UpdateSettings();
		}

		private static void DeleteSettings()
		{
			EditorPrefs.DeleteKey(WaypointGizmoStyle.NODE_RADIS_KEY);
			EditorPrefs.DeleteKey(WaypointGizmoStyle.GLOWING_SPEED_KEY);
			EditorPrefs.DeleteKey(WaypointGizmoStyle.ARROW_SIZE_KEY);
		}

		private void OnGUI()
		{
			nodeRadius = EditorGUILayout.FloatField("Node Radius", nodeRadius);
			glowingSpeed = EditorGUILayout.FloatField("Glowing Speed", glowingSpeed);
			arrowSize = EditorGUILayout.FloatField("Arrow Size", arrowSize);

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Default"))
			{
				DeleteSettings();
				Reset();
				ApplySettings();
			}
			else if (GUILayout.Button("Apply"))
			{
				ApplySettings();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}