using DUCK.Editor.Inspectors;
using DUCK.Utils;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DUCK.AudioSystem.Editor
{
	[CustomEditor(typeof(AudioConfigSequence))]
	public class AudioConfigSequenceEditor : ArrayInspector
	{
		private AudioConfigSequence config;
		private SerializedProperty delay;

		protected override Action<SerializedProperty> drawArrayElement { get { return DrawEntry; } }
		protected override string arrayPropertyName { get { return "entries"; } }

		protected override void OnEnable()
		{
			base.OnEnable();

			delay = serializedObject.FindProperty("delay");
		}

		private void DrawEntry(SerializedProperty property)
		{
			var audioConfig = property.FindPropertyRelative("audioConfig");

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.PropertyField(audioConfig);

				var clipMode = property.FindPropertyRelative("clipMode");
				var clipIndexConstant = property.FindPropertyRelative("clipIndexConstant");
				var clipIndexParameter = property.FindPropertyRelative("clipIndexParameter");

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(clipMode);

					var audioClipMode = (AudioConfigSequence.AudioClipIndexType)clipMode.enumValueIndex;
					switch (audioClipMode)
					{
						case AudioConfigSequence.AudioClipIndexType.Constant:
							clipIndexConstant.intValue = Math.Max(0, EditorGUILayout.IntField(clipIndexConstant.intValue));
							break;

						case AudioConfigSequence.AudioClipIndexType.Parameter:
							clipIndexParameter.stringValue = EditorGUILayout.TextField(clipIndexParameter.stringValue).Trim().Replace(" ", string.Empty);
							break;
					}
				}
				EditorGUILayout.EndHorizontal();

				var postClipDelay = property.FindPropertyRelative("postClipDelay");
				EditorGUILayout.PropertyField(postClipDelay);
			}
			EditorGUILayout.EndVertical();
		}
	}
}