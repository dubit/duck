using UnityEngine;

namespace DUCK.Utils
{
	public static class TextureExtensions
	{
		public static Sprite ToSprite(this Texture2D texture, Vector2 pivot)
		{
			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);
		}
		
		public static Sprite ToSprite(this Texture2D texture)
		{
			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
	}
}
