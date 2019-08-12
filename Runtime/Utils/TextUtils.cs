using UnityEngine;

namespace DUCK.Utils
{
	public static class TextUtils
	{
		/// <summary>
		/// Calculate the size of some content if it is rendered with this style.
		/// </summary>
		/// <param name="text">The string to calculate</param>
		/// <param name="font">The font you are displaying the text with</param>
		/// <param name="fontSize">The size of the font</param>
		/// <returns>The calculated size</returns>
		public static Vector2 GetSize(string text, Font font, int fontSize)
		{
			var guiStyle = new GUIStyle
			{
				font = font,
				fontSize = fontSize
			};
			return guiStyle.CalcSize(new GUIContent(text));
		}

		/// <summary>
		/// How tall is this content given a specific width
		/// </summary>
		/// <param name="text">The string you want to calculate</param>
		/// <param name="font">The font you are displaying the text with</param>
		/// <param name="fontSize"></param>
		/// <param name="width">The width of the container the text is wrapped inside</param>
		/// <returns>The height of the content</returns>
		public static float GetHeight(string text, Font font, int fontSize, float width)
		{
			var guiStyle = new GUIStyle
			{
				font = font,
				fontSize = fontSize,
				wordWrap = true,
			};
			return guiStyle.CalcHeight(new GUIContent(text), width);
		}
	}
}