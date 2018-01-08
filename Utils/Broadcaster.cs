using System;
using UnityEngine;

namespace DUCK.Utils
{
	public static class Broadcaster
	{
		public static void BroadcastTo<T>(GameObject gameObject, Action<T> execute, bool broadcastToChildren = true)
		{
			if (broadcastToChildren)
			{
				var children = gameObject.GetComponentsInChildren<T>();
				foreach (var child in children)
				{
					execute(child);
				}
			}
			else
			{
				var components = gameObject.GetComponents<T>();
				foreach (var component in components)
				{
					execute(component);
				}
			}
		}
	}
}