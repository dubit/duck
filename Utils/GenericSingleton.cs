using System;
using System.Reflection;

namespace DUCK.Utils
{
	/// <summary>
	/// A generic singleton class 
	/// </summary>
	/// <typeparam name="T">The class you are creating as a singleton</typeparam>
	public abstract class GenericSingleton<T> where T : GenericSingleton<T>
	{
		private static T instance;

		/// <summary>
		/// The current instance of this object
		/// </summary>
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					var t = typeof(T);
#if UNITY_WSA
					instance = (T)Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance);
#else
					instance = (T)Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
#endif
				}

				return instance;
			}
		}
	}
}
