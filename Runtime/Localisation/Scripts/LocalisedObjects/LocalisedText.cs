using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Localisation.LocalisedObjects
{
	/// <summary>
	/// Automatically updates a Text component's text to the current locale
	/// </summary>
	public class LocalisedText : AbstractLocalisedObject<Text>
	{
		protected override void HandleLocaleChanged(bool translationFound, string localisedString)
		{
			if (Component != null && Component is Text)
			{
				Component.text = translationFound
					? localisedString :
					$"<color=red>{localisedValue.LocalisationKey}</color>";
			}
		}
	}
}