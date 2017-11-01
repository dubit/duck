using System.IO;
using UnityEditor;
using UnityEngine;

namespace DUCK.Editor.MenuItems
{
	/// <summary>
	/// Render to File is a very simple tool that can help you render things out to a PNG or JPEG from a camera
	/// </summary>
	public class RenderToFile : EditorWindow
	{
		private const int PREVIEW_POS_Y = 90;
		private const int PREVIEW_GAP = 10;
		private const int SCROLL_BAR_WIDTH = 15;

		private Camera renderCamera;
		private int jpegQuality = 75;
		private Vector2 previewScroll;
		private float previewZoom = 1.0f;

		private int renderTimes;
		private string lastPath;

		/// <summary>
		/// Showing the window.
		/// </summary>
		[MenuItem("DUCK/Render to File/Show Window", false)]
		public static void ShowWindow()
		{
			GetWindow<RenderToFile>(false, "Render to File", true);
		}

		private void OnGUI()
		{
			// Pick the Camera
			renderCamera = EditorGUILayout.ObjectField("Render Camera", renderCamera, typeof(Camera), true) as Camera;

			if (renderCamera == null)
			{
				EditorGUILayout.HelpBox("You need to select a Camera first (in the scene).", MessageType.Info);
				return;
			}

			// Get the render texture
			var renderTexture = renderCamera.targetTexture;
			if (renderTexture == null)
			{
				EditorGUILayout.HelpBox("The Render Camera must have a RenderTexture.", MessageType.Error);
				return;
			}

			jpegQuality = EditorGUILayout.IntSlider("JPEG Quality", jpegQuality, 1, 100);

			GUILayout.BeginHorizontal();
			var renderToPNG = GUILayout.Button("Render to PNG");
			var renderToJPEG = GUILayout.Button("Render to JPEG");
			GUILayout.EndHorizontal();

			if (renderToPNG || renderToJPEG)
			{
				var currentObj = Selection.activeObject;
				// Audo generate a filename
				var filename = currentObj == null ? renderTimes.ToString("000") : currentObj.name;
				// Convert to all lower cases and replace the spaces with underscore
				filename = filename.ToLower().Replace(' ', '_');

				// Call the panel and pick a path
				var path = EditorUtility.SaveFilePanel("Save PNG to...",
					string.IsNullOrEmpty(lastPath) ? Application.dataPath : lastPath, filename, renderToPNG ? "png" : "jpeg");

				if (!string.IsNullOrEmpty(path))
				{
					// Remember the last path -- fancy feature eh?
					lastPath = path;

					// Activate the current render texture
					RenderTexture.active = renderTexture;

					// Clear the texture and render it
					GL.Clear(true, true, Color.clear);
					renderCamera.Render();

					// Create a texture2D and put all the pixels in it
					var texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
					texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
					texture2D.Apply();

					// Output to an image file
					var imageData = renderToPNG ? texture2D.EncodeToPNG() : texture2D.EncodeToJPG(jpegQuality);
					File.WriteAllBytes(path, imageData);
					DestroyImmediate(texture2D);

					// Nice little counter for auto name generation
					++renderTimes;
				}
			}

			GUILayout.Space(10);
			previewZoom = EditorGUILayout.Slider("Preview Zoom", previewZoom, 0.1f, 2.0f);

			var previewWidth = position.width - PREVIEW_GAP * 2 - SCROLL_BAR_WIDTH;
			var previewHeight = previewWidth * renderTexture.width / renderTexture.height;
			var previewRect = new Rect(PREVIEW_GAP, PREVIEW_POS_Y, previewWidth * previewZoom, previewHeight * previewZoom);
			var scrollRect = new Rect(PREVIEW_GAP, PREVIEW_POS_Y, previewWidth + SCROLL_BAR_WIDTH, position.height - PREVIEW_POS_Y);

			// Draw the preview and the scrolls
			previewScroll = GUI.BeginScrollView(scrollRect, previewScroll, previewRect, false, false);
			EditorGUI.DrawPreviewTexture(previewRect, renderTexture);
			GUI.EndScrollView();
		}
	}
}