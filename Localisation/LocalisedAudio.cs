using UnityEngine;

namespace DUCK.Localisation
{
	[RequireComponent(typeof(AudioSource))]
	public class LocalisedAudio : LocalisedObject
	{
		public override LocalisedResourceType ResourceType { get { return LocalisedResourceType.Audio; } }

		private AudioSource source;

		protected override void Awake()
		{
			base.Awake();
			source = GetComponent<AudioSource>();
		}

		protected override void OnLocaleChanged()
		{
			if (source == null) return;

			string newPath;
			if (Localiser.GetLocalisedString(localisationKey, out newPath))
			{
				var newAudio = Resources.Load<AudioClip>(newPath);
				if (newAudio != null)
				{
					source.clip = newAudio;
				}
			}
			else
			{
				source.clip = null;
			}
		}
	}
}