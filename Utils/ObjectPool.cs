using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Pooling
{
	public static class ObjectPool
	{
		private interface Pool
		{
			Type ObjectType { get; }

			void Add(PoolableObject obj);
		}

		private class Pool<T> : Pool 
			where T : PoolableObject
		{
			private Queue<T> pooledObjects = new Queue<T>();

			public Type ObjectType
			{
				get
				{
					return typeof(T);
				}
			}

			public T Get()
			{
				var obj = (pooledObjects.Count > 0)
					? pooledObjects.Dequeue()
					: null;

				if (obj)
				{
					obj.RemoveFromPool();
				}

				return obj;
			}

			public void Add(PoolableObject obj)
			{
				if (obj != null)
				{
					obj.AddToPool();
					pooledObjects.Enqueue((T)obj);
				}
			}
		}

		private static Dictionary<Type, Pool> poolLookup = new Dictionary<Type, Pool>();

		public static T Instantiate<T>(T original = null) where T : PoolableObject
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

		public static T Instantiate<T>(string resourcePath) where T : PoolableObject
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

		public static void Destroy(PoolableObject obj)
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

		private static void CreatePool<T>() where T : PoolableObject
		{
			if (PoolExists(typeof(T))) return;

			poolLookup.Add(typeof(T), new Pool<T>());
		}

		private static Pool<T> FindPool<T>() where T : PoolableObject
		{
			if (!PoolExists(typeof(T))) throw new KeyNotFoundException(typeof(T).ToString());

			return (Pool <T>)poolLookup[typeof(T)];
		}

		private static bool PoolExists(Type type)
		{
			return poolLookup.ContainsKey(type);
		}

		private static T GetFromPool<T>() where T : PoolableObject
		{
			return PoolExists(typeof(T))
				? FindPool<T>().Get()
				: null;
		}

		private static void ReturnToPool(PoolableObject obj)
		{
			if (!PoolExists(obj.GetType())) return;

			poolLookup[obj.GetType()].Add(obj);
		}
	}
}
