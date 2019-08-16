using TMPro;
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
			var newText = translationFound ? localisedString : $"<color=red>{localisedValue.LocalisationKey}</color>";

			if (Component == null)
			{
				var textMeshPro = GetComponent<TextMeshProUGUI>();
				textMeshPro.text = newText;
			}
			else
			{
				Component.text = newText;
			}
		}
	}
}
