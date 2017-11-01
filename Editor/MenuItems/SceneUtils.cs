using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DUCK.Editor.MenuItems
{
	// Needed for detecting playing mode state
	[ExecuteInEditMode]
	public sealed class SceneUtils : EditorWindow
	{
		private const string ICON_PATH = "Assets/DUCK/Editor/MenuItems/Icons/";

		private const string TARGET_SCENE_INDEX = "TARGET_SCENE_INDEX";
		private const string LAST_SCENE_INDEX = "LAST_SCENE_INDEX";
		private const string LAST_SCENE_PATH = "LAST_SCENE_PATH";
		private const string IS_LAST_SCENE_UNLOADED = "IS_LAST_SCENE_UNLOADED";
		private const int DEFAULT_GRID_BUTTON_WIDTH = 168;

		private static EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[0];
		private static string[] sceneNames = new string[0];

		private Texture playButtonOnIcon;
		private Texture playButtonOffIcon;
		private Texture jumpInButtonIcon;
		private Texture jumpOutButtonIcon;
		private Texture sceneSelectionCurrentIcon;
		private Texture sceneSelectionJumpIcon;
		private GUIContent playButtonGUIContent;
		private GUIContent jumpButtonGUIContent;
		private Vector2 scrollPos;

		/// <summary>
		/// For better controls for jump in / out scenes.
		/// Boolean is not enough because the user may cancel the request.
		/// </summary>
		private enum JumpResult
		{
			Success,
			Fail,
			Cancelled
		}

		/// <summary>
		/// Showing the window.
		/// </summary>
		[MenuItem("DUCK/Scene Utils/Show Window", false, 100)]
		public static void ShowWindow()
		{
			GetWindow<SceneUtils>(false, "Scene Utils", true);
		}

		/// <summary>
		/// Jump to the target scene (if there is one) then play the game!
		/// After the game stops jump back to the last scene (if there is one).
		/// </summary>
		[MenuItem("DUCK/Scene Utils/Play _`", false, 200)]
		public static void Play()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				switch (JumpInScene())
				{
					// Cancel the play request if the user cancelled the save
					case JumpResult.Cancelled:
						return;
					// On success we are going to mark the unloaded to true (for jump back automatically after playing)
					case JumpResult.Success:
						EditorPrefs.SetBool(IS_LAST_SCENE_UNLOADED, true);
						break;
				}
				EditorApplication.isPlaying = true;
			}
			else if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}
		}

		/// <summary>
		/// Jump into a scene or jump back to the previous scene
		/// </summary>
		[MenuItem("DUCK/Scene Utils/Jump Scene", false, 201)]
		public static void JumpScene()
		{
			// Is playing?
			if (EditorApplication.isPlaying) return;

			// If we failed on jump out from a scene
			if (JumpOutScene() == JumpResult.Fail)
			{
				// Then we jump into a scene!
				JumpInScene();
			}
		}

		/// <summary>
		/// Jump to the target scene (user selected one on GUI)
		/// </summary>
		/// <returns>False if no jumps happenning.</returns>
		private static JumpResult JumpInScene()
		{
			// No target path?
			var targetSceneIndex = GetTargetSceneIndex();
			if (targetSceneIndex < 0)
			{
				return JumpResult.Fail;
			}

			// Current scene is the target scene?
			var currentScene = SceneManager.GetActiveScene();
			if (currentScene.buildIndex == targetSceneIndex)
			{
				return JumpResult.Fail;
			}

			// User cancelled?
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				return JumpResult.Cancelled;
			}

			// READY TO JUMP!
			EditorPrefs.SetInt(LAST_SCENE_INDEX, currentScene.buildIndex);
			if (currentScene.buildIndex < 0)
			{
				EditorPrefs.SetString(LAST_SCENE_PATH, currentScene.path);
			}
			EditorSceneManager.OpenScene(scenes[targetSceneIndex].path, OpenSceneMode.Single);

			return JumpResult.Success;
		}

		/// <summary>
		/// Jump out (back) to the previous scene if it exist
		/// </summary>
		/// <returns>False if no jumps happenning.</returns>
		private static JumpResult JumpOutScene()
		{
			// The last scene not exist?
			var lastSceneIndex = EditorPrefs.GetInt(LAST_SCENE_INDEX, -1);
			var lastScenePath = EditorPrefs.GetString(LAST_SCENE_PATH, null);
			if (lastSceneIndex < 0 && string.IsNullOrEmpty(lastScenePath))
			{
				return JumpResult.Fail;
			}

			// Current scene is the last scene?
			var currentScene = SceneManager.GetActiveScene();
			if (currentScene.buildIndex == lastSceneIndex)
			{
				return JumpResult.Fail;
			}

			// User cancelled?
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				return JumpResult.Cancelled;
			}

			// READY TO JUMP!
			EditorSceneManager.OpenScene(lastSceneIndex < 0 ? lastScenePath : scenes[lastSceneIndex].path, OpenSceneMode.Single);
			EditorPrefs.SetInt(LAST_SCENE_INDEX, -1);
			EditorPrefs.SetString(LAST_SCENE_PATH, null);

			return JumpResult.Success;
		}

		/// <summary>
		/// Get the target scene (user selected one on GUI)
		/// </summary>
		/// <returns>The path of the target scene</returns>
		private static int GetTargetSceneIndex()
		{
			var index = EditorPrefs.GetInt(TARGET_SCENE_INDEX, -1);
			if (index >= 0 && index < scenes.Length)
			{
				return index;
			}

			return -1;
		}

		/// <summary>
		/// Init icons and tooltips
		/// </summary>
		private void OnEnable()
		{
			playButtonOnIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "play_button_on.png");
			playButtonOffIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "play_button_off.png");
			playButtonGUIContent = new GUIContent {tooltip = "Play selected scene and back to the current one later."};

			jumpInButtonIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "jump_in_button.png");
			jumpOutButtonIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "jump_out_button.png");
			jumpButtonGUIContent = new GUIContent {tooltip = "Jump to the selected scene or back to the previous one."};

			sceneSelectionCurrentIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "scene_selection_current.png");
			sceneSelectionJumpIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH + "scene_selection_last.png");
		}

		/// <summary>
		/// Refresh the scene list on focus
		/// </summary>
		private void OnFocus()
		{
			scenes = EditorBuildSettings.scenes;
			sceneNames = scenes.Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
		}

		/// <summary>
		/// Jump back to the previous scene if the player used the jump and play function.
		/// I know it sucks but this is the only way to detect you have just finished the playing mode.
		/// </summary>
		private void Update()
		{
			var isLastSceneUnloaded = EditorPrefs.GetBool(IS_LAST_SCENE_UNLOADED, false);
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying && isLastSceneUnloaded)
			{
				EditorPrefs.SetBool(IS_LAST_SCENE_UNLOADED, false);
				JumpOutScene();
			}
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();

			// Switch the icon for play button
			playButtonGUIContent.image = EditorApplication.isPlayingOrWillChangePlaymode ? playButtonOnIcon : playButtonOffIcon;
			if (GUILayout.Button(playButtonGUIContent, GUILayout.ExpandWidth(false)))
			{
				Play();
				return; // Kinda optional return
			}

			// Only available on edit mode
			GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

			// Switch the icon for jump button
			var lastSceneIndex = EditorPrefs.GetInt(LAST_SCENE_INDEX, -1);
			var lastScenePath = EditorPrefs.GetString(LAST_SCENE_PATH, null);
			jumpButtonGUIContent.image = lastSceneIndex < 0 && string.IsNullOrEmpty(lastScenePath) ? jumpInButtonIcon : jumpOutButtonIcon;
			if (GUILayout.Button(jumpButtonGUIContent, GUILayout.ExpandWidth(false)))
			{
				JumpScene();
				return; // Kinda optional return
			}

			// Begin Scroll View
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

				// Build the names -- mark up buttons with special indicators
				var currentSceneIndex = EditorApplication.isPlaying ? GetTargetSceneIndex() : SceneManager.GetActiveScene().buildIndex;
				var sceneNameGCs = new GUIContent[sceneNames.Length];
				for (var i = 0; i < sceneNames.Length; ++i)
				{
					var guiContent = new GUIContent(sceneNames[i]);
					if (i == currentSceneIndex)
					{
						guiContent.image = sceneSelectionCurrentIcon;
					}
					else if (i == lastSceneIndex)
					{
						guiContent.image = sceneSelectionJumpIcon;
					}
					sceneNameGCs[i] = guiContent;
				}

				// Get the persisted index (and clamp it if the user removed some scenes from the build settings)
				var targetSceneIndex = Mathf.Clamp(EditorPrefs.GetInt(TARGET_SCENE_INDEX, 0), 0, sceneNameGCs.Length - 1);
				// Limit the width of the grid buttons and make several columns for it
				var selectedIndex = GUILayout.SelectionGrid(targetSceneIndex, sceneNameGCs, Mathf.Max(1, Mathf.FloorToInt(position.width / DEFAULT_GRID_BUTTON_WIDTH)));
				// We don't want to do it every frame so we do it with dirty check
				if (selectedIndex != targetSceneIndex)
				{
					targetSceneIndex = selectedIndex;
					EditorPrefs.SetInt(TARGET_SCENE_INDEX, targetSceneIndex);

					// IMPORTANT FEATURE: Reset the jump scene when the user selected a new target scene
					EditorPrefs.SetInt(LAST_SCENE_INDEX, -1);
					EditorPrefs.SetString(LAST_SCENE_PATH, null);
				}

				EditorGUILayout.EndScrollView();
			}
			// End of Scroll View

			// Enable the GUI again
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}
	}
}