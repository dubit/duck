using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	[CustomEditor(typeof(LocalisationSettings))]
	public class LocalisationSettingsEditor : UnityEditor.Editor
	{
		private SerializedProperty codeGenerationFilePathProperty;
		private SerializedProperty localisationTableFolderProperty;
		private SerializedProperty schemaProperty;
		private SerializedProperty categories;

		private bool[] contentToggles = new bool[0];
		private readonly Dictionary<string, int> duplicateCategories = new Dictionary<string, int>();
		private readonly Dictionary<string, int> duplicateKeys = new Dictionary<string, int>();

		private void OnEnable()
		{
			codeGenerationFilePathProperty = serializedObject.FindProperty("codeGenerationFilePath");
			localisationTableFolderProperty = serializedObject.FindProperty("localisationTableFolder");
			schemaProperty = serializedObject.FindProperty("schema");
			categories = schemaProperty.FindPropertyRelative("categories");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label("Paths", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(codeGenerationFilePathProperty);
			EditorGUILayout.PropertyField(localisationTableFolderProperty);

			EditorGUILayout.Space();
			GUILayout.Label("Schema", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(
				"Define your localisation keys by category.",
				MessageType.Info);

			var arraySize = categories.arraySize;
			if (contentToggles.Length < arraySize)
			{
				contentToggles = new bool[arraySize];
			}

			duplicateCategories.Clear();

			var blankNames = false;

			var schema = (target as LocalisationSettings).Schema;
			for (var i = 0; i < arraySize; i++)
			{
				if (schema != null && schema.categories[i] == null)
				{
					schema.categories[i] = new LocalisationKeySchema.LocalisationKeyCategory();
					serializedObject.Update();
				}

				var categoryProperty = categories.GetArrayElementAtIndex(i);

				var nameProperty = categoryProperty.FindPropertyRelative("name");
				var keysProperty = categoryProperty.FindPropertyRelative("keys");

				var labelContent = $"{nameProperty.stringValue} ({keysProperty.arraySize.ToString()})";

				if (!duplicateCategories.ContainsKey(nameProperty.stringValue))
				{
					duplicateCategories[nameProperty.stringValue] = 0;
				}
				duplicateCategories[nameProperty.stringValue]++;

				if (string.IsNullOrEmpty(nameProperty.stringValue))
				{
					blankNames = true;
				}

				EditorGUI.indentLevel++;
				contentToggles[i] = EditorGUILayout.Foldout(contentToggles[i], labelContent);
				if (contentToggles[i])
				{
					if (DrawCategory(nameProperty, keysProperty, i))
					{
						EditorGUI.indentLevel--;
						break;
					}

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			var enumerator = duplicateCategories.GetEnumerator();
			for (var e = enumerator; enumerator.MoveNext();)
			{
				if (e.Current.Value > 1)
				{
					EditorGUILayout.HelpBox(
						string.Format("The name '{0}' belongs to {1} categories - they must be unique!", e.Current.Key, e.Current.Value),
						MessageType.Error);
				}
			}
			enumerator.Dispose();

			if (blankNames)
			{
				EditorGUILayout.HelpBox("Categories without a name exist - please populate them.", MessageType.Error);
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format("Count: {0}", categories.arraySize));
			if (GUILayout.Button("Add Category"))
			{
				categories.InsertArrayElementAtIndex(categories.arraySize);

				var newCategory = categories.GetArrayElementAtIndex(categories.arraySize - 1);
				newCategory.FindPropertyRelative("name").stringValue = "NewCategory";
				newCategory.FindPropertyRelative("keys").arraySize = 0;
			}
			EditorGUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

		private bool DrawCategory(SerializedProperty nameProperty, SerializedProperty keysProperty, int index)
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal();
			var newName = EditorGUILayout.TextField("Name", nameProperty.stringValue);
			newName = newName.Trim();
			newName = Regex.Replace(newName, @"[^a-zA-Z0-9\-]", string.Empty);
			newName = Regex.Replace(newName, @"^[\d-]*\s*", string.Empty);
			if (nameProperty.stringValue != newName)
			{
				nameProperty.stringValue = newName;
			}

			var buttonStyle = new GUIStyle(GUI.skin.button) {normal = {textColor = Color.red}};
			if (GUILayout.Button("DELETE", buttonStyle))
			{
				categories.DeleteArrayElementAtIndex(index);
				return true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel++;

			duplicateKeys.Clear();
			var blankKeys = false;

			for (var j = 0; j < keysProperty.arraySize; j++)
			{
				var keyProperty = keysProperty.GetArrayElementAtIndex(j);

				GUILayoutOption[] options = {GUILayout.MinWidth(180.0f)};
				EditorGUILayout.BeginHorizontal();

				var newKey = EditorGUILayout.TextField(string.Empty, keyProperty.stringValue, options);
				newKey = newKey.Trim();
				newKey = Regex.Replace(newKey, @"[^a-zA-Z0-9\-]", string.Empty);
				newKey = Regex.Replace(newKey, @"^[\d-]*\s*", string.Empty);
				if (keyProperty.stringValue != newKey)
				{
					keyProperty.stringValue = newKey;
				}

				if (!duplicateKeys.ContainsKey(keyProperty.stringValue))
				{
					duplicateKeys[keyProperty.stringValue] = 0;
					EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(80f));
				}
				else
				{
					var labelStyle = new GUIStyle(EditorStyles.boldLabel) {normal = {textColor = Color.red}};
					EditorGUILayout.LabelField("✕", labelStyle, GUILayout.MaxWidth(80f));
				}
				duplicateKeys[keyProperty.stringValue]++;

				if (string.IsNullOrEmpty(keyProperty.stringValue))
				{
					blankKeys = true;
				}

				if (GUILayout.Button("  X  ", GUILayout.MaxWidth(40f)))
				{
					keysProperty.DeleteArrayElementAtIndex(j);
					EditorGUILayout.EndHorizontal();
					break;
				}

				EditorGUILayout.EndHorizontal();

				var enumerator = duplicateKeys.GetEnumerator();
				for (var e = enumerator; enumerator.MoveNext();)
				{
					if (e.Current.Value > 1)
					{
						EditorGUILayout.HelpBox(
							string.Format("The key '{0}' exists {1} times in this category - they must be unique!", e.Current.Key,
								e.Current.Value), MessageType.Error);
					}
				}
				enumerator.Dispose();

				if (blankKeys)
				{
					EditorGUILayout.HelpBox("Blank keys exist - please populate them.", MessageType.Error);
				}
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format("Count: {0}", keysProperty.arraySize));
			if (GUILayout.Button("  +  ", GUILayout.MaxWidth(40f)))
			{
				keysProperty.arraySize++;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			return false;
		}
	}
}