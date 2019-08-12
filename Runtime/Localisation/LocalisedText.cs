using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Localisation
{
	[RequireComponent(typeof(Text))]
	public class LocalisedText : LocalisedObject
	{
		public override LocalisedResourceType ResourceType { get { return LocalisedResourceType.Text; } }

		private Text text;

		protected override void Awake()
		{
			base.Awake();
			text = GetComponent<Text>();

			OnLocaleChanged();
		}

		protected override void OnLocaleChanged()
		{
			if (text == null) return;

			string newText;
			text.text = Localiser.GetLocalisedString(localisationKey, out newText)
				? newText
				: string.Format("<color=red>{0}</color>", localisationKey);
		}
	}
}