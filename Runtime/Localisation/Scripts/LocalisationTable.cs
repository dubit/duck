using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Localisation
{
	/// <summary>
	/// Data class representing a dictionary-style table of [localisationKey(CRC) --> localisedContent] for a specific language
	/// </summary>
	[Serializable]
	public class LocalisationTable : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private string[] locales = {};
		public ICollection<string> SupportedLocales => locales;

		[SerializeField]
		private int crcEncodingVersion;

		[SerializeField]
		private List<int> keys = new List<int>();

		[SerializeField]
		private List<string> values = new List<string>();

		/// <summary>
		/// The version of CRC encoding which was used to make this table.
		/// If the way CRCs are encoded changes, the old version should still be supported.
		/// See LocalisationEditor.GetCRCWithEncodingVersion
		/// </summary>
		public int CRCEncodingVersion { get { return crcEncodingVersion; } }

		private readonly Dictionary<int, string> data = new Dictionary<int, string>();

		/// <summary>
		/// Returns the localised string which matches the key CRC supplied
		/// </summary>
		public string GetString(int key)
		{
			if (data.ContainsKey(key))
			{
				return data[key];
			}

			throw new KeyNotFoundException(string.Format("Localisation value missing for key: {0}", key));
		}

		/// <summary>
		/// Adds key-value pairs of CRC(int) --> text(string), optionally overwriting values only if they are empty
		/// <param name="newData">The key-value pairs of CRC->text to add to the localisation table</param>
		/// <param name="overwriteOnlyIfEmpty">If true, this will overwrite duplicate keys only if they don't already exist</param>
		/// </summary>
		public void SetData(IEnumerable<KeyValuePair<int, string>> newData, bool overwriteOnlyIfEmpty = false)
		{
			var enumerator = newData.GetEnumerator();
			for (var e = enumerator; enumerator.MoveNext();)
			{
				var kvp = e.Current;
				SetData(kvp.Key, kvp.Value, overwriteOnlyIfEmpty);
			}
			enumerator.Dispose();
		}
		/// <summary>
		/// Sets a single pair of CRC(int) --> text(string), optionally overwriting values only if they are empty
		/// <param name="key">The CRC (key) being set</param>
		/// <param name="key">The string (value) being set for this key</param>
		/// <param name="overwriteOnlyIfEmpty">If true, this will write the key & value only if the key doesn't already exist in the table</param>
		/// </summary>
		public void SetData(int key, string value, bool overwriteOnlyIfEmpty = false)
		{
			if (!overwriteOnlyIfEmpty || !data.ContainsKey(key))
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
