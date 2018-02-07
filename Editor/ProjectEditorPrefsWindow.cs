using System.Linq;
using DUCK.Prefs;
using DUCK.Utils.Editor.EditorGUIHelpers;
using UnityEditor;

namespace DUCK.Editor
{
	public class ProjectEditorPrefsWindow : EditorWindow
	{
		[MenuItem("DUCK/Project Editor Prefs")]
		private static void CreateEditorPrefsWindow()
		{
			GetWindow<ProjectEditorPrefsWindow>("ProjectPrefs");
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void OnGUI()
		{
			var types = ProjectEditorPrefs.StoredPrefs;
			foreach (var pair in types.ToList())
			{
				var key = pair.Key;
				var type = pair.Value;
				var value = ProjectEditorPrefs.Get(key);

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUILayoutHelpers.FieldByType(key, value, type);
				if (EditorGUI.EndChangeCheck())
				{
					ProjectEditorPrefs.Set(key, newValue, type);
				}
			}
		}
	}
}
