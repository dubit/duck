using UnityEngine;

namespace DUCK.Localisation
{
	/// <summary>
	/// LocalisedObject family of components: attach one of these to a Text, Image or AudioSource to make them localisation-aware.
	/// 
	/// When the language is changed, they will automatically try to update their content to the localised version.
	/// </summary>
	public abstract class LocalisedObject : MonoBehaviour, ILocalisedObject
	{
		public enum LocalisedResourceType
		{
			Text,
			Image,
			Audio
		}

		public abstract LocalisedResourceType ResourceType
		{
			get;
		}

		[SerializeField]
		protected int localisationKey;

		// Only used by Editor
		[SerializeField]
		protected string keyName;

		[SerializeField]
		protected string categoryName;

		protected abstract void OnLocaleChanged();

		protected virtual void Awake()
		{
			Localiser.OnLocaleChanged += OnLocaleChanged;
		}

		protected void OnDestroy()
		{
			Localiser.OnLocaleChanged -= OnLocaleChanged;
		}
	}
}
