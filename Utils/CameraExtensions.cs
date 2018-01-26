using UnityEngine;

namespace DUCK.Utils
{
	public static class CameraExtensions
	{
		public static Texture2D TakeScreenshot(this Camera camera, int width, int height, 
			TextureFormat textureFormat = TextureFormat.ARGB32, int cullingMask = 0)
		{
			var renderTexture = new RenderTexture(width, height, 24);
			var screenShot = new Texture2D(width, height, textureFormat, false);

			var prevCullingMask = camera.cullingMask;
			if (cullingMask > 0)
			{
				camera.cullingMask = cullingMask;
			}

			camera.targetTexture = renderTexture;
			camera.Render();
			camera.targetTexture = null;
			camera.cullingMask = prevCullingMask;

			RenderTexture.active = renderTexture;
			screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			screenShot.Apply();
			RenderTexture.active = null;

			renderTexture.Release();

			return screenShot;
		}
	}
}
