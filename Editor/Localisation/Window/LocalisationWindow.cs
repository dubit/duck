using UnityEditor;

namespace DUCK.Localisation.Editor.Window
{
	public partial class LocalisationWindow : EditorWindow
	{
		private readonly SettingsGui settingsGui = new SettingsGui();
		private readonly CodeGenerationGui codeGenerationGui = new CodeGenerationGui();
		private readonly TablesGui tablesGui = new TablesGui();

		[MenuItem("DUCK/Localisation")]
		public static void ShowWindow()
		{
			GetWindow(typeof(LocalisationWindow), false, "Localisation");
		}

		private void OnEnable()
		{
			codeGenerationGui.OnEnable();
		}

		private void OnGUI()
		{
			settingsGui.Draw();

			if (LocalisationSettings.Current == null) return;

			codeGenerationGui.Draw();

			tablesGui.Draw();
		}
	}
}