using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor
{
	public static class LocalisationConstsGenerator
	{
		// Code generation parameters for the Loc bridge class
		private const string CONFIG_REPLACE_SUFFIX = ".old";
		private const string CONFIG_LOC_CLASS_NAME = "Loc";

		/// <summary>
		/// Generates the LocalisationConfig class from the supplied key schema - this allows application-side code to access the available keys
		/// and request localised content with them. Also exports some cross functionality such as the Localisation folder path which the application
		/// will need.
		/// </summary>
		public static void GenerateLocalisationConfig(TextAsset configTemplate, LocalisationKeySchema schema)
		{
			if (configTemplate == null)
			{
				throw new Exception("No template supplied - can't generate localisation config file");
			}

			var filePath = $"{Application.dataPath}/{LocalisationSettings.Current.CodeGenerationFilePath}";
			var directoryPath = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			if (File.Exists(filePath))
			{
				var renamedPath = filePath + CONFIG_REPLACE_SUFFIX;
				if (File.Exists(renamedPath))
				{
					File.Delete(renamedPath);
				}

				File.Delete(filePath);
			}

			Debug.Log("Writing to " + filePath);
			var totalKeys = 0;

			var stringBuilder = new StringBuilder();
			foreach (var category in schema.categories)
			{
				stringBuilder.AppendLine($"\t\tpublic enum {category.name}");
				stringBuilder.AppendLine("\t\t{");
				for (var i = 0; i < category.keys.Length; i++)
				{
					var key = category.keys[i];

					stringBuilder.Append("\t\t\t" + key + " = " + CrcUtils.GetCrc(category.name, key));
					if (i < category.keys.Length - 1)
					{
						stringBuilder.AppendLine(",");
					}
					else
					{
						stringBuilder.AppendLine();
					}
				}
				stringBuilder.AppendLine("\t\t}");
				totalKeys += category.keys.Length;
			}

			var outputText = configTemplate.text;
			outputText = outputText.Replace("{CLASS}", CONFIG_LOC_CLASS_NAME);
			outputText = outputText.Replace("{KEYS}", stringBuilder.ToString());
			File.WriteAllText(filePath, outputText);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			Debug.Log($"Successfully generated {totalKeys} localisation keys.");
		}
	}
}