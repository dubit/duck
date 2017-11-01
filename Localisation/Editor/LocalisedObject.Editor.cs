using System;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	[CustomEditor(typeof(LocalisedObject), true)]
	public class LocalisedObjectEditor : UnityEditor.Editor
	{
		private SerializedProperty localisationKey;
		private SerializedProperty keyName;
		private SerializedProperty categoryName;

		private int selectedCategoryIndex;
		private int selectedKeyIndex;
		private bool initialised;

		private void Reset()
		{
			initialised = false;
			Repaint();
		}

		private void OnEnable()
		{
			localisationKey = serializedObject.FindProperty("localisationKey");
			keyName = serializedObject.FindProperty("keyName");
			categoryName = serializedObject.FindProperty("categoryName");
		}

		public override void OnInspectorGUI()
		{
			var currentSchema = LocalisationEditor.CurrentSchema;

			if (currentSchema == null)
			{
				EditorGUILayout.HelpBox("Please populate Localisation Key Schema (or create a new one). Menu: Dubit/Localisation",
					MessageType.Warning);
			}
			else
			{
				var keyLookup = currentSchema.FindKey(categoryName.stringValue, keyName.stringValue);

				if (!initialised)
				{
					if (keyLookup.IsValid)
					{
						selectedCategoryIndex = Math.Max(0, keyLookup.categoryIndex);
						selectedKeyIndex = Math.Max(0, keyLookup.keyIndex);
					}
					else
					{
						selectedCategoryIndex = 0;
						selectedKeyIndex = 0;
					}

					initialised = true;
				}

				var resourceType = (target as LocalisedObject).ResourceType;
				var availableCategories = LocalisationEditorUtils.GetAvailableCategories(resourceType);
				var categoryNames = new string[availableCategories.Length];
				for (var i = 0; i < categoryNames.Length; i++)
				{
					if (availableCategories[i] > currentSchema.categories.Length)
					{
						categoryNames[i] = "< REMOVED? >";
					}
					else
					{
						categoryNames[i] = currentSchema.categories[availableCategories[i]].name;
					}
				}

				var categoryIndex = selectedCategoryIndex;
				if (categoryIndex < 0 || categoryIndex >= availableCategories.Length)
				{
					categoryIndex = 0;
				}
				categoryIndex = EditorGUILayout.Popup(categoryIndex, categoryNames);
				selectedCategoryIndex = categoryIndex;

				var category = currentSchema.categories[availableCategories[categoryIndex]];
				var locKeyIndex = selectedKeyIndex;
				if (locKeyIndex < 0 || locKeyIndex >= category.keys.Length)
				{
					locKeyIndex = 0;
				}
				locKeyIndex = EditorGUILayout.Popup(locKeyIndex, category.keys);
				selectedKeyIndex = locKeyIndex;

				var selectedLocKey = category.keys[locKeyIndex];
				var savedLocKey = keyName.stringValue;

				if (savedLocKey != selectedLocKey)
				{
					if (GUILayout.Button("SET"))
					{
						keyName.stringValue = selectedLocKey;
						categoryName.stringValue = category.name;

						localisationKey.intValue = LocalisationEditor.GetCRC(categoryName.stringValue, keyName.stringValue);

						savedLocKey = selectedLocKey;
					}
				}

				if (string.IsNullOrEmpty(savedLocKey))
				{
					GUILayout.Label("< NONE >", EditorStyles.boldLabel);
				}
				else
				{
					var style = new GUIStyle(EditorStyles.boldLabel);

					if (!keyLookup.IsValid)
					{
						EditorGUILayout.HelpBox(
							string.Format("Key contents ({0}/{1}:{2}) not found in the schema - please set it to a new value",
								categoryName.stringValue, keyName.stringValue, localisationKey.intValue), MessageType.Error);

						style.normal.textColor = Color.red;
					}

					GUILayout.Label(string.Format("{0}/{1} ({2}) {3}",
						categoryName.stringValue,
						keyName.stringValue,
						localisationKey.intValue,
						savedLocKey != selectedLocKey ? "*" : string.Empty
					), style);
				}

				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}