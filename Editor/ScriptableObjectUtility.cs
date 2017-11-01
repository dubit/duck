using UnityEditor;
using UnityEngine;

namespace DUCK.Editor
{
	public static class ScriptableObjectUtility
	{
		/// <summary>
		/// Create new asset from <see cref="ScriptableObject"/> type with unique name at
		/// selected folder in project window. Asset creation can be cancelled by pressing
		/// escape key when asset is initially being named.
		/// </summary>
		/// <typeparam name="T">Type of scriptable object.</typeparam>
		public static void CreateAsset<T>(string path = "") where T : ScriptableObject
		{
			var asset = ScriptableObject.CreateInstance<T>();
			path = string.IsNullOrEmpty(path) ? "New " + typeof(T).Name + ".asset" : path;

			ProjectWindowUtil.CreateAsset(asset, path);
		}
	}
}