using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DUCK.Utils
{
	public sealed class MonoBehaviourServiceBehaviour : MonoBehaviour, IMonoBehaviourService
	{
		public event Action OnUpdate;
		public event Action OnLateUpdate;
		public event Action OnFixedUpdate;

		public event Action<bool> ApplicationPause;
		public event Action ApplicationQuit;
		public event Action<bool> ApplicationFocus;

		public event Action OnLevelLoaded;

		private static MonoBehaviourServiceBehaviour instance;

		#region Scene Load Events

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				throw new Exception("There must only one service at a time");
			}

			SceneManager.sceneLoaded += HandleSceneLoaded;
		}

		private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			OnLevelLoaded.SafeInvoke();
		}

		#endregion

		#region Update Events

		private void Update()
		{
			OnUpdate.SafeInvoke();
		}

		private void LateUpdate()
		{
			OnLateUpdate.SafeInvoke();
		}

		private void FixedUpdate()
		{
			OnFixedUpdate.SafeInvoke();
		}

		#endregion

		#region Application Events

		private void OnApplicationPause(bool paused)
		{
			ApplicationPause.SafeInvoke(paused);
		}

		private void OnApplicationQuit()
		{
			ApplicationQuit.SafeInvoke();
		}

		private void OnApplicationFocus(bool focused)
		{
			ApplicationFocus.SafeInvoke(focused);
		}

		#endregion

		#region Deconstruction

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= HandleSceneLoaded;
		}

		#endregion
	}
}
