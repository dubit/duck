using UnityEngine;

namespace DUCK.Utils
{
	public static class TextureUtils
	{
		/// <summary>
		/// Combine two textures by writing the contents of one on top of the other, at a specified offset
		/// </summary>
		/// <param name="baseTexture">The texture onto which to write a second texture.</param>
		/// <param name="addedTexture">The texture to be written onto baseTexture.</param>
		/// <param name="xOffset">X offset (from bottom-left) of the added texture sub-area.</param>
		/// <param name="yOffset">Y offset (from bottom-left, upwards) of the added texture sub-area.</param>
		/// <param name="additive">Whether to add the textures (true) or draw the addition over the base texture (false)</param>
		/// <returns>The newly created Texture2D.</returns>
		public static Texture2D CombineTextures(Texture2D baseTexture, Texture2D addedTexture, int xOffset = 0, int yOffset = 0, bool additive = false)
		{
			// Get the pixel (flattened) arrays of the base and overlay textures
			var basePixels = baseTexture.GetPixels();
			var addedPixels = addedTexture.GetPixels();

			for (var p = 0; p < basePixels.Length; p++)
			{
				// Convert linear index to [x,y] position on baseTexture
				var tx = p % baseTexture.width;
				var ty = (p - tx) / baseTexture.width;

				// Transform the [x,y] into the space of the overlay texture by subtracting the offset
				tx -= xOffset;
				ty -= yOffset;

				// If these coordinates fall within the overlay, add its pixels to the base
				if ((tx >= 0 && tx < addedTexture.width) && (ty >= 0 && ty < addedTexture.height))
				{
					var inputColor = addedPixels[ty * addedTexture.width + tx];
					inputColor = inputColor * inputColor.a;

					basePixels[(ty + yOffset) * baseTexture.width + (tx + xOffset)] = (!additive 
						? (basePixels[p] * (1f - inputColor.a) + inputColor)
						: basePixels[p] + inputColor);
				}
			}

			// Save the modified pixel array as a new texture
			var texture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false);
			texture.SetPixels(basePixels);
			texture.Apply();

			return texture;
		}
	}
}

