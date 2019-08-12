using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Utils
{
	/// <summary>
	/// Color utilities for converting and manipulating Color types
	/// </summary>
	public static class ColorUtils
	{
		/// <summary>
		/// Create a Color from a single int value representing the RGB, and optional alpha modifier
		/// </summary>
		public static Color IntToColor(int intColor, float alpha = 1.0f)
		{
			var bytes = BitConverter.GetBytes(intColor);
			int r = bytes[2];
			int g = bytes[1];
			int b = bytes[0];
			return new Color((float)r / 255, (float)g / 255, (float)b / 255) {a = alpha};
		}

		/// <summary>
		/// Create a Color from 4 ints in the range 0 - 255
		/// </summary>
		public static Color IntToColor(int r, int g, int b, int alpha = 255)
		{
			return new Color((float)r / 255, (float)g / 255, (float)b / 255, (float)alpha / 255);
		}

		/// <summary>
		/// Create a Color32 from 4 ints in the range 0 - 255
		/// </summary>
		public static Color32 IntToColor32(int r, int g, int b, int alpha = 255)
		{
			return new Color32((byte)r, (byte)g, (byte)b, (byte)alpha);
		}

		/// <summary>
		/// Convert an unsigned int to a Color32 value
		/// </summary>
		public static Color32 IntToColor32(uint intColor)
		{
			var bytes = BitConverter.GetBytes(intColor);
			return new Color32(bytes[3], bytes[2], bytes[1], bytes[0]);
		}

		/// <summary>
		/// Convert a Color to an int RGB value
		/// </summary>
		public static int ColorToInt(Color color)
		{
			var r = Mathf.RoundToInt(color.r * 255);
			var g = Mathf.RoundToInt(color.g * 255);
			var b = Mathf.RoundToInt(color.b * 255);

			var intColor = 0;

			intColor |= r << 16;
			intColor |= g << 8;
			intColor |= b;

			return intColor;
		}

		/// <summary>
		/// Adjust a Color's alpha value without changing the RGB
		/// </summary>
		public static Color FadeColor(Color baseColor, float alpha)
		{
			return new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
		}

		/// <summary>
		/// Marge an array of Color values equally
		/// </summary>
		public static Color MergeColors(Color[] colors)
		{
			float r = 0f, g = 0f, b = 0f;
			var totalAlpha = 0f;

			foreach (var color in colors)
			{
				r += color.r * color.a;
				g += color.g * color.a;
				b += color.b * color.a;
				totalAlpha += color.a;
			}

			return new Color(r / totalAlpha, g / totalAlpha, b / totalAlpha, totalAlpha / colors.Length);
		}

		/// <summary>
		/// Set the alpha value for a Graphic target's color without override the RGB value.
		/// </summary>
		public static void SetAlpha(this Graphic target, float alpha)
		{
			var color = target.color;
			color.a = alpha;
			target.color = color;
		}

		/// <summary>
		/// Set RGBA for a Graphic target's color without override the existing values.
		/// </summary>
		public static void SetColor(this Graphic target, float? r = null, float? g = null, float? b = null, float? a = null)
		{
			var color = target.color;
			if (r != null) color.r = r.Value;
			if (g != null) color.g = g.Value;
			if (b != null) color.b = b.Value;
			if (a != null) color.a = a.Value;
			target.color = color;
		}
	}
}