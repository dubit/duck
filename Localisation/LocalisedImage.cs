using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Localisation
{
	[RequireComponent(typeof(Image))]
	public class LocalisedImage : LocalisedObject
	{
		public override LocalisedResourceType ResourceType { get { return LocalisedResourceType.Image; } }

		private Image image;

		protected override void Awake()
		{
			base.Awake();
			image = GetComponent<Image>();
		}

		protected override void OnLocaleChanged()
		{
			if (image == null) return;

			string newPath;
			if (Localiser.GetLocalisedString(localisationKey, out newPath))
			{
				var newSprite = Resources.Load<Sprite>(newPath);
				if (newSprite != null)
				{
					image.sprite = newSprite;
				}
			}
			else
			{
				image.sprite = null;
			}
		}
	}
}