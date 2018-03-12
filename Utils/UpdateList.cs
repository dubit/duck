using System;
using System.Collections.Generic;

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

		public bool Contains(Action<float> updateFunction)
		{
			var index = updateFunctions.IndexOf(updateFunction);
			return index != -1 && !deadFunctionIndices.Contains(index);
		}

		public void Add(Action<float> updateFunction)
		{
			var index = updateFunctions.IndexOf(updateFunction);
			if (index >= 0)
			{
				// if the function exists but it's marked as dead we should, unmark it (to re add it)
				if (deadFunctionIndices.Contains(index))
				{
					deadFunctionIndices.Remove(index);
					return;
				}

				throw new Exception("Cannot add the given function. It is already in the list");
			}

			updateFunctions.Add(updateFunction);
		}

		public void Remove(Action<float> updateFunction)
		{
			var index = updateFunctions.IndexOf(updateFunction);
			if (index < 0)
			{
				throw new Exception("Cannot remove the given function. It was not found in the list");
			}

			// if we are currently updating we mark it as dead,
			// otherwise (else) we can remove it straight away
			if (isUpdating)
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
			for (var i = updateFunctions.Count - 1; i >= 0; --i)
			{
				if (!deadFunctionIndices.Contains(i))
				{
					updateFunctions[i](dt);
				}
			}
			isUpdating = false;

			if (deadFunctionIndices.Count > 0)
			{
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