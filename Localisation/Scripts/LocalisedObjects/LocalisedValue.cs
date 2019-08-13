using System;
using UnityEngine;

namespace DUCK.Localisation.LocalisedObjects
{
	[Serializable]
	public class LocalisedValue
	{
#if UNITY_EDITOR
		// Only used by Editor for human-readable display options
		// When built, all categories and keys are compiled away to CRC int values
		[SerializeField]
		protected string keyName;
		public string KeyName => keyName;

		[SerializeField]
		protected string categoryName;
		public string CategoryName => categoryName;
#endif

		[SerializeField]
		protected int localisationKey;
		public int LocalisationKey => localisationKey;

		public string StringValue
		{
			get
			{
				var localisedText = "";
				if (Localiser.GetLocalisedString(LocalisationKey, out localisedText))
				{
					return localisedText;
				}

				return $"No translation found! {LocalisationKey}";
			}
		}
	}
}