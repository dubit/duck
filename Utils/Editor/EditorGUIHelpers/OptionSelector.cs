using System;
using UnityEditor;

namespace DUCK.Utils.Editor.EditorGUIHelpers
{
	public static partial class EditorGUILayoutHelpers
	{
		public static string OptionSelectorField(string label, string value, string[] options)
		{
			var index = Array.IndexOf(options, value);
			var selectedIndex = EditorGUILayout.Popup(label, index, options);
			return selectedIndex >= 0 ? options[selectedIndex] : "";
		}
	}
}