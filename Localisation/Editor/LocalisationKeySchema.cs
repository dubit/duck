using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	/// <summary>
	/// Key schema: the master-list of all localisation keys in the game, sorted by category, and what content type they refer to.
	/// This is used in generating the LocalisationConfig for application-side, and driving most of the related Editor UI.
	/// </summary>
	[Serializable]
	public class LocalisationKeySchema : ISerializationCallbackReceiver
	{
		[Serializable]
		public class LocalisationKeyCategory
		{
			public string name = string.Empty;
			public string[] keys = { };

			public override string ToString()
			{
				return name + "(" + keys.Length + ")";
			}
		}

		[SerializeField]
		public LocalisationKeyCategory[] categories;

		public struct KeySchemaLookup
		{
			public int categoryIndex;
			public int keyIndex;

			public bool IsValid { get { return categoryIndex >= 0 && keyIndex >= 0; } }
		}

		private Dictionary<string, Dictionary<string, KeySchemaLookup>> generatedLookupTable;

		public KeySchemaLookup FindKey(string targetCategory, string targetKey)
		{
			GenerateLookupTable();

			try
			{
				return generatedLookupTable[targetCategory][targetKey];
			}
			catch
			{
				return new KeySchemaLookup
				{
					categoryIndex = -1,
					keyIndex = -1
				};
			}
		}

		private void GenerateLookupTable()
		{
			if (generatedLookupTable != null || categories == null) return;

			generatedLookupTable = new Dictionary<string, Dictionary<string, KeySchemaLookup>>();

			for (var i = 0; i < categories.Length; i++)
			{
				var dict = new Dictionary<string, KeySchemaLookup>();

				if (categories[i] != null && categories[i].keys != null)
				{
					for (var j = 0; j < categories[i].keys.Length; j++)
					{
						dict[categories[i].keys[j]] = new KeySchemaLookup
						{
							categoryIndex = i,
							keyIndex = j
						};
					}

					generatedLookupTable[categories[i].name] = dict;
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			GenerateLookupTable();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			generatedLookupTable = null;
		}
	}
}