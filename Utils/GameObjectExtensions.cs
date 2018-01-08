using UnityEngine;

namespace DUCK.Utils
{
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Returns true if game object is in view of given camera
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static bool IsInViewOf(this GameObject gameObject, Camera camera)
		{
			return gameObject.transform.IsInViewOf(camera);
		}

		/// <summary>
		/// Creates a new child object of this game object, that has the given component and the name of that component
		/// </summary>
		/// <param name="obj">The target game object</param>
		/// <param name="worldPositionStays">If true, the parent-relative position, scale and
		/// rotation are modified such that the object keeps the same world space position,
		/// rotation and scale as before.</param>
		/// <typeparam name="TComponent">The component to add to the child game object</typeparam>
		/// <returns>The newly created component</returns>
		public static TComponent AddChildWithComponent<TComponent>(this GameObject obj, bool worldPositionStays = true)
			where TComponent : Component
		{
			var component = new GameObject(typeof(TComponent).Name).AddComponent<TComponent>();
			component.transform.SetParent(obj.transform, worldPositionStays);
			return component;
		}

		/// <summary>
		/// Instantiates a prefab from resources and adds it as a child of this game object
		/// </summary>
		/// <param name="obj">The target game object</param>
		/// <param name="path">The path of the resource to load</param>
		/// <param name="worldPositionStays">If true, the parent-relative position, scale and
		/// rotation are modified such that the object keeps the same world space position,
		/// rotation and scale as before.</param>
		/// <typeparam name="TComponent">The type of component expected on the prefab and that will be returned</typeparam>
		/// <returns>The TComponent found on the prefab</returns>
		public static TComponent AddResourceChild<TComponent>(this GameObject obj, string path,
			bool worldPositionStays = true)
			where TComponent : Component
		{
			return Instantiator.InstantiateResource<TComponent>(path, obj.transform, worldPositionStays);
		}
	}
}