using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Utils
{
	/// <summary>
	/// UpdateList: allows you to add multiple update functions to a list, then just update the list and all functions 
	/// will be called. It also allows removing an update function from the list during the update iteration.
	/// It is very well optimized and is a way to get around multiple monobehaviours. It also allows you to make 
	/// multiple instances, to update different objects with different time scales.
	/// </summary>
	public class UpdateList
	{
		private readonly List<Action<float>> updateFunctions = new List<Action<float>>();
		private readonly List<int> deadFunctionIndices = new List<int>();

		private bool isUpdating;
		private int currentUpdatingIndex;

		public void Add(Action<float> updateFunction)
		{
			if (updateFunctions.Contains(updateFunction))
			{
				Debug.LogWarning("You should not add the same update function more than once into update container.");
				return;
			}

			updateFunctions.Add(updateFunction);
		}

		public void Remove(Action<float> updateFunction)
		{
			var index = updateFunctions.IndexOf(updateFunction);
			if (index < 0) return;

			// if we are currently updating (and not updating the current one) we mark it as dead, 
			// otherwise (else) we can remove it straight away
			if (isUpdating && currentUpdatingIndex != index)
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

		public void Update(float dt)
		{
			isUpdating = true;
			for (currentUpdatingIndex = updateFunctions.Count - 1; currentUpdatingIndex >= 0; --currentUpdatingIndex)
			{
				if (deadFunctionIndices.Remove(currentUpdatingIndex))
				{
					updateFunctions.RemoveAt(currentUpdatingIndex);
				}
				else
				{
					updateFunctions[currentUpdatingIndex](dt);
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
			}
		}
	}
}