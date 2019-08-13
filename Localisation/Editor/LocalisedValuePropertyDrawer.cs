using System;
using System.Linq;
using DUCK.Localisation.LocalisedObjects;
using DUCK.Localisation.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	[CustomPropertyDrawer(typeof(LocalisedValue))]
	public class LocalisedValuePropertyDrawer: PropertyDrawer
	{
		private const float LINE_HEIGHT = 18;
		private const float INDENTATION = 10;

		bool isInitialized = false;
		Vector2Int selectedKeyAndCategory;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return LINE_HEIGHT * 3f;
		}

		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(rect, label, property);

			// Draw label
			EditorGUI.LabelField(rect, label);

			// Prepare data
			var currentSchema = LocalisationSettings.Current.Schema;
			if (currentSchema == null)
			{
				EditorGUILayout.HelpBox("Please populate Localisation Key Schema (or create a new one). Menu: DUCK/Localisation",
					MessageType.Warning);
			}
			else
			{
				var keyName = property.FindPropertyRelative("keyName");
				var categoryName = property.FindPropertyRelative("categoryName");
				var localisationKey = property.FindPropertyRelative("localisationKey");

				var keyLookup = currentSchema.FindKey(
					categoryName.stringValue,
					keyName.stringValue);

				if (!isInitialized)
				{
					selectedKeyAndCategory = keyLookup.IsValid ?
						new Vector2Int
						{
							x = Math.Max(0, keyLookup.keyIndex),
							y = Math.Max(0, keyLookup.categoryIndex),
						}
						: Vector2Int.zero;

					isInitialized = true;
				}

				// CATEGORIES
				var categories = currentSchema.categories;
				var categoryNames = categories.Select(c => c.name).ToArray();
				var categoryIndex = selectedKeyAndCategory.y;
				categoryIndex = categoryIndex < 0 || categoryIndex >= categoryNames.Length ?
					0 : selectedKeyAndCategory.y;

				var categoryRect = new Rect(rect)
				{
					x = rect.x + INDENTATION,
					y = rect.y + LINE_HEIGHT,
					height = LINE_HEIGHT,
					width = rect.width - INDENTATION,
				};
				categoryIndex = EditorGUI.Popup(categoryRect, "Category", categoryIndex, categoryNames);
				selectedKeyAndCategory.y = categoryIndex;

				var category = currentSchema.categories[categoryIndex];

				// KEYS
				var locKeyIndex = selectedKeyAndCategory.x;
				if (locKeyIndex < 0 || locKeyIndex >= category.keys.Length)
				{
					locKeyIndex = 0;
				}
				var keysRect = new Rect(rect)
				{
					x = rect.x + INDENTATION,
					y = rect.y + LINE_HEIGHT * 2,
					height = LINE_HEIGHT,
					width = rect.width - INDENTATION
				};
				locKeyIndex = EditorGUI.Popup(keysRect, "Key", locKeyIndex,  category.keys);
				selectedKeyAndCategory.x = locKeyIndex;

				var selectedLocKey = category.keys[locKeyIndex];
				var savedLocKey = keyName.stringValue;

				var crc = CrcUtils.GetCrc(
					category.name,
					selectedLocKey);

				if (localisationKey.intValue != crc)
				{
					keyName.stringValue = selectedLocKey;
					categoryName.stringValue = category.name;

					localisationKey.intValue = CrcUtils.GetCrc(
						categoryName.stringValue,
						keyName.stringValue);

					savedLocKey = selectedLocKey;
				}

			
			}

			EditorGUI.EndProperty();
		}
	}
}