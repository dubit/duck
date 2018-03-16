using DUCK.Utils;
using System;
using UnityEngine;

namespace DUCK.Pooling
{
	public class PoolableObject : MonoBehaviour
	{
		public event Action OnAddedToPool;
		public event Action OnRemovedFromPool;

		public void AddToPool()
		{
			OnAddedToPool.SafeInvoke();
		}

		public void RemoveFromPool()
		{
			OnRemovedFromPool.SafeInvoke();
		}

		public void CopyFrom(PoolableObject instance) { }
	}
}
