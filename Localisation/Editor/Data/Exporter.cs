using System;
using System.IO;
using UnityEngine;

namespace DUCK.Localisation.Editor.Data
{
	public static class Exporter
	{
		/// <summary>
		/// Exports table contents to a csv file in the format (Category,Key,Content)
		/// </summary>
		/// <param name="path">Path where the csv file will be written</param>
		/// <param name="locTable">The localisation table to export</param>
		/// <param name="currentSchema">The current schema</param>
		/// <param name="emptyValuesOnly">If set to true only the missing values will be exported</param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void ExportTableToCsv(
			string path,
			LocalisationTable locTable,
			LocalisationKeySchema currentSchema,
			bool emptyValuesOnly = false)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (locTable == null) throw new ArgumentNullException(nameof(locTable));
			if (currentSchema == null) throw new ArgumentNullException(nameof(currentSchema));

			Func<string, string, string, string> cleanOutput = (csvKey1, csvKey2, csvValue) =>
				$"{csvKey1},{csvKey2},\"{csvValue}\"";

			if (locTable.CRCEncodingVersion != CrcUtils.KEY_CRC_ENCODING_VERSION)
			{
				Debug.LogError(
					$"Table encoding version ({locTable.CRCEncodingVersion}) does not match current version (1) - please update the table before trying to export anything.");
				return;
			}

			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}

				var streamWriter = new StreamWriter(File.OpenWrite(path));

				foreach (var category in currentSchema.categories)
				{
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
							// ignored
						}

						if (string.IsNullOrEmpty(locValue) || !emptyValuesOnly)
						{
							streamWriter.WriteLine(cleanOutput(category.name, key, locValue));
						}
					}
				}

				streamWriter.Close();
				Debug.Log(
					$"Contents of table '{locTable.name}' written to: {path} {(emptyValuesOnly ? "(empty only)" : string.Empty)}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Could not save to CSV: {e.Message}");
			}
		}
	}
}