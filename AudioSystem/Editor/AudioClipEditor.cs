using DUCK.Editor;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DUCK.AudioSystem.Editor
{
	[CustomEditor(typeof(AudioClip))]
	[CanEditMultipleObjects]
	public class AudioClipEditor : UnityEditor.Editor
	{
		private bool groupClips;

		private void OnEnable()
		{
			groupClips = false;
		}
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUI.enabled = true;

			EditorGUILayout.BeginHorizontal();
			var multipleSelected = (Selection.objects.Length > 1);
			if (GUILayout.Button(multipleSelected ? "Create AudioConfigs for these clips" : "Create AudioConfig for this clip"))
			{
				CreateAudioConfigs(Selection.objects, groupClips);
			}

			if (multipleSelected)
			{
				groupClips = EditorGUILayout.Toggle("Group?", groupClips);
			}

			EditorGUILayout.EndHorizontal();
		}

		private static void CreateAudioConfigs(UnityEngine.Object[] selectedObjects, bool groupIntoOneConfig = false)
		{
			var sharedAudioConfigPath = "";
			AudioConfig sharedAudioConfig = null;

			foreach (var obj in selectedObjects)
			{
				var audioClip = obj as AudioClip;
				var assetFolderPath = "Assets" + new FileInfo(AssetDatabase.GetAssetPath(audioClip)).Directory.FullName.Substring(Application.dataPath.Length);

				var newAudioConfig = sharedAudioConfig;

				if (newAudioConfig == null)
				{
					newAudioConfig = ScriptableObject.CreateInstance<AudioConfig>();

					if (groupIntoOneConfig)
					{
						sharedAudioConfig = newAudioConfig;
						sharedAudioConfigPath = assetFolderPath + "/New " + newAudioConfig.GetType() + ".asset";
					}
					else
					{
						newAudioConfig.name = audioClip.name;
					}
				}

				newAudioConfig.AudioClips.Add(audioClip);

				if (!groupIntoOneConfig)
				{
					var newAudioConfigPath = assetFolderPath + "/" + audioClip.name + ".asset";
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