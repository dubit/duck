using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Utils
{
	/// <summary>
	/// Instantiator is a helper class which can speed up the resource loading / initialising (for your code).
	/// </summary>
	public static class Instantiator
	{
		/// <summary>
		/// Instantiates an unloaded Object in the Resources folder.
		/// Then return the Object.
		/// </summary>
		/// <param name="path">The path to the Object inside the resource folder</param>
		/// <param name="parent">The transform to be set as the parent for the new Object</param>
		/// <param name="worldPositionStays">Makes the world position stay when it attaches onto the parent transform</param>
		/// <returns>The instantiated (GameObject's) Component</returns>
		public static T InstantiateResource<T>(string path, Transform parent = null, bool worldPositionStays = true)
			where T : Object
		{
			var loadedObject = Resources.Load<T>(path);
			if (loadedObject == null)
			{
				throw new Exception("Could not find resource (" + typeof(T).Name + ") in " + path);
			}
			return Instantiate(loadedObject, parent, worldPositionStays);
		}

		/// <summary>
		/// Load a resource in the Resources folder, return the request.
		/// Then instantiate the resource, invoke the callback.
		/// </summary>
		/// <param name="path">The path to the Object inside the resource folder</param>
		/// <param name="onLoadCompete">The callback function on resource load</param>
		/// <param name="parent">The transform to be set as the parent for the new Object</param>
		/// <param name="worldPositionStays">Makes the world position stay when it attaches onto the parent transform</param>
		/// <typeparam name="T">Any Component</typeparam>
		/// <returns>The resource request it created</returns>
		public static ResourceRequest InstantiateResourceAsync<T>(string path, Action<T> onLoadCompete, Transform parent = null, bool worldPositionStays = true)
			where T : Object
		{
			return LoadResourceAsync<T>(path, loadedObject => onLoadCompete(Instantiate(loadedObject, parent, worldPositionStays)));
		}

		/// <summary>
		/// Instantiates the Component on a loaded GameObject, renames and sets its parent.
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="obj">The target object</param>
		/// <param name="parent">The transform to be set as the parent for the new Object</param>
		/// <param name="worldPositionStays">Makes the world position stay when it attaches onto the parent transform</param>
		/// <returns>The new instantiated version of the obj</returns>
		public static T Instantiate<T>(T obj, Transform parent = null, bool worldPositionStays = true)
			where T : Object
		{
			if (obj == null)
			{
				throw new Exception("Loaded object cannot be null");
			}
			var instantiateComponent = Object.Instantiate(obj, parent, worldPositionStays);
			instantiateComponent.name = obj.name;
			return instantiateComponent;
		}

		/// <summary>
		/// Load a resource in the Resources folder, return the request.
		/// Then do the callback after it is loaded.
		/// </summary>
		/// <param name="path">The path to the Object inside the resource folder</param>
		/// <param name="onLoadCompete">The callback function on resource load</param>
		/// <typeparam name="T">Any Object</typeparam>
		/// <returns>The Resource Request</returns>
		/// <exception cref="Exception">Invalid path</exception>
		public static ResourceRequest LoadResourceAsync<T>(string path, Action<T> onLoadCompete)
			where T : Object
		{
			var request = Resources.LoadAsync<T>(path);
			if (request.asset == null && request.isDone)
			{
				throw new Exception("Could not find resource (" + typeof(T).Name + ") in " + path);
			}
			MonoBehaviourService.Instance.StartCoroutine(UpdateAsyncProgress(request, () => onLoadCompete((T)request.asset)));
			return request;
		}

		/// <summary>
		/// A helper (for coroutine) that callback when an async operation is done.
		/// </summary>
		/// <param name="request">Target async operation</param>
		/// <param name="onLoadCompelte">Callback function when the async operation is done</param>
		private static IEnumerator UpdateAsyncProgress(AsyncOperation request, Action onLoadCompelte)
		{
			yield return new WaitUntil(() => request.isDone);
			onLoadCompelte();
		}
	}
}