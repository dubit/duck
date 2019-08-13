using System;
using System.Collections.Generic;
using DUCK.Localisation.Editor.Data;
using DUCK.Localisation.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	[CustomEditor(typeof(LocalisationTable))]
	public class LocalisationTableEditor : UnityEditor.Editor
	{
		private SerializedProperty locales;
		private SerializedProperty localisationKeys;
		private SerializedProperty localisationValues;
		private SerializedProperty crcEncodingVersion;

		private LocalisationTable targetTable;
		private bool initialised;
		private bool showCRC;
		private bool emptyValuesOnly;
		private bool[] contentToggles = new bool[10];
		private readonly Dictionary<int, int> keyMap = new Dictionary<int, int>();

		private void OnEnable()
		{
			locales = serializedObject.FindProperty("locales");
			localisationKeys = serializedObject.FindProperty("keys");
			localisationValues = serializedObject.FindProperty("values");
			crcEncodingVersion = serializedObject.FindProperty("crcEncodingVersion");
			targetTable = (LocalisationTable)target;

			initialised = false;
		}

		public static void ExportEmptyValues(LocalisationTable locTable, LocalisationKeySchema currentSchema)
		{
			if (locTable == null || currentSchema == null) return;

			var path = GuiUtils.SaveCsvFileDialog($"{locTable.name}-0.csv");
			if (!string.IsNullOrEmpty(path))
			{
				Exporter.ExportTableToCsv(path, locTable, currentSchema, true);
			}
		}

		public static int FindEmptyValues(LocalisationTable locTable, LocalisationKeySchema currentSchema,
			bool fixMissing = false)
		{
			var emptyValues = 0;

			foreach (var category in currentSchema.categories)
			{
				if (category == null || category.keys == null)
				{
					continue;
				}

				foreach (var key in category.keys)
				{
					var locKeyCRC = CrcUtils.GetCrc(category.name, key);
					var locValue = string.Empty;

					try
					{
						locValue = locTable.GetString(locKeyCRC);
					}
					catch
					{
						if (fixMissing)
						{
							locTable.SetData(locKeyCRC, string.Empty);
						}
					}

					if (string.IsNullOrEmpty(locValue))
					{
						emptyValues++;
					}
				}
			}

			return emptyValues;
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label("Locales (languages) supported", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(locales, true);
			EditorGUI.indentLevel--;

			GUILayout.Label("Save / load", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			var currentSchema = LocalisationSettings.Current.Schema;
			if (currentSchema == null)
			{
				EditorGUILayout.HelpBox("Please populate Localisation Key Schema (or create a new one). Menu: Duck/Localisation",
					MessageType.Warning);
			}
			else
			{
				if (!initialised)
				{
					FindEmptyValues(target as LocalisationTable, currentSchema, true);
					serializedObject.Update();
					initialised = true;
				}

				if (GUILayout.Button("Load from CSV"))
				{
					LoadFromFile(target as LocalisationTable, currentSchema, emptyValuesOnly);
					serializedObject.Update();
					return;
				}

				if (GUILayout.Button("Save to CSV"))
				{
					var path = GuiUtils.SaveCsvFileDialog($"{targetTable.name}.csv");
					if (!string.IsNullOrEmpty(path))
					{
						Exporter.ExportTableToCsv(path, targetTable, currentSchema, emptyValuesOnly);
					}

					return;
				}

				if (emptyValuesOnly)
				{
					EditorGUILayout.HelpBox(
						"When loading, only keys which are empty in the localisation table will be overwritten. When Saving, only empty keys will be output.",
						MessageType.Warning);
				}
				emptyValuesOnly = EditorGUILayout.Toggle("Empty values only", emptyValuesOnly);

				DrawLocalisationContents(currentSchema);
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
			{
				AssetDatabase.SaveAssets();
			}
		}

		private void DrawLocalisationContents(LocalisationKeySchema currentSchema)
		{
			GUILayout.Label("Localisation texts", EditorStyles.boldLabel);

			showCRC = EditorGUILayout.Toggle("Show CRC values", showCRC);
			EditorGUILayout.LabelField("CRC version: ", crcEncodingVersion.intValue.ToString(), EditorStyles.helpBox);

			if (currentSchema.categories.Length > contentToggles.Length)
			{
				contentToggles = new bool[currentSchema.categories.Length];
			}

			keyMap.Clear();
			for (var i = 0; i < localisationKeys.arraySize; i++)
			{
				// mapping of locKey to its index in the keysArray (i.e. Array.IndexOf lookup-table)
				keyMap.Add(localisationKeys.GetArrayElementAtIndex(i).intValue, i);
			}

			var updateKeyEncoding = (crcEncodingVersion.intValue >= 0 &&
			                         crcEncodingVersion.intValue != CrcUtils.KEY_CRC_ENCODING_VERSION);

			for (var i = 0; i < currentSchema.categories.Length; i++)
			{
				var category = currentSchema.categories[i];

				contentToggles[i] = EditorGUILayout.Foldout(contentToggles[i], category.name);
				if (contentToggles[i] || updateKeyEncoding)
				{
					EditorGUI.indentLevel++;
					for (var j = 0; j < category.keys.Length; j++)
					{
						var locKeyCRC =
							CrcUtils.GetCrcWithEncodingVersion(category.name, category.keys[j], crcEncodingVersion.intValue);

						if (contentToggles[i])
						{
							if (!showCRC)
							{
								EditorGUILayout.BeginHorizontal();
							}
							EditorGUILayout.LabelField(
								string.Format("{0} {1}", category.keys[j], showCRC ? "(" + locKeyCRC + ")" : string.Empty),
								EditorStyles.boldLabel, GUILayout.MaxWidth(showCRC ? 200f : 140f));
						}

						var currentValue = string.Empty;
						SerializedProperty currentValueProperty = null;

						if (keyMap.ContainsKey(locKeyCRC))
						{
							var currentArrayIndex = keyMap[locKeyCRC];

							if (updateKeyEncoding)
							{
								keyMap.Remove(locKeyCRC);
								locKeyCRC = CrcUtils.GetCrc(category.name, category.keys[j]);
								localisationKeys.GetArrayElementAtIndex(currentArrayIndex).intValue = locKeyCRC;
								keyMap.Add(locKeyCRC, currentArrayIndex);
							}

							currentValueProperty = localisationValues.GetArrayElementAtIndex(currentArrayIndex);
							currentValue = currentValueProperty.stringValue;
						}

						if (contentToggles[i])
						{
							var newValue = EditorGUILayout.DelayedTextField(currentValue);
							if (!showCRC)
							{
								EditorGUILayout.EndHorizontal();
							}

							if (newValue != currentValue)
							{
								if (currentValueProperty != null)
								{
									currentValueProperty.stringValue = newValue;
								}
								else
								{
									localisationKeys.arraySize++;
									localisationValues.arraySize++;
									localisationKeys.GetArrayElementAtIndex(localisationKeys.arraySize - 1).intValue = locKeyCRC;
									localisationValues.GetArrayElementAtIndex(localisationValues.arraySize - 1).stringValue = newValue;
								}
							}
						}
					}

					EditorGUI.indentLevel--;
				}
			}

			if (updateKeyEncoding)
			{
				crcEncodingVersion.intValue = CrcUtils.KEY_CRC_ENCODING_VERSION;
				Debug.Log(string.Format("Localisation table '{0}' updated key CRC encoding to latest version: {1}",
					targetTable.name, crcEncodingVersion.intValue));
			}

			serializedObject.ApplyModifiedProperties();
		}

		/*
		 * FORMAT:
		 * Category,Key,Content
		 * Category,Key,"Complex, Content"
		 */
		private void LoadFromFile(
			LocalisationTable locTable,
			LocalisationKeySchema currentSchema,
			bool emptyValuesOnly = false,
			bool saveAssets = true)
		{
			if (locTable == null) throw new ArgumentNullException(nameof(locTable));
			if (currentSchema == null) throw new ArgumentNullException(nameof(currentSchema));

			var path = GuiUtils.OpenCsvFileDialog();
			if (!string.IsNullOrEmpty(path))
			{
				Importer.ImportFromCsv(path, targetTable, currentSchema, emptyValuesOnly, saveAssets);
			}
		}
	}
}