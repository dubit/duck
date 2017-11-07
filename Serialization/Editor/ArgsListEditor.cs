using System.Collections.Generic;
using DUCK.Utils.Editor.EditorGUIHelpers;

namespace DUCK.Serialization.Editor
{
	/// <summary>
	/// Rather than extending editor and actually being an editor, 
	/// this is just a static function that allows you to draw it from another custom editor
	/// </summary>
	public static class ArgsListEditor
	{
		public static void Draw(ArgsList argsList, IList<string> argNames = null)
		{
			for (var i = 0; i < argsList.ArgTypes.Count; i++)
			{
				var argType = argsList.ArgTypes[i];
				var argValue = argsList[i];
				var label = argNames != null && argNames.Count > i ? argNames[i] : argType.Name;
				var newArg = EditorGUILayoutHelpers.FieldByType(label, argValue, argType);
				argsList[i] = newArg;
			}
		}
	}
}