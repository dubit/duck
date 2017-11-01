using UnityEditor;
using UnityEngine;

namespace DUCK.Editor.MenuItems
{
	public static class AnchorsUtils
	{
		/// <summary>
		/// Contains a function that sets the anchors to be positioned at the corners of all selected rect transforms.
		/// </summary>
		[MenuItem("DUCK/UI/Set Anchors to Corners _[")]
		private static void SetAnchorsToCorners()
		{
			var transforms = Selection.transforms;
			foreach (var transform in transforms)
			{
				// verify we are a rect transform and our parent is also
				var rectTransform = transform.transform as RectTransform;
				if (rectTransform == null) continue;
				var parent = rectTransform.parent as RectTransform;
				if (parent == null) continue;

				// get the dimensions
				var parentRect = parent.rect;
				var parentWidth = parentRect.width;
				var parentHeight = parentRect.height;

				// get the offset & anchor min/miax
				var offsetMin = rectTransform.offsetMin;
				var offsetMax = rectTransform.offsetMax;
				var anchorMin = rectTransform.anchorMin;
				var anchorMax = rectTransform.anchorMax;

				// recalculate the anchor min & max from the offset min & max
				anchorMin = new Vector2(anchorMin.x + offsetMin.x / parentWidth, anchorMin.y + offsetMin.y / parentHeight);
				anchorMax = new Vector2(anchorMax.x + offsetMax.x / parentWidth, anchorMax.y + offsetMax.y / parentHeight);

				// set them
				rectTransform.anchorMin = anchorMin;
				rectTransform.anchorMax = anchorMax;

				// zero out the offset min & max
				rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
			}
		}

		/// <summary>
		/// Set the corners to be positioned at the anchors of all selected rect transforms.
		/// </summary>
		[MenuItem("DUCK/UI/Set Corners to Anchors _]")]
		private static void SetCornersToAnchors()
		{
			foreach (var transform in Selection.transforms)
			{
				var rectTransform = transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
				}
			}
		}
	}
}
