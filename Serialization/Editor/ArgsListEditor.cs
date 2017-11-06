using DUCK.Utils.Editor.EditorGUIHelpers;

namespace DUCK.Serialization.Editor
{
	/// <summary>
	/// Rather than extending editor and actually being an editor, 
	/// this is just a static function that allows you to draw it from another custom editor
	/// </summary>
	public static class ArgsListEditor
	{
		public static void Draw(ArgsList argsList)
		{
			for (var i = 0; i < argsList.ArgTypes.Count; i++)
			{
				var argType = argsList.ArgTypes[i];
				var argValue = argsList[i];
				var newArg = EditorGUILayoutHelpers.FieldByType(argValue, argType);
				argsList[i] = newArg;
			}
		}
	}
}