using System.Collections.Generic;
using DUCK.Utils.Editor.EditorGUIHelpers;
using UnityEditor;
using UnityEngine;

namespace DUCK.Serialization.Editor
{
	/// <summary>
	/// Rather than extending editor and actually being an editor,
	/// this is just a static function that allows you to draw it from another custom editor
	/// </summary>
	public static class ArgsListEditor
	{
		public delegate object CustomArgDrawFunc(string label, object currentValue);

		/// <summary>
		///	Draw args list
		/// </summary>
		/// <param name="argsList">The argsList to draw</param>
		/// <param name="parentObject">The parent object, is required to record undo</param>
		/// <param name="argNames">Optional overrides for the display names of args</param>
		/// <param name="customDrawFunctions">Custom draw functions to override draw functions for specific types</param>
		public static void Draw(ArgsList argsList,
			Object parentObject,
			IList<string> argNames = null,
			Dictionary<int, CustomArgDrawFunc> customDrawFunctions = null)
		{
			for (var i = 0; i < argsList.ArgTypes.Count; i++)
			{
				var argType = argsList.ArgTypes[i];
				var argValue = argsList[i];
				var label = argNames != null && argNames.Count > i ? argNames[i] : argType.Name;

				EditorGUI.BeginChangeCheck();

				if (customDrawFunctions != null && customDrawFunctions.ContainsKey(i))
				{
					argValue = customDrawFunctions[i](label, argValue);
				}
				else
				{
					argValue = EditorGUILayoutHelpers.FieldByType(label, argValue, argType);
				}

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(parentObject,
						"Set " + parentObject.name + " argsList[" + label + "] to " + argValue);
					argsList[i] = argValue;

					// Undo functionality works, and should also mark objects as dirty,
					// however for scriptable objects it won't mark them as dirty and you can't save changes
					if (parentObject is ScriptableObject)
					{
						EditorUtility.SetDirty(parentObject);
					}
				}
			}
		}
	}
}