using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor.Window
{
	public static class GuiUtils
	{
		public static string OpenCsvFileDialog()
		{
			var filters = new []
			{
				"CSV files", "csv",
				"Text files", "txt",
				"All files", ""
			};

			var path = EditorUtility.OpenFilePanelWithFilters(
				"Localisation table contents",
				Application.dataPath,
				filters);

			return path;
		}

		public static string OpenFolderPanel()
		{
			var path = EditorUtility.OpenFolderPanel(
				"Localisations",
				Application.dataPath,
				"Localisations");

			return path;
		}

		public static string SaveCsvFileDialog(string fileName)
		{
			var path = EditorUtility.SaveFilePanel(
				"Save localisation table",
				Application.dataPath,
				$"{fileName}-0.csv",
				"csv");

			return path;
		}
	}
}