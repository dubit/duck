
using TMPro;

namespace DUCK.Localisation.LocalisedObjects
{
	/// <summary>
	/// Automatically updates a Text component's text to the current locale
	/// </summary>
	public class LocalisedTextMesh : AbstractLocalisedObject<TextMeshProUGUI>
	{
		protected override void HandleLocaleChanged(bool translationFound, string localisedString)
		{
			if (Component != null)
			{
				Component.text = translationFound
					? localisedString :
					$"<color=red>{localisedValue.LocalisationKey}</color>";
			}
		}
	}
}