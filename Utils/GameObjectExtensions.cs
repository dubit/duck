using UnityEngine;

namespace DUCK.Utils
{
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Returns true if game object is in view of given camera
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static bool IsInViewOf(this GameObject gameObject, Camera camera)
		{
			return gameObject.transform.IsInViewOf(camera);
		}
	}
}
