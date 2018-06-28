using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Utils
{
	public static class TransformExtensions
	{
		/// <summary>
		/// Destroy all children in this transform
		/// </summary>
		/// <param name="transform"></param>
		public static void DestroyAllChildren(this Transform transform)
		{
			var isRuntime = Application.isPlaying;
			foreach (Transform child in transform)
			{
				//Check to see if using method while not in runtime mode
				if (isRuntime)
				{
					Object.Destroy(child.gameObject);
				}
				else
				{
					Object.DestroyImmediate(child.gameObject);
				}
			}
		}

		/// <summary>
		/// Loop over each child and call function with child as parameter
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="action"></param>
		public static void ForEach(this Transform transform, Action<Transform> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			foreach (Transform child in transform)
			{
				action.Invoke(child);
			}
		}

		/// <summary>
		/// Returns true if the transform is in view of the given camera screen space
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static bool IsInViewOf(this Transform transform, Camera camera)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}

			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}

			var screenPoint = camera.WorldToViewportPoint(transform.position);
			return (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1);
		}

		/// <summary>
		/// Reset the transform position, rotation and local scale to default.
		/// </summary>
		public static void Reset(this Transform transform, bool local = false)
		{
			if (local)
			{
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
			else
			{
				transform.position = Vector3.zero;
				transform.rotation = Quaternion.identity;
			}

			transform.localScale = Vector3.one;
		}

		/// <summary>
		/// Set a pivot of a rect tranform
		/// </summary>
		/// <param name="transform">The Rect Tranform, will not work if it is not</param>
		/// <param name="pivot">The new pivot point</param>
		/// <param name="worldPositionStays">Is the Transform keeps the same position or not</param>
		public static void SetPivot(this Transform transform, Vector2 pivot, bool worldPositionStays = true)
		{
			var rectTransform = transform as RectTransform;
			if (rectTransform == null) return;

			if (worldPositionStays)
			{
				var delta = pivot - rectTransform.pivot;
				delta.Scale(rectTransform.rect.size);
				delta.Scale(rectTransform.localScale);
				rectTransform.anchoredPosition += delta;
			}

			rectTransform.pivot = pivot;
		}

		/// <summary>
		/// Creates a new child object of this transform, that has the given component and
		/// the name of that component.
		/// </summary>
		/// <param name="transform">The target transform</param>
		/// <param name="worldPositionStays">If true, the parent-relative position, scale and
		/// rotation are modified such that the object keeps the same world space position,
		/// rotation and scale as before.</param>
		/// <typeparam name="TComponent">The component to add to the child game object</typeparam>
		/// <returns>The newly created component</returns>
		public static TComponent AddChildWithComponent<TComponent>(this Transform transform, bool worldPositionStays = true)
			where TComponent : Component
		{
			var component = new GameObject(typeof(TComponent).Name).AddComponent<TComponent>();
			component.transform.SetParent(transform, worldPositionStays);
			return component;
		}

		/// <summary>
		/// Instantiates a prefab from resources and adds it as a child of this transform
		/// </summary>
		/// <param name="obj">The target transform</param>
		/// <param name="path">The path of the resource to load</param>
		/// <param name="worldPositionStays">If true, the parent-relative position, scale and
		/// rotation are modified such that the object keeps the same world space position,
		/// rotation and scale as before.</param>
		/// <typeparam name="TComponent">The type of component expected on the prefab and that will be returned</typeparam>
		/// <returns>The TComponent found on the prefab</returns>
		public static TComponent AddResourceChild<TComponent>(this Transform transform, string path,
			bool worldPositionStays = true)
			where TComponent : Component
		{
			return Instantiator.InstantiateResource<TComponent>(path, transform, worldPositionStays);
		}

		/// <summary>
		/// Take a snapshot of a transform in its current state in this frame.
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static TransformSnapshot Snapshot(this Transform transform)
		{
			return new TransformSnapshot(transform);
		}

		public static void ApplySnapshot(this Transform transform, TransformSnapshot snapshot)
		{
			snapshot.ApplyTo(transform);
		}
	}
}
