using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Localisation
{
	/// <summary>
	/// Localisation manager class which maintains access to the localisation tables, loads them on-demand when the language is changed,
	/// and returns localised content in the correct language.
	/// </summary>
	public static class Localiser
	{
		public const string DEFAULT_LOCALE = "en-US";

		public static readonly CultureInfo[] SupportedLocales =
		{
			new CultureInfo("en-GB"),
			new CultureInfo("en-US"),
			new CultureInfo("fr"),
			new CultureInfo("it"),
			new CultureInfo("de"),
			new CultureInfo("es"),
			new CultureInfo("pt-BR")
		};

		public static CultureInfo CurrentLocale { get; private set; }
		public static bool Initialised { get { return currentLocalisationTable != null; } }

		private static readonly Dictionary<string, string> tablePathsByLocale = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> allTablePaths = new Dictionary<string, string>();

		private static LocalisationTable currentLocalisationTable;

		public static event Action OnLocaleChanged;

		/// <summary>
		/// Initialises the localisation system in a folder in Resources/ where the localisation table assets are found.
		/// </summary>
		/// <param name="path"></param>
		public static bool Initialise(string path)
		{
			if (CurrentLocale == null)
			{
#if UNITY_WSA
				CurrentLocale = CultureInfo.CurrentCulture;
#else
				CurrentLocale = CultureInfo.InstalledUICulture;
#endif
			}

			tablePathsByLocale.Clear();
			allTablePaths.Clear();

			var localisationTables = Resources.LoadAll<LocalisationTable>(path);

			foreach (var locTable in localisationTables)
			{
				var useThisTable = false;
				var fullPath = Path.Combine(path, locTable.name);

				allTablePaths.Add(locTable.name, fullPath);

				if (locTable.SupportedLocales.Count == 0)
				{
					Debug.LogError(string.Format(
						"Did not initialise localisation table '{0}' as it has no supported locales so would never be used.",
						locTable.name));
					continue;
				}

				foreach (var localeName in locTable.SupportedLocales)
				{
					if (tablePathsByLocale.ContainsKey(localeName))
					{
						Debug.LogWarning(string.Format("Localisation table contains locale '{0}' which has already been added!",
							localeName));
					}
					else
					{
						tablePathsByLocale.Add(localeName, fullPath);

						if (localeName == CurrentLocale.Name && Application.isPlaying)
						{
							useThisTable = true;
							SwitchLanguage(localeName);
						}
					}
				}

				if (!useThisTable)
				{
					Resources.UnloadAsset(locTable);
				}
			}

			if (CurrentLocale != null && Application.isPlaying)
			{
				Debug.Log(string.Format("Localiser initialised in {0}, current locale is: {1} ({2})", path, CurrentLocale.Name,
					CurrentLocale.NativeName));
				if (currentLocalisationTable == null)
				{
					Debug.LogError(string.Format("Error: unsupported locale: {0}", CurrentLocale.Name));
					SwitchLanguage(DEFAULT_LOCALE);
				}
			}

			return Initialised;
		}

		public static void SwitchLanguage(string localeName)
		{
			CurrentLocale = new CultureInfo(localeName);
			currentLocalisationTable = LoadLocalisationTable(localeName);

			if (currentLocalisationTable == null)
			{
				Debug.LogError(string.Format("Error loading localisation table for locale: {0}", localeName));
			}

			OnLocaleChanged.SafeInvoke();
		}

		public static bool GetLocalisedString(int key, out string target)
		{
			if (!Initialised)
			{
				throw new Exception("Localiser not initialised");
			}

			try
			{
				target = currentLocalisationTable.GetString(key);
			}
			catch (KeyNotFoundException)
			{
				target = string.Empty;
				return false;
			}

			return true;
		}

		public static Dictionary<string, string> GetTablePaths()
		{
			return allTablePaths;
		}

		private static LocalisationTable LoadLocalisationTable(string localeName)
		{
			if (tablePathsByLocale.ContainsKey(localeName))
			{
				var loadPath = tablePathsByLocale[localeName];
				return Resources.Load<LocalisationTable>(loadPath);
			}

			return null;
		}
	}
}