using UnityEngine.Events;

namespace DUCK.Utils
{
	/// <summary>
	/// Collection of extension methods designed to improve the UnityEvent class.
	/// </summary>
	public static class UnityEventExtensions
	{
		/// <summary>
		/// Checks if unityEvent is null and if not, invokes unityEvent
		/// </summary>
		/// <param name="unityEvent"></param>
		public static void SafeInvoke(this UnityEvent unityEvent)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		/// <summary>
		/// Checks if unityEvent is null and if not, invokes unityEvent
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="unityEvent"></param>
		/// <param name="value"></param>
		public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T value)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke(value);
			}
		}

		/// <summary>
		/// Checks if unityEvent is null and if not, invokes unityEvent
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="unityEvent"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		public static void SafeInvoke<T1, T2>(this UnityEvent<T1, T2> unityEvent, T1 value1, T2 value2)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke(value1, value2);
			}
		}

		/// <summary>
		/// Checks if unityEvent is null and if not, invokes unityEvent
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="unityEvent"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		public static void SafeInvoke<T1, T2, T3>(this UnityEvent<T1, T2, T3> unityEvent, T1 value1, T2 value2, T3 value3)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke(value1, value2, value3);
			}
		}

		/// <summary>
		/// Checks if unityEvent is null and if not, invokes unityEvent
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <param name="unityEvent"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		/// <param name="value4"></param>
		public static void SafeInvoke<T1, T2, T3, T4>(this UnityEvent<T1, T2, T3, T4> unityEvent, T1 value1, T2 value2, T3 value3, T4 value4)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke(value1, value2, value3, value4);
			}
		}
	}
}