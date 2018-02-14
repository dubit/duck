using UnityEditor;
using UnityEngine;

namespace DUCK.Editor.EditorSpotlight
{
	static class Styles
	{
		public const string PRO_SKIN_HIGHLIGHT_COLOR = "eeeeee";
		public const string PRO_SKIN_NORMAL_COLOR = "cccccc";

		public const string PERSONAL_SKIN_HIGHLIGHT_COLOR = "eeeeee";
		public const string PERSONAL_SKIN_NORMAL_COLOR = "222222";

		public static readonly GUIStyle inputFieldStyle;
		public static readonly GUIStyle placeholderStyle;
		public static readonly GUIStyle resultLabelStyle;
		public static readonly GUIStyle entryEven;
		public static readonly GUIStyle entryOdd;

		static Styles()
		{
			inputFieldStyle = new GUIStyle(EditorStyles.textField)
			{
				contentOffset = new Vector2(10, 10),
				fontSize = 32,
				focused = new GUIStyleState()
			};

			placeholderStyle = new GUIStyle(inputFieldStyle)
			{
				normal =
				{
					textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, .2f) : new Color(.2f, .2f, .2f, .4f)
				}
			};

			resultLabelStyle = new GUIStyle(EditorStyles.largeLabel)
			{
				alignment = TextAnchor.MiddleLeft,
				richText = true
			};

			entryOdd = new GUIStyle("CN EntryBackOdd");
			entryEven = new GUIStyle("CN EntryBackEven");
		}
	}
}