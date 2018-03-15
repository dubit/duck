using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Pooling
{
	public abstract class PoolableComponent : MonoBehaviour
	{
		protected internal virtual void OnAddedToPool()
		{
			enabled = false;
			gameObject.SetActive(false);
		}

		protected internal virtual void OnRemovedFromPool()
		{
			gameObject.SetActive(true);
			enabled = true;
		}

		protected internal abstract void CopyFrom(PoolableComponent instance);
	}

	public static class ObjectPool
	{
		private abstract class AbstractPool
		{
			public abstract Type ObjectType { get; }

			public abstract void Add(PoolableComponent obj);
		}

		private class Pool<T> : AbstractPool 
			where T : PoolableComponent
		{
			private Queue<T> pooledObjects = new Queue<T>();

			public override Type ObjectType
			{
				get
				{
					return typeof(T);
				}
			}

			public T Remove()
			{
				var obj = (pooledObjects.Count > 0)
					? pooledObjects.Dequeue()
					: null;

				if (obj)
				{
					obj.OnRemovedFromPool();
				}

				return obj;
			}

			public override void Add(PoolableComponent obj)
			{
				if (obj != null)
				{
					obj.OnAddedToPool();
					pooledObjects.Enqueue((T)obj);
				}
			}
		}

		private static Dictionary<Type, AbstractPool> poolLookup = new Dictionary<Type, AbstractPool>();

		public static T Instantiate<T>(T original = null) where T : PoolableComponent
		{
			T obj = null;

			if (PoolExists(typeof(T)))
			{
				obj = GetFromPool<T>();

				if (obj != null)
				{
					if (original != null)
					{
						obj.CopyFrom(original);
					}

					return obj;
				}
			}
			else
			{
				CreatePool<T>();
			}

			return obj ?? GameObject.Instantiate<T>(original);
		}

		public static T Instantiate<T>(string resourcePath) where T : PoolableComponent
		{
			T obj = null;

			if (PoolExists(typeof(T)))
			{
				obj = GetFromPool<T>();
			}
			else
			{
				CreatePool<T>();
			}

			return obj ?? Resources.Load<T>(resourcePath);
		}

		public static void Destroy(PoolableComponent obj)
		{
			if (obj == null) return;

			if (PoolExists(obj.GetType()))
			{
				ReturnToPool(obj);
			}
			else
			{
				Destroy(obj);
			}
		}

		private static void CreatePool<T>() where T : PoolableComponent
		{
			if (PoolExists(typeof(T))) return;

			poolLookup.Add(typeof(T), new Pool<T>());
		}

		private static Pool<T> FindPool<T>() where T : PoolableComponent
		{
			if (!PoolExists(typeof(T))) throw new KeyNotFoundException(typeof(T).ToString());

			return (Pool <T>)poolLookup[typeof(T)];
		}

		private static bool PoolExists(Type type)
		{
			return poolLookup.ContainsKey(type);
		}

		private static T GetFromPool<T>() where T : PoolableComponent
		{
			return PoolExists(typeof(T))
				? FindPool<T>().Remove()
				: null;
		}

		private static void ReturnToPool(PoolableComponent obj)
		{
			if (!PoolExists(obj.GetType())) return;

			poolLookup[obj.GetType()].Add(obj);
		}
	}
}
