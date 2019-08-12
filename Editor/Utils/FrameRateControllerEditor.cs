using UnityEditor;
using UnityEngine;

namespace DUCK.Utils.Editor
{
	[CustomEditor(typeof(FrameRateController))]
	public class FrameRateControllerEditor : UnityEditor.Editor
	{
		private FrameRateController frameRateController;
		private bool isEditorVSyncDisabled;

		private void OnEnable()
		{
			frameRateController = (FrameRateController)target;
			isEditorVSyncDisabled = QualitySettings.vSyncCount == 0;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (!Application.isPlaying) return;

			EditorGUILayout.Space();

			// Help the user disable the Editor VSync (temporary, run time only)
			if (!isEditorVSyncDisabled)
			{
				EditorGUILayout.HelpBox(
					"Editor VSync is enabled -- "+
					"you are not be able to set the target frame rate unless you disable the VSync.", MessageType.Warning);
			}
			var disableEditorVSync = EditorGUILayout.Toggle("Disable Editor VSync", isEditorVSyncDisabled);
			if (disableEditorVSync != isEditorVSyncDisabled)
			{
				isEditorVSyncDisabled = disableEditorVSync;
				QualitySettings.vSyncCount = disableEditorVSync ? 0 : 1;
			}

			// Draw the progress bar -- monitoring the performance
			var performance = frameRateController.CurrentFPS / frameRateController.TargetFPS;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(string.Format("Performance: {0:0.#}%", performance * 100.0f));
			DrawProgressBar(string.Format("FPS: {0:0.#}", frameRateController.CurrentFPS), performance);
			EditorGUILayout.EndHorizontal();
			Repaint();

			// Apply all settings (just in case if the user changed any)
			frameRateController.RefreshSettings();
		}

		private static void DrawProgressBar(string label, float value)
		{
			var rect = GUILayoutUtility.GetRect(18, 18);
			EditorGUI.ProgressBar(rect, value, label);
		}
	}
}