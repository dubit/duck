using System;
using UnityEngine;

namespace DUCK.Localisation.LocalisedObjects
{
	public abstract class AbstractLocalisedObject : MonoBehaviour
	{
		[SerializeField]
		protected LocalisedValue localisedValue;

		[SerializeField]
		protected string[] formatParameters;
	}

	/// <summary>
	/// LocalisedObject family of components: attach one of these to a Text, Image or AudioSource to make them localisation-aware.
	/// When the language is changed, they will automatically try to update their content to the localised version.
	/// </summary>
	public abstract class AbstractLocalisedObject<TComponent> : AbstractLocalisedObject
		where TComponent : Component
	{
		protected TComponent Component { get; private set; }

		protected abstract void HandleLocaleChanged(bool translationFound, string localisedString);

		protected void Awake()
		{
			Component = GetComponent<TComponent>();

			Localiser.OnLocaleChanged += OnLocaleChanged;

			OnLocaleChanged();
		}

		protected void OnDestroy()
		{
			Localiser.OnLocaleChanged -= OnLocaleChanged;
		}

		private void OnLocaleChanged()
		{
			if (Localiser.Initialised)
			{
				var localisedText = "";
				var foundtranslation = Localiser.GetLocalisedString(localisedValue.LocalisationKey, out localisedText);
				if (foundtranslation && formatParameters != null && formatParameters.Length > 0)
				{
					try
					{
						localisedText = string.Format(localisedText, formatParameters);
					}
					catch (FormatException e)
					{
						Debug.LogError($"FormatException thrown by {name}: {e.Message}", this);
					}
				}
				HandleLocaleChanged(foundtranslation, localisedText);
			}
			else
			{
				Debug.LogWarning("Cannot localise, the localiser is not initialised!");
			}
		}

#if ODIN_INSPECTOR && UNITY_EDITOR
		[Sirenix.OdinInspector.Button]
		private void Test(string locale = "en-GB", string tablesPaths = "")
		{
			var localisationTables = Resources.LoadAll<LocalisationTable>(tablesPaths);
			var table = System.Linq.Enumerable.FirstOrDefault(localisationTables, t => t.SupportedLocales.Contains(locale));
			if (table == null)
			{
				throw new System.Exception($"No table found that supports the locale {locale}");
			}
			var value = table.GetString(localisedValue.LocalisationKey);

			Component = GetComponent<TComponent>();

			var shouldFormat = formatParameters != null && formatParameters.Length > 0;
			value = shouldFormat ? string.Format(value, formatParameters) : value;
			HandleLocaleChanged(true, value);
		}
#endif
	}
}
