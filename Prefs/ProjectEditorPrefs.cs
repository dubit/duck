using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace DUCK.Prefs
{
	public static class ProjectEditorPrefs
	{
		public static Dictionary<string, Type> StoredPrefs { get; private set; }
		private readonly static string projectName;

		static ProjectEditorPrefs()
		{
			projectName = Application.productName;
			StoredPrefs = new Dictionary<string, Type>();
		}

		public static T Get<T>(string key, T defaultValue = default(T))
		{
			var storedKey = projectName + ":" + key;
			if (StoredPrefs.ContainsKey(key))
			{
				return GetEditorPref<T>(storedKey);
			}
			// TODO: check editor prefs for existing value
			else
			{

			}

			Set<T>(key, defaultValue);
			return defaultValue;
		}

		public static object Get(string key)
		{
			var storedKey = projectName + ":" + key;
			if (StoredPrefs.ContainsKey(key))
			{
				return GetEditorPref(storedKey, StoredPrefs[key]);
			}

			throw new Exception("Key could not be found");
		}

		public static void Set<T>(string key, T value)
		{
			Set(key, value, typeof(T));
		}

		public static void Set(string key, object value, Type type)
		{
			var storedKey = projectName + ":" + key;
			SetEditorPref(storedKey, value, type);
			StoredPrefs[key] = type;
		}

		private static T GetEditorPref<T>(string key)
		{
			return (T) GetEditorPref(key, typeof(T));
		}

		private static object GetEditorPref(string key, Type type)
		{
			if (type == typeof(string))
			{
				return EditorPrefs.GetString(key);
			}
			if (type == typeof(bool))
			{
				return EditorPrefs.GetBool(key);
			}
			if (type == typeof(int))
			{
				return EditorPrefs.GetInt(key);
			}
			if (type == typeof(float))
			{
				return EditorPrefs.GetFloat(key);
			}

			return null;
		}

		private static void SetEditorPref(string key, object value, Type type)
		{
			if (type == typeof(string))
			{
				EditorPrefs.SetString(key, (string)value);
			}
			if (type == typeof(bool))
			{
				EditorPrefs.SetBool(key, (bool)value);
			}
			if (type == typeof(int))
			{
				 EditorPrefs.SetInt(key, (int)value);
			}
			if (type == typeof(float))
			{
				 EditorPrefs.SetFloat(key, (float)value);
			}
		}
	}
}
#endif