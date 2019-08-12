using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DUCK.AudioSystem.Editor
{
	public class AudioClipContextMenuExtensions : UnityEditor.Editor
	{
		private const string GENERATE_AUDIO_CONFIGS = "Assets/Generate AudioConfig";

		[MenuItem(GENERATE_AUDIO_CONFIGS, true)]
		private static bool CanGenerateAudioConfig()
		{
			return Selection.objects.All(obj => obj is AudioClip);
		}

		[MenuItem(GENERATE_AUDIO_CONFIGS)]
		private static void GenerateAudioConfig()
		{
			if (!CanGenerateAudioConfig())
			{
				Debug.LogError("Unable to generate an AudioConfig from the selected assets.");
				return;
			}

			GenerateAudioConfig(Selection.objects);
		}

		private static void GenerateAudioConfig(UnityEngine.Object[] selectedObjects, bool groupIntoOneConfig = true)
		{
			var sharedAudioConfigPath = "";
			AudioConfig sharedAudioConfig = null;

			foreach (var obj in selectedObjects)
			{
				var audioClip = obj as AudioClip;
				var assetFolderPath = "Assets" + new FileInfo(AssetDatabase.GetAssetPath(audioClip)).Directory.FullName.Substring(Application.dataPath.Length);
				var newAudioConfigPath = assetFolderPath + "/" + audioClip.name + ".asset";
				var newAudioConfig = sharedAudioConfig;

				if (newAudioConfig == null)
				{
					newAudioConfig = ScriptableObject.CreateInstance<AudioConfig>();

					if (groupIntoOneConfig)
					{
						sharedAudioConfig = newAudioConfig;
						sharedAudioConfigPath = newAudioConfigPath;
					}
					else
					{
						newAudioConfig.name = audioClip.name;
					}
				}

				newAudioConfig.AudioClips.Add(audioClip);

				if (!groupIntoOneConfig)
				{
					AssetDatabase.CreateAsset(newAudioConfig, AssetDatabase.GenerateUniqueAssetPath(newAudioConfigPath));
				}
			}

			if (groupIntoOneConfig)
			{
				AssetDatabase.CreateAsset(sharedAudioConfig, AssetDatabase.GenerateUniqueAssetPath(sharedAudioConfigPath));
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}