using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DUCK.Utils
{
	/// <summary>
	/// A type safe Transform (Prefab) ComponentInstantiator
	/// </summary>
	public class PrefabInstantiator : ComponentInstantiator<Transform> {}

	/// <summary>
	/// This behaviour is used to avoid breaking prefab references when nesting prefabs.
	/// The prefab will be displayed in the scene at editor time as it would be at runtime.
	/// </summary>
	/// <typeparam name="T">The type of component the on the prefab</typeparam>
	[ExecuteInEditMode]
	public abstract class ComponentInstantiator<T> : MonoBehaviour
		where T : Component
	{
		public T InstantiatedPrefab { get; private set; }

		[SerializeField]
		private T prefab;

		[SerializeField]
		private bool instantiateOnAwake = true;

#if UNITY_EDITOR
		protected Transform editorPrefab;
#endif

		public event Action<T> OnInstantiated;

		private void Awake()
		{
			// If the application is running then create the real instance of the prefab
			if (!Application.isPlaying) return;

			if (instantiateOnAwake)
			{
				Instantiate();
			}
		}

		public bool Instantiate()
		{
			if (InstantiatedPrefab != null) return false;

			InstantiatedPrefab = Instantiator.Instantiate(prefab, transform.parent, false);
			OnInstantiate();

			// Safe invoke for any event listeners
			OnInstantiated.SafeInvoke(InstantiatedPrefab);

			return true;
		}

		protected virtual void OnInstantiate()
		{
			InstantiatedPrefab.transform.localPosition = transform.localPosition;
			InstantiatedPrefab.transform.localRotation = transform.localRotation;
			InstantiatedPrefab.transform.localScale = transform.localScale;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Set postion, rotation and scale of the editorPrefab transform
		/// </summary>
		protected virtual void SetTransformInfo()
		{
			// Match the intantiators position, rotation and scale on the editor prefab
			editorPrefab.localPosition = Vector3.zero;
			editorPrefab.localRotation = Quaternion.identity;
			editorPrefab.localScale = Vector3.one;
		}

		protected virtual void OnEnable()
		{
			const string GAMEOBJECT_NAME = "{0}Instantiator ({1})";

			// Return if we are at runtime
			if (Application.isPlaying) return;

			// If an editor prefab already exists then destroy it so we can create a fresh up to date version
			if (editorPrefab)
			{
				Destroy(editorPrefab.gameObject);
			}

			// Return if we dont have any prefab to create
			if (!prefab) return;

			// Create the prefab and set the flags to DontSave.
			editorPrefab = Instantiate(prefab.transform, transform);
			editorPrefab.name = prefab.name;
			editorPrefab.gameObject.hideFlags = HideFlags.DontSave;
			editorPrefab.localPosition = Vector3.zero;

			// Add the prefab name to the instantiators gameobject name quick identification
			transform.name = string.Format(GAMEOBJECT_NAME, typeof(T).Name, prefab.name);
		}

		protected virtual void Update()
		{
			if (Application.isPlaying || !editorPrefab) return;

			// Force any selection of the editor version of the prefab to select this gameObject instead
			if (Selection.activeGameObject == editorPrefab)
			{
				Selection.activeGameObject = gameObject;
			}

			SetTransformInfo();
		}

		protected virtual void OnDisable()
		{
			transform.DestroyAllChildren();
		}

#endif
	}
}