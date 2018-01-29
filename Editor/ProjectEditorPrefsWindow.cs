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
			foreach (var key in types.Keys.ToList())
			{
				var type = types[key];
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
