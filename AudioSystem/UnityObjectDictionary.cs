using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace DUCK.AudioSystem

{
	/// <summary>
	/// Unity Object Dictionary is designed as a Dictionary-like Container that holding an Unity Object as a reference key.
	/// </summary>
	/// <typeparam name="T1">Key Type [UnityEngine.Object]</typeparam>
	/// <typeparam name="T2">Value Type</typeparam>
	public class UnityObjectDictionary<T1, T2> where T1 : Object
	{
		private class Pair
		{
			public T1 Key { get; private set; }
			public T2 Value { get; private set; }

			public bool IsInvalid { get { return Key == null || Value == null; } }

			public Pair(T1 key, T2 value)
			{
				Key = key;
				Value = value;
			}

			/// <summary>
			/// A shortcut to check if the key is valid and also matches.
			/// </summary>
			/// <param name="key">The target object</param>
			/// <returns>True if the key is valid and matches</returns>
			public bool IsRequestedBy(T1 key)
			{
				return key != null && Key.Equals(key);
			}
		}

		private readonly List<Pair> pairs = new List<Pair>();

		/// <summary>
		/// Try to add a new request if it's a valid one.
		/// </summary>
		/// <param name="key">The target object</param>
		/// <param name="value">The value</param>
		/// <returns>True on success.</returns>
		public bool Add(T1 key, T2 value)
		{
			if (key == null || value == null || GetIndex(key) >= 0)
			{
				return false;
			}

			pairs.Add(new Pair(key, value));
			return true;
		}

		/// <summary>
		/// Try to remove the request.
		/// It also removes all invalid requests too.
		/// </summary>
		/// <param name="key">The target object</param>
		/// <returns>True on success.</returns>
		public bool Remove(T1 key)
		{
			var index = GetIndex(key);
			if (index >= 0)
			{
				pairs.RemoveAt(index);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Try to find a reqeust and invoke the callback.
		/// </summary>
		/// <param name="key">The target object</param>
		/// <returns>Default value of the value if the key is not exist</returns>
		public T2 Get(T1 key)
		{
			var index = GetIndex(key);
			return index >= 0 ? pairs[index].Value : default(T2);
		}

		/// <summary>
		/// Try to find a reqeust and invoke the callback.
		/// Will also remove the request on success.
		/// </summary>
		/// <param name="key">The target object</param>
		/// <returns>Default value of the value if the key is not exist</returns>
		public T2 GetAndRemove(T1 key)
		{
			var index = GetIndex(key);
			if (index < 0) return default(T2);

			var request = pairs[index];
			pairs.RemoveAt(index);
			return request.Value;
		}

		/// <summary>
		/// Get all valid requests and try to invoke them.
		/// Will also remove them from the list on success.
		/// </summary>
		/// <param name="method">The callback method on each successful predicate</param>
		/// <param name="predicate">[Optional] A predicate check. If null then always success.</param>
		public void GetAllAndRemove(Action<T1, T2> method, Func<T1, bool> predicate = null)
		{
			for (var i = pairs.Count - 1; i >= 0; --i)
			{
				var pair = pairs[i];
				if (pair.IsInvalid)
				{
					pairs.RemoveAt(i);
				}
				else if (predicate == null || predicate(pair.Key))
				{
					pairs.RemoveAt(i);
					method.Invoke(pair.Key, pair.Value);
				}
			}
		}

		/// <summary>
		/// Remove everything.
		/// </summary>
		public void Clear()
		{
			pairs.Clear();
		}

		private int GetIndex(T1 key)
		{
			for (var i = pairs.Count - 1; i >= 0; --i)
			{
				var pair = pairs[i];
				if (pair.IsInvalid)
				{
					pairs.RemoveAt(i);
				}
				else if (pair.IsRequestedBy(key))
				{
					return i;
				}
			}

			return -1;
		}
	}
}