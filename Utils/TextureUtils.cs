using UnityEngine;

namespace DUCK.Utils
{
	public static class TextureUtils
	{
		public static Texture2D CombineTextures(Texture2D baseTexture, Texture2D addedTexture, int xOffset = 0, int yOffset = 0, bool additive = false)
		{
			var basePixels = baseTexture.GetPixels();
			var addedPixels = addedTexture.GetPixels();

			for (var p = 0; p < basePixels.Length; p++)
			{
				var tx = p % baseTexture.width;
				var ty = (p - tx) / baseTexture.width;

				tx -= xOffset;
				ty -= yOffset;

				if ((tx >= 0 && tx < addedTexture.width) && (ty >= 0 && ty < addedTexture.height))
				{
					var inputColor = addedPixels[ty * addedTexture.width + tx];
					inputColor = inputColor * inputColor.a;

					basePixels[(ty + yOffset) * baseTexture.width + (tx + xOffset)] = (!additive 
						? (basePixels[p] * (1f - inputColor.a) + inputColor)
						: basePixels[p] + inputColor);
				}
			}

			var texture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false);
			texture.SetPixels(basePixels);
			texture.Apply();

			return texture;
		}
	}
}

