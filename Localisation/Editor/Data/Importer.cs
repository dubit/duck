using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor.Data
{
	public static class Importer
	{
		/// <summary>
		/// Import from csv file, in the format (Category,Key,Content)
		/// </summary>
		/// <param name="path">The path to the csv file to import from</param>
		/// <param name="locTable">The localisation table to import to</param>
		/// <param name="currentSchema">The current schema to import against</param>
		/// <param name="emptyValuesOnly"> If set to true, the import will only import values that are currently empty,
		/// and will not override existing values, defaults to false.
		/// </param>
		/// <param name="saveAssets">If set to true the LocalisationTable object will be saved to disk.
		/// Otherwise it will import and the changes will be in memory (and therefore require saving). defaults to true
		/// </param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void ImportFromCsv(
			string path,
			LocalisationTable locTable,
			LocalisationKeySchema currentSchema,
			bool emptyValuesOnly = false,
			bool saveAssets = true)
		{
			if (locTable == null) throw new ArgumentNullException(nameof(locTable));
			if (currentSchema == null) throw new ArgumentNullException(nameof(currentSchema));
			if (path == null) throw new ArgumentNullException(nameof(path));

			try
			{
				const char separator = ',';

				var newData = new Dictionary<int, string>();

				Func<string, string> cleanInput = input => input.Replace("\t", string.Empty);

				var lines = CsvParser.Parse(File.ReadAllText(path));
				foreach (var line in lines)
				{
					if (line.Count < 3) continue;

					var category = cleanInput(line[0]);
					var key = cleanInput(line[1]);
					var value = cleanInput(line[2]);

					var lookup = currentSchema.FindKey(category, key);
					if (!lookup.IsValid)
					{
						continue;
					}

					var keyCRC = CrcUtils.GetCrc(category, key);
					if (newData.ContainsKey(keyCRC))
					{
						continue;
					}

					newData.Add(keyCRC, value);
				}

				locTable.SetData(newData, emptyValuesOnly);
				EditorUtility.SetDirty(locTable);
				AssetDatabase.SaveAssets();

				Debug.Log($"Data populated from file: {path} {(emptyValuesOnly ? "(empty only)" : string.Empty)}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Could not load from CSV format: {e.Message}");
			}
		}

		public static void ImportSchema(string path, LocalisationSettings settings)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			var csvCategories = new List<string>();
			var csvKeys = new Dictionary<string, List<string>>();

			var settingsSerializedObject = new SerializedObject(settings);
			var schema = settingsSerializedObject.FindProperty("schema");
			var categories = schema.FindPropertyRelative("categories");

			try
			{
				var lines = CsvParser.Parse(File.ReadAllText(path));

				// Step 1: populate csvCategories, and csvKeys from the CSV
				foreach (var line in lines)
				{
					var categoryName = line[0];
					var keyName = line[1];

					if (!csvCategories.Contains(categoryName))
					{
						csvCategories.Add(categoryName);
					}

					if (!csvKeys.ContainsKey(categoryName))
					{
						csvKeys.Add(categoryName, new List<string>());
					}

					var keys = csvKeys[categoryName];

					if (!keys.Contains(keyName))
					{
						keys.Add(keyName);
					}
				}

				// Step 2: Loop through each category found in the csv.
				foreach (var newCategoryName in csvCategories)
				{
					// Step 2a: find the category serializedProperty that corresponds to the category in question
					SerializedProperty category = null;
					for (var i = 0; i < categories.arraySize; i++)
					{
						var existingCategory = categories.GetArrayElementAtIndex(i);
						if (existingCategory.FindPropertyRelative("name").stringValue == newCategoryName)
						{
							category = existingCategory;
							break;
						}
					}

					// Step 2b: if it was not found then it's new - let's add it
					if (category == null)
					{
						categories.InsertArrayElementAtIndex(categories.arraySize);
						category = categories.GetArrayElementAtIndex(categories.arraySize - 1);
						category.FindPropertyRelative("name").stringValue = newCategoryName;
						category.FindPropertyRelative("keys").ClearArray();

						Debug.Log("Add category: " + newCategoryName);
					}

					// Step 2c: Loop through each key in the category from the csv, adding any new ones
					var keys = category.FindPropertyRelative("keys");
					foreach (var newKeyName in csvKeys[newCategoryName])
					{
						// find out if the key exists
						var keyAlreadyExists = false;
						for (var i = 0; i < keys.arraySize; i++)
						{
							var key = keys.GetArrayElementAtIndex(i);
							if (key.stringValue == newKeyName)
							{
								keyAlreadyExists = true;
								break;
							}
						}

						// if it does not exist, then add it.
						if (!keyAlreadyExists)
						{
							keys.InsertArrayElementAtIndex(keys.arraySize);
							var newKey = keys.GetArrayElementAtIndex(keys.arraySize - 1);
							newKey.stringValue = newKeyName;

							Debug.Log("Add Key: " + newCategoryName + " : " + newKeyName);
						}
					}
				}

				// TODO: Remove all catetgories/keys that were not found in the csv (should be optional, requires GUI)

				settingsSerializedObject.ApplyModifiedProperties();
				settingsSerializedObject.Update();
			}
			catch (Exception e)
			{
				Debug.LogError($"Could not load from CSV format: {e.Message}");
			}
		}

		public static void ImportAll(string path, LocalisationSettings settings)
		{
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			var localisationTables = AssetDatabase.FindAssets("t:LocalisationTable")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<LocalisationTable>)
				.ToArray();

			var files = Directory.GetFiles(path, "*.csv");

			if (files.Length == 0)
			{
				throw new Exception("There are no .csv files in the directory specified");
			}

			var schemaFile = files[0];

			ImportSchema(schemaFile, settings);

			foreach (var file in files)
			{
				var fileName = Path.GetFileNameWithoutExtension(file);
				var table = localisationTables.FirstOrDefault(t => t.name == fileName);
				if (table == null)
				{
					Debug.LogWarning($"LocalisationTable could not be found with the name {fileName}");
					continue;
				}

				ImportFromCsv(file, table, settings.Schema);
			}

			foreach (var localisationTable in localisationTables)
			{
				Resources.UnloadAsset(localisationTable);
			}
		}
	}
}