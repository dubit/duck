using UnityEngine;

namespace DUCK.Localisation.LocalisedObjects
{
	/// <summary>
	/// Automatically updates an AudioSource's clip to a different file path specified in the localisation table for the current locale
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class LocalisedAudioSource : AbstractLocalisedObject<AudioSource>
	{
		protected override void HandleLocaleChanged(bool translationFound, string localisedString)
		{
			if (translationFound)
			{
				var newAudio = Resources.Load<AudioClip>(localisedString);

				if (newAudio != null)
				{
					Component.clip = newAudio;
				}
			}
			else
			{
				Component.clip = null;
			}
		}
	}
}