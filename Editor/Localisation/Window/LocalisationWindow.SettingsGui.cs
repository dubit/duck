using DUCK.Localisation.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor.Window
{
	public partial class LocalisationWindow
	{
		private class SettingsGui
		{
			public void Draw()
			{
				// Draw title
				EditorGUILayout.Space();
				GUILayout.Label("Settings", EditorStyles.boldLabel);

				// Draw settings field
				var settings = LocalisationSettings.Current;

				settings =
					(LocalisationSettings) EditorGUILayout.ObjectField(
						"Settings",
						settings,
						typeof(LocalisationSettings),
						false);

				LocalisationSettings.Current = settings;

				// if no settings then draw the help box and create button
				if (settings == null)
				{
					EditorGUILayout.HelpBox(
						"Please specify a settings object, or alternatively create a new one with the button below",
						MessageType.Info);

					if (GUILayout.Button("Create new LocalisationSettings"))
					{
						LocalisationSettings.Create();
					}

					return;
				}

				EditorGUILayout.Space();

				if (GUILayout.Button("Import Schema"))
				{
					var path = GuiUtils.OpenCsvFileDialog();
					if (!string.IsNullOrEmpty(path))
					{
						Importer.ImportSchema(path, settings);

						Debug.Log("Finished importing to schema");
					}
				}

				if (GUILayout.Button("Import All (schema & translations)"))
				{
					var path = GuiUtils.OpenFolderPanel();
					if (!string.IsNullOrEmpty(path))
					{
						Importer.ImportAll(path, settings);

						Debug.Log("Finished importing everything!");
					}
				}
			}
		}
	}
}