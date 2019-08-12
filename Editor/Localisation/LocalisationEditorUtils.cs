using System.Collections.Generic;
using UnityEditor;

namespace DUCK.Localisation.Editor
{
	public class LocalisationEditorUtils
	{
		/// <summary>
		/// Util: array of names built from SupportedLocales array, and a reverse-lookup function for finding one
		/// </summary>
		private static string[] supportedLocaleNames;

		private static string[] SupportedLocaleNames
		{
			get
			{
				if (supportedLocaleNames == null)
				{
					var supportedLocales = Localiser.SupportedLocales;
					supportedLocaleNames = new string[supportedLocales.Length];
					for (var i = 0; i < supportedLocales.Length; i++)
					{
						supportedLocaleNames[i] = supportedLocales[i].Name;
					}
				}

				return supportedLocaleNames;
			}
		}

		public static int SupportedLocaleIndex(string localeName)
		{
			var index = -1;
			for (var i = 0; i < SupportedLocaleNames.Length; i++)
			{
				if (localeName == SupportedLocaleNames[i])
				{
					index = i;
					break;
				}
			}
			return index;
		}

		public static void DrawLocalesProperty(SerializedProperty locales)
		{
			locales.arraySize = EditorGUILayout.IntField("Size", locales.arraySize);

			var supportedLocaleNames = SupportedLocaleNames;

			for (var i = 0; i < locales.arraySize; i++)
			{
				var currentValue = locales.GetArrayElementAtIndex(i).stringValue;
				var currentIndex = SupportedLocaleIndex(currentValue);

				if (currentIndex < 0)
				{
					currentIndex = 0;
				}

				var newIndex = EditorGUILayout.Popup(currentIndex, supportedLocaleNames);
				locales.GetArrayElementAtIndex(i).stringValue = supportedLocaleNames[newIndex];
			}

			if (locales.arraySize == 0)
			{
				EditorGUILayout.HelpBox("A table with no Locales will never be used as it doesn't relate to any language(s)",
					MessageType.Warning);
			}
		}

		/// <summary>
		/// Maintains a dictionary of categories by resource type, allowing calling code to get all key categories for a certain kind of content.
		/// </summary>
		private static readonly Dictionary<LocalisedObject.LocalisedResourceType, List<int>> categoriesByResourceType =
			new Dictionary<LocalisedObject.LocalisedResourceType, List<int>>();

		public static int[] GetAvailableCategories(LocalisedObject.LocalisedResourceType resourceType)
		{
			return categoriesByResourceType.ContainsKey(resourceType)
				? categoriesByResourceType[resourceType].ToArray()
				: new int[] { };
		}

		public static void RefreshCategories()
		{
			var schema = LocalisationEditor.CurrentSchema;
			if (schema == null)
			{
				return;
			}

			categoriesByResourceType.Clear();
			foreach (var resourceType in (LocalisedObject.LocalisedResourceType[])System.Enum.GetValues(
				typeof(LocalisedObject.LocalisedResourceType)))
			{
				categoriesByResourceType[resourceType] = new List<int>();
			}

			for (var i = 0; i < schema.categories.Length; i++)
			{
				var category = schema.categories[i];
				categoriesByResourceType[category.type].Add(i);
			}
		}
	}
}