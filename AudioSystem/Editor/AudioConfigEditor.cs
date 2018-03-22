using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DUCK.AudioSystem.Editor
{
	[CustomEditor(typeof(AudioConfig))]
	[CanEditMultipleObjects]
	public class AudioConfigEditor : UnityEditor.Editor
	{
		private AudioConfig config;
		private SerializedProperty audioClips;
		private SerializedProperty volumeScale;
		private SerializedProperty playMode;
		private SerializedProperty templateAudioSource;
		private SerializedProperty loop;
		private SerializedProperty priority;
		private SerializedProperty spatialMode;
		private SerializedProperty minDistance;
		private SerializedProperty maxDistance;
		private SerializedProperty alternativeText;

		private MethodInfo playClipMethod;
		private MethodInfo stopAllClipsMethod;

		private void OnEnable()
		{
			// The config
			config = (AudioConfig)target;

			// Serialized Properties
			audioClips = serializedObject.FindProperty("audioClips");
			volumeScale = serializedObject.FindProperty("volumeScale");
			playMode = serializedObject.FindProperty("playMode");
			templateAudioSource = serializedObject.FindProperty("templateAudioSource");
			loop = serializedObject.FindProperty("loop");
			priority = serializedObject.FindProperty("priority");
			spatialMode = serializedObject.FindProperty("spatialMode");
			minDistance = serializedObject.FindProperty("minDistance");
			maxDistance = serializedObject.FindProperty("maxDistance");
			alternativeText = serializedObject.FindProperty("alternativeText");

			// Editor Audio Utils method for the Preview feature
			// Pretty much the only option to play an audio clip in the Editor, for now
			var audioUtil = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
			playClipMethod = audioUtil.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null,
				new[] {typeof(AudioClip)}, null);
			stopAllClipsMethod = audioUtil.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null,
				new Type[] { }, null);
		}

		private void PlayClip(AudioClip clip)
		{
			// Always stop all clips before playing a new clip...
			// Otherwise it will create a new channel and we will have no way to stop it.
			StopAllClips();
			playClipMethod.Invoke(null, new object[] {clip});
		}

		/// <summary>
		/// Note: Unity will not stop all clips in all channel, it will only stop for the last channel.
		/// Therefore, always stop all clips before you play anything.
		/// Or go write (and hack with a better code.
		/// </summary>
		private void StopAllClips()
		{
			stopAllClipsMethod.Invoke(null, new object[] { });
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(audioClips, true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(volumeScale);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(playMode);
			switch ((AudioConfig.AudioPlayModes) playMode.enumValueIndex)
			{
				case AudioConfig.AudioPlayModes.Normal:
					EditorGUILayout.HelpBox("Play normally -- no restrictions at all.", MessageType.None, true);
					break;
				case AudioConfig.AudioPlayModes.Override:
					EditorGUILayout.HelpBox("Each play will override (stop) the previous one.", MessageType.Info, true);
					break;
				case AudioConfig.AudioPlayModes.OneAtATime:
					EditorGUILayout.HelpBox("Will not play if the previous one is still playing.", MessageType.Info, true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(templateAudioSource);
			if (templateAudioSource.objectReferenceValue != null)
			{
				EditorGUILayout.HelpBox("The rest settings will now override by the template.", MessageType.Info, true);
			}
			else
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(loop);
				EditorGUILayout.PropertyField(priority);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(spatialMode);
				switch ((AudioConfig.SpatialModes)spatialMode.enumValueIndex)
				{
					case AudioConfig.SpatialModes.Pure2D:
						EditorGUILayout.HelpBox("No Spatial Blend at all.", MessageType.None, true);
						break;
					case AudioConfig.SpatialModes.Simple2D:
						EditorGUILayout.HelpBox("Volume will be scaled based on the distance.\n" +
						                        "Still using 2D Spatial Blend. (No Stereo Pan calculation)", MessageType.Info, true);
						EditorGUILayout.PropertyField(minDistance);
						EditorGUILayout.PropertyField(maxDistance);
						break;
					case AudioConfig.SpatialModes.Simple3D:
						EditorGUILayout.HelpBox("Using 3D Spatial Blend + Linear Rolloff.", MessageType.Info, true);
						EditorGUILayout.PropertyField(minDistance);
						EditorGUILayout.PropertyField(maxDistance);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(alternativeText);

			serializedObject.ApplyModifiedProperties();

			// Quick Preview
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Quick Preview", "(no volume or templates applied)");
			EditorGUILayout.BeginHorizontal();
			GUI.enabled = config.AudioClips.Count > 0;
			if (GUILayout.Button("Play Solo"))
			{
				PlayClip(config.GetAudioClip());
			}
			if (GUILayout.Button("Stop"))
			{
				StopAllClips();
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
		}
	}
}