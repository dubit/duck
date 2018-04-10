using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Utils
{
	/// <summary>
	/// Allows access to MonoBehaviour like events to non MonoBehaviours
	/// </summary>
	public sealed class MonoBehaviourService : GenericSingleton<MonoBehaviourService>, IMonoBehaviourService
	{
		/// <summary>
		/// OnUpdate is called every frame.
		/// </summary>
		public event Action OnUpdate;

		/// <summary>
		/// OnLateUpdate is called every frame at the end of the 
		/// <a href="https://docs.unity3d.com/Manual/ExecutionOrder.html">Game Logic Update</a>
		/// </summary>
		public event Action OnLateUpdate;

		/// <summary>
		/// OnFixedUpdate is called every fixed framerate frame.
		/// </summary>
		public event Action OnFixedUpdate;

		/// <summary>
		/// ApplicationPause is called when the player pauses and resumes.
		/// </summary>
		public event Action<bool> ApplicationPause;

		/// <summary>
		/// ApplicationQuit is called before the application is quit.
		/// </summary>
		public event Action ApplicationQuit;

		/// <summary>
		/// ApplicationFocus is called when the player gets or loses focus.
		/// </summary>
		public event Action<bool> ApplicationFocus;

		/// <summary>
		/// OnLevelLoaded is called when a scene has loaded.
		/// </summary>
		public event Action OnLevelLoaded;

		private readonly MonoBehaviourServiceBehaviour serviceInstance;

		/// <summary>
		/// Starts a coroutine.
		/// </summary>
		/// <param name="toRun">The routine to run</param>
		/// <returns>The coroutine</returns>
		public Coroutine StartCoroutine(IEnumerator toRun)
		{
			return serviceInstance.StartCoroutine(toRun);
		}

		/// <summary>
		/// Stops a coroutine.
		/// </summary>
		/// <param name="toStop">Coroutine to stop</param>
		public void StopCoroutine(Coroutine toStop)
		{
			if (toStop != null)
			{
				serviceInstance.StopCoroutine(toStop);
			}
			else
			{
				throw new ArgumentException("toStop");
			}
		}

		private MonoBehaviourService()
		{
			var gameObject = new GameObject { hideFlags = HideFlags.HideInHierarchy };
			//NOTE When running tests you cannot use DontDestroyOnLoad in editor mode
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gameObject);
			}
			serviceInstance = gameObject.AddComponent<MonoBehaviourServiceBehaviour>();

			serviceInstance.OnLevelLoaded += () => { OnLevelLoaded.SafeInvoke(); };

			serviceInstance.OnLateUpdate += () => { OnLateUpdate.SafeInvoke(); };
			serviceInstance.OnUpdate += () => { OnUpdate.SafeInvoke(); };
			serviceInstance.OnFixedUpdate += () => { OnFixedUpdate.SafeInvoke(); };

			serviceInstance.ApplicationPause += paused => { ApplicationPause.SafeInvoke(paused); };
			serviceInstance.ApplicationFocus += focused => { ApplicationFocus.SafeInvoke(focused); };
			serviceInstance.ApplicationQuit += () => { ApplicationQuit.SafeInvoke(); };
		}
	}
}
