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

		private static string defaultCulture = "en-US";
		private static LocalisationTable currentLocalisationTable;

		public static event Action OnLocaleChanged;

		/// <summary>
		/// Initialises the localisation system in a folder in Resources/ where the localisation table assets are found.
		/// </summary>
		/// <param name="path">Filepath to the folder of localisation tables</param>
		/// <param name="cultureName">Optional culture name to initialise with. Falls back to default culture if not passed in or invalid</param>
		public static bool Initialise(string path, string cultureName = null)
		{
			CurrentLocale = TryGetCultureInfo(cultureName);

			tablePathsByLocale.Clear();
			allTablePaths.Clear();

			var localisationTables = Resources.LoadAll<LocalisationTable>(path);

			if (localisationTables.Length == 0)
			{
				Debug.LogError(string.Format("No localisation tables found at path: {0}", path));
			}

			foreach (var locTable in localisationTables)
			{
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
							currentLocalisationTable = locTable;
						}
						else if (!locTable.SupportedLocales.Contains(CurrentLocale.Name))
						{
							Resources.UnloadAsset(locTable);
						}
					}
				}
			}

			if (currentLocalisationTable == null && Application.isPlaying)
			{
				Debug.LogError(string.Format("Error: unsupported locale: {0}, switching to {1}", CurrentLocale.Name, defaultCulture));
				SwitchLanguage(defaultCulture);
			}

			return Initialised;
		}

		public static void SetDefaultCulture(string defaultCultureName)
		{
			if (TryGetCultureInfo(defaultCultureName).Name == defaultCultureName)
			{
				defaultCulture = defaultCultureName;
			}
			else
			{
				Debug.LogError("Invalid culture name: " + defaultCultureName);
			}
		}

		public static void SwitchLanguage(string cultureName)
		{
			var loadedTable = LoadLocalisationTable(cultureName);
			if (loadedTable == null)
			{
				Debug.LogError(string.Format("Error loading localisation table for locale: {0}", cultureName));
				return;
			}

			CurrentLocale = new CultureInfo(cultureName);
			currentLocalisationTable = loadedTable;

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

		/// <returns>Returns the correct CultureInfo if name is valid, otherwise returns CultureInfo(defaultCulture)</returns>
		private static CultureInfo TryGetCultureInfo(string cultureName)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				cultureName = defaultCulture;
			}

			CultureInfo cultureInfo = null;

			try
			{
				cultureInfo = new CultureInfo(cultureName);
			}
			catch (ArgumentException)
			{
				Debug.LogError("Invalid culture name: " + cultureName);
				cultureInfo = new CultureInfo(defaultCulture);
			}

			return cultureInfo;
		}

		private static LocalisationTable LoadLocalisationTable(string localeName)
		{
			if (!tablePathsByLocale.ContainsKey(localeName)) return null;

			var loadPath = tablePathsByLocale[localeName];
			return Resources.Load<LocalisationTable>(loadPath);
		}
	}
}