using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Tween
{
	/// <summary>
	/// Animation Driver should maintain a list of update
	/// </summary>
	public interface IAnimationDriver
	{
		/// <summary>
		/// Add the Update Function into the update list.
		/// </summary>
		/// <param name="updateFunction">The target Update Function</param>
		void Add(Action<float> updateFunction);

		/// <summary>
		/// Remove the Update Function from the update list.
		/// </summary>
		/// <param name="updateFunction">The target Update Function</param>
		void Remove(Action<float> updateFunction);
	}

	public class DefaultAnimationDriver : MonoBehaviour, IAnimationDriver
	{
		public static DefaultAnimationDriver Instance { get { return instance == null ? CreateInstance() : instance; } }
		private static DefaultAnimationDriver instance;

		private static readonly List<Action<float>> updateFunctions = new List<Action<float>>();
		private static readonly List<int> deadFunctionIndices = new List<int>();

		private bool isUpdating;
		private int currentUpdateID;

		private static DefaultAnimationDriver CreateInstance()
		{
			var gameObject = new GameObject { hideFlags = HideFlags.HideInHierarchy };
			DontDestroyOnLoad(gameObject);
			return instance = gameObject.AddComponent<DefaultAnimationDriver>();
		}

		public void Add(Action<float> updateFunction)
		{
			if (updateFunctions.Contains(updateFunction))
			{
				Debug.LogWarning("You should not add the same update function more than once into the animation drive.");
				return;
			}

			updateFunctions.Add(updateFunction);
		}

		public void Remove(Action<float> updateFunction)
		{
			var index = updateFunctions.IndexOf(updateFunction);
			if (index < 0) return;

			// If it's updating and not a self-destroy request (stop itself during update)
			// Then we push it into the removal list
			if (isUpdating && currentUpdateID != index)
			{
				if (!deadFunctionIndices.Contains(index))
				{
					deadFunctionIndices.Add(index);
				}
			}
			else
			{
				updateFunctions.RemoveAt(index);
			}
		}

		private void Update()
		{
			isUpdating = true;
			for (currentUpdateID = updateFunctions.Count - 1; currentUpdateID >= 0; --currentUpdateID)
			{
				if (deadFunctionIndices.Remove(currentUpdateID))
				{
					updateFunctions.RemoveAt(currentUpdateID);
				}
				else
				{
					updateFunctions[currentUpdateID](Time.deltaTime);
				}
			}
			isUpdating = false;

			// Clean up the rest of the dead update functions (have to do this in one frame)
			if (deadFunctionIndices.Count > 0)
			{
				// We have to remove the function descendingly or sort the indices by descending order
				// Because if you remove the function in the middle of the list, the rest part of functions indecies will be changed.
				for (var i = updateFunctions.Count - 1; i >= 0; --i)
				{
					if (deadFunctionIndices.Remove(i))
					{
						updateFunctions.RemoveAt(i);
					}
				}
				// We should have nothing left... or something must went wrong seriously.
				if (deadFunctionIndices.Count > 0)
				{
					Debug.LogError("This should never happen! Please investigate the Default Animation Driver.");
					deadFunctionIndices.Clear();
				}
			}
		}
	}
}