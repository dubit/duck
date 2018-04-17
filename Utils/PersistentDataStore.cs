using System;
using System.IO;
using UnityEngine;

namespace DUCK.Utils
{
	public static class PersistentDataStore
	{
		private static readonly string SaveDataPath = Application.persistentDataPath + "/PersistentData/";

		public static bool Exists<T>(string uid = "")
		{
			return File.Exists(GetFilePath<T>(uid));
		}

		public static void Save<T>(T dataObject, string uid = "")
		{
			if (dataObject == null) throw new ArgumentException("Cannot save a null object.");

			File.WriteAllText(GetFilePath<T>(uid), JsonUtility.ToJson(dataObject));
		}

		public static T Load<T>(string uid = "")
		{
			var path = GetFilePath<T>(uid);

			try
			{
				return JsonUtility.FromJson<T>(File.ReadAllText(path));
			}
			catch
			{
				return default(T);
			}
		}

		public static bool Delete<T>(string uid = "")
		{
			var path = GetFilePath<T>(uid);

			if (File.Exists(path))
			{
				File.Delete(path);
				return true;
			}

			return false;
		}

		private static string GetFilePath<T>(string uid)
		{
			if (!Directory.Exists(SaveDataPath))
			{
				Directory.CreateDirectory(SaveDataPath);
			}

			var name = typeof(T).ToString()
				+ ((!string.IsNullOrEmpty(uid))
				? "-" + uid
				: ""); 

			return SaveDataPath + name + ".json";
		}
	}
}
