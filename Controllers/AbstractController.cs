using System;
using System.Collections.Generic;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Controllers
{
	/// <summary>
	/// The AbstractController is used to create a hierarchical structure across the application by
	/// creating children controllers and having reference to a parent controller
	/// </summary>
	public abstract class AbstractController : MonoBehaviour
	{
		public bool HasParent { get { return parentController != null; } }
		public int ChildCount { get { return childControllers.Count; } }

		protected bool IsApplicationQuitting { get; private set; }

		private bool isInitialized;
		private bool isDestroyed;
		private AbstractController parentController;
		private readonly List<AbstractController> childControllers = new List<AbstractController>();

		/// <summary>
		/// Invoked after child controllers are destroyed, and before the object is destroyed.
		/// If destroyed via GameObject.Destroy, then expecting (an Unity fake) null AbstractController.
		/// </summary>
		public event Action<AbstractController> OnDestroyed;

		/// <summary>
		/// Used for initial Controller setup, called from the create Controller functions.
		/// When overriding always call the base Initialize.
		/// </summary>
		/// <param name="parent">Parent AbstractController</param>
		/// <param name="args">The arguments passed into the Controller</param>
		public virtual void Initialize(AbstractController parent = null, object args = null)
		{
			if (isInitialized)
			{
				throw new Exception(GetType().Name + " is already initialized");
			}

			parentController = parent;
			isInitialized = true;
		}

		/// <summary>
		/// Destroys all child controllers and then destroys itself.
		/// </summary>
		public virtual void Destroy()
		{
			if (isDestroyed) return;

			isDestroyed = true;

			for (var i = childControllers.Count - 1; i >= 0; i--)
			{
				childControllers[i].Destroy();
			}

			if (OnDestroyed != null)
			{
				OnDestroyed.Invoke(this);
			}

#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				Destroy(gameObject);
			}
			else
			{
				DestroyImmediate(gameObject);
			}
#else
			Destroy(gameObject);
#endif
		}

		/// <summary>
		/// Remove a child controller.
		/// </summary>
		/// <param name="controller">The target controller</param>
		/// <typeparam name="TController">an AbstractController</typeparam>
		/// <returns>True if removed. Otherwise false.</returns>
		public bool RemoveController<TController>(TController controller)
			where TController : AbstractController
		{
			if (childControllers.Remove(controller))
			{
				// This should never happen -- unless the user did some dirty hacks
				if (controller.parentController != this)
				{
					throw new Exception("A child controller: " + controller.name +
					                    " has been removed, but it has a different parent!");
				}
				controller.OnDestroyed -= HandleChildControllerDestroyed;
				controller.parentController = null;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Add an existing controller to this controller.
		/// Will empty its previous parent connections.
		/// </summary>
		/// <param name="controller">The target controller</param>
		/// <param name="parent">A new parent transform</param>
		/// <param name="worldPositionStays">The Controller keeps its local orientation rather than its global orientation</param>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <returns>The target controller</returns>
		public TController AddController<TController>(TController controller, Transform parent = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			if (controller.HasParent)
			{
				controller.parentController.RemoveController(controller);
			}
			controller.transform.SetParent(parent, worldPositionStays);
			controller.OnDestroyed += HandleChildControllerDestroyed;
			childControllers.Add(controller);
			return controller;
		}

		/// <summary>
		/// Add an existing controller to this controller.
		/// Will empty its previous parent connections.
		/// Automatically put the controller into its transform
		/// </summary>
		/// <param name="controller">The target controller</param>
		/// <param name="worldPositionStays">The Controller keeps its local orientation rather than its global orientation</param>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <returns>The target controller</returns>
		public TController AddChildController<TController>(TController controller, bool worldPositionStays = true)
			where TController : AbstractController
		{
			return AddController(controller, transform, worldPositionStays);
		}

		/// <summary>
		/// Creates and Initializes a new Controller in a new GameObject
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="parent">The parent of the controller's transform</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController AddController<TController>(object args = null, Transform parent = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			var controller = new GameObject(typeof(TController).Name).AddComponent<TController>();
			controller.transform.SetParent(parent, worldPositionStays);
			controller.Initialize(this, args);
			controller.OnDestroyed += HandleChildControllerDestroyed;
			childControllers.Add(controller);
			return controller;
		}

		/// <summary>
		/// Creates and Initializes a new Controller in a new GameObject
		/// Automatically put the newly created controller into its transform
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController AddChildController<TController>(object args = null, bool worldPositionStays = true)
			where TController : AbstractController
		{
			return AddController<TController>(args, transform, worldPositionStays);
		}

		/// <summary>
		/// Loads, Instantiates and Initializes a Controller prefab
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="path">The resource path to the Controller prefab</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="parent">The parent of the controller's transform</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController AddResourceController<TController>(string path, object args = null, Transform parent = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			var controller = Instantiator.InstantiateResource<TController>(path, parent, worldPositionStays);
			controller.Initialize(this, args);
			controller.OnDestroyed += HandleChildControllerDestroyed;
			childControllers.Add(controller);
			return controller;
		}

		/// <summary>
		/// Loads, Instantiates and Initializes a Controller prefab
		/// Automatically put the newly created controller into its transform
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="path">The resource path to the Controller prefab</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController AddResourceChildController<TController>(string path, object args = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			return AddResourceController<TController>(path, args, transform, worldPositionStays);
		}

		/// <summary>
		/// Async loads, Instantiates and Initializes a Controller prefab
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="path">The resource path to the Controller prefab</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="parent">The parent of the controller's transform</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <param name="onComplete">The callback when the async load completed</param>
		public void AddResourceControllerAsync<TController>(string path, object args = null, Transform parent = null,
			bool worldPositionStays = true, Action<TController> onComplete = null)
			where TController : AbstractController
		{
			Instantiator.InstantiateResourceAsync<TController>(path, controller =>
			{
				controller.Initialize(this, args);
				controller.OnDestroyed += HandleChildControllerDestroyed;
				childControllers.Add(controller);
				if (onComplete != null)
				{
					onComplete.Invoke(controller);
				}
			}, parent, worldPositionStays);
		}

		/// <summary>
		/// Async loads, Instantiates and Initializes a Controller prefab
		/// Automatically put the newly created controller into its transform
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="path">The resource path to the Controller prefab</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <param name="onComplete">The callback when the async load completed</param>
		public void AddResourceChildControllerAsync<TController>(string path, object args = null,
			bool worldPositionStays = true, Action<TController> onComplete = null)
			where TController : AbstractController
		{
			AddResourceControllerAsync(path, args, transform, worldPositionStays, onComplete);
		}

		/// <summary>
		/// Instantiates a clone of the given controller and initializes it
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="toClone">The already instantiated AbstractController</param>
		/// <param name="parent">The parent of the controller's transform</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController CloneController<TController>(TController toClone, Transform parent, object args = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			var controller = Instantiator.Instantiate(toClone, parent, worldPositionStays);
			controller.Initialize(this, args);
			controller.OnDestroyed += HandleChildControllerDestroyed;
			childControllers.Add(controller);
			return controller;
		}

		/// <summary>
		/// Instantiates a clone of the given controller and initializes it
		/// The cloned controller's transform parent will stay the same with the original one
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="toClone">The already instantiated AbstractController</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController CloneController<TController>(TController toClone, object args = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			return CloneController(toClone, toClone.transform.parent, args, worldPositionStays);
		}

		/// <summary>
		/// Instantiates a clone of the given controller and initializes it
		/// Automatically put the newly created controller into its transform
		/// </summary>
		/// <typeparam name="TController">A type of AbstractController</typeparam>
		/// <param name="toClone">The already instantiated AbstractController</param>
		/// <param name="args">The arguments to be passed to the Controller</param>
		/// <param name="worldPositionStays">Make the world position stays when it attachs on the parent transform</param>
		/// <returns>The instantiated AbstractController</returns>
		public TController CloneChildController<TController>(TController toClone, object args = null,
			bool worldPositionStays = true)
			where TController : AbstractController
		{
			return CloneController(toClone, transform, args, worldPositionStays);
		}

		/// <summary>
		/// Called on the destruction of a child controller so that we can remove our reference
		/// </summary>
		/// <param name="controller">The child controller being destroyed</param>
		private void HandleChildControllerDestroyed(AbstractController controller)
		{
			if (!childControllers.Contains(controller))
			{
				throw new Exception(gameObject.name + " does not hold reference to a controller of type " +
				                    controller.GetType().Name + ". Are you sure this is my child?");
			}

			childControllers.Remove(controller);
		}

		/// <summary>
		/// Called on all MonoBehaviours before Destroy when the application is quitting.
		/// Caching when the application is quitting is used to prevent race conditions on close,
		/// e.g. unsubscribing from another object's event on Destroy when the other object may get destroyed first.
		/// </summary>
		private void OnApplicationQuit()
		{
			IsApplicationQuitting = true;
		}

		/// <summary>
		/// Give the users a warning (CS0108) that if the derived class have another OnDestroy method decleard.
		/// (The user should "new" the method if they really want to break it)
		/// </summary>
		protected void OnDestroy()
		{
			if (isDestroyed) return;

			Destroy();
		}
	}
}