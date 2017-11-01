using System;

namespace DUCK.Utils
{
	/// <summary>
	/// Collection of extension methods designed to
	/// improve Action Class
	/// </summary>
	public static class ActionExtensions
	{
		/// <summary>
		/// Checks if action is null and if not Invokes action
		/// </summary>
		/// <param name="action"></param>
		public static void SafeInvoke(this Action action)
		{
			if (action != null)
			{
				action.Invoke();
			}
		}

		/// <summary>
		/// Checks if action is null and if not Invokes action
		/// with parameter value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		/// <param name="value"></param>
		public static void SafeInvoke<T>(this Action<T> action, T value)
		{
			if (action != null)
			{
				action.Invoke(value);
			}
		}

		/// <summary>
		/// Checks if action is null and if not Invokes action
		/// with parameter value
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="action"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 value1, T2 value2)
		{
			if (action != null)
			{
				action.Invoke(value1, value2);
			}
		}

		/// <summary>
		/// Checks if action is null and if not Invokes action
		/// with parameter value
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <param name="action"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3)
		{
			if (action != null)
			{
				action.Invoke(value1, value2, value3);
			}
		}

		/// <summary>
		/// Checks if action is null and if not Invokes action
		/// with parameter value
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <param name="action"></param>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		/// <param name="value4"></param>
		public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 value1, T2 value2, T3 value3, T4 value4)
		{
			if (action != null)
			{
				action.Invoke(value1, value2, value3, value4);
			}
		}
	}
}
