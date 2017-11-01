using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Localisation
{
	/// <summary>
	/// Data class representing a table of [localisationKey --> localisedContent]
	/// (thin wrapper for a Dictionary)
	/// </summary>
	[Serializable]
	public class LocalisationTable : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private string[] locales = {};

		[SerializeField]
		private List<int> keys = new List<int>();

		[SerializeField]
		private List<string> values = new List<string>();

		[SerializeField]
		public int crcEncodingVersion;

		public ICollection<string> SupportedLocales { get { return locales; } }

		private readonly Dictionary<int, string> data = new Dictionary<int, string>();

		public string GetString(int key)
		{
			if (data.ContainsKey(key))
			{
				return data[key];
			}

			throw new KeyNotFoundException(string.Format("Localisation value missing for key: {0}", key));
		}

		public void SetData(Dictionary<int, string> newData, bool overwriteEmptyValuesOnly = false)
		{
			var enumerator = newData.GetEnumerator();
			for (var e = enumerator; enumerator.MoveNext();)
			{
				var kvp = e.Current;
				SetData(kvp.Key, kvp.Value, overwriteEmptyValuesOnly);
			}
			enumerator.Dispose();
		}

		public void SetData(int key, string value, bool onlyIfEmpty = false)
		{
			if (!onlyIfEmpty || !data.ContainsKey(key) || string.IsNullOrEmpty(data[key]))
			{
				data[key] = value;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			foreach (var kvp in data)
			{
				keys.Add(kvp.Key);
				values.Add(kvp.Value);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			data.Clear();
			for (var i = 0; i < Math.Min(keys.Count, values.Count); i++)
			{
				data.Add(keys[i], values[i]);
			}
		}
	}
}
