using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor.Window
{
	public partial class LocalisationWindow
	{
		private class CodeGenerationGui
		{
			private const string BUILT_IN_CONSTS_TEMPLATE_GUID = "b65180d1b746cf848a0eb57f4490dace";

			private TextAsset constsTemplate;

			public void OnEnable()
			{
				constsTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(
					AssetDatabase.GUIDToAssetPath(BUILT_IN_CONSTS_TEMPLATE_GUID));
			}

			public void Draw()
			{
				// Draw title
				EditorGUILayout.Space();
				GUILayout.Label("Generate localisation consts", EditorStyles.boldLabel);

				// Indent
				EditorGUI.indentLevel++;
				{
					// Show help box
					EditorGUILayout.HelpBox(
						"Click Generate to build a class file which can be referenced application-side to get localisation keys and other data in-game.",
						MessageType.Info);

					// Show paths
					EditorGUILayout.LabelField("Output filename:", LocalisationSettings.Current.CodeGenerationFilePath);
					EditorGUILayout.LabelField("Localisation data path:",
						LocalisationSettings.Current.LocalisationTableFolder + "/");

					DrawConstsTemplateAndGenerateButton();
				}
				EditorGUI.indentLevel--;
			}

			private void DrawConstsTemplateAndGenerateButton()
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Template required to generate localisation consts");

				EditorGUILayout.BeginHorizontal();
				constsTemplate =
					(TextAsset) EditorGUILayout.ObjectField("Consts template:", constsTemplate, typeof(TextAsset),
						false);

				EditorGUI.BeginDisabledGroup(constsTemplate == null);
				if (GUILayout.Button("Generate Consts"))
				{
					LocalisationConstsGenerator.GenerateLocalisationConfig(
						constsTemplate, LocalisationSettings.Current.Schema);
				}

				EditorGUI.EndDisabledGroup();

				EditorGUILayout.EndHorizontal();
			}
		}
	}
}