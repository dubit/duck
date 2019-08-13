using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Localisation.LocalisedObjects
{
	/// <summary>
	/// Automatically updates an Image's sprite to a different file path specified in the localisation table for the current locale
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class LocalisedImage : AbstractLocalisedObject<Image>
	{
		protected override void HandleLocaleChanged(bool translationFound, string localisedString)
		{
			if (translationFound)
			{
				var newSprite = Resources.Load<Sprite>(localisedString);

				if (newSprite != null)
				{
					Component.sprite = newSprite;
				}
			}
			else
			{
				Component.sprite = null;
			}
		}
	}
}