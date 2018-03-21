using DUCK.Editor.Inspectors;
using DUCK.Utils;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DUCK.AudioSystem.Editor
{
	[CustomEditor(typeof(SequenceAudioConfig))]
	public class SequenceAudioConfigEditor : ArrayInspector
	{
		private SequenceAudioConfig config;
		private SerializedProperty delay;

		protected override Action<SerializedProperty> drawArrayElement { get { return DrawEntry; } }
		protected override string arrayPropertyName { get { return "entries"; } }

		protected override void OnEnable()
		{
			base.OnEnable();

			delay = serializedObject.FindProperty("delay");
		}

		protected override void DrawPostArray()
		{
			base.DrawPostArray();

			EditorGUILayout.Space();
			delay.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Delay between clips:", delay.floatValue));
		}

		private void DrawEntry(SerializedProperty property)
		{
			var audioConfig = property.FindPropertyRelative("audioConfig");
			var clipMode = property.FindPropertyRelative("clipMode");
			var clipIndexConstant = property.FindPropertyRelative("clipIndexConstant");
			var clipIndexParameter = property.FindPropertyRelative("clipIndexParameter");

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.PropertyField(audioConfig);

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(clipMode);

					var audioClipMode = (SequenceAudioConfig.AudioClipIndexType)clipMode.enumValueIndex;
					switch (audioClipMode)
					{
						case SequenceAudioConfig.AudioClipIndexType.Constant:
							clipIndexConstant.intValue = Math.Max(0, EditorGUILayout.IntField(clipIndexConstant.intValue));
							break;

						case SequenceAudioConfig.AudioClipIndexType.Parameter:
							var stringValue = EditorGUILayout.TextField(clipIndexConstant.stringValue);
							if (!string.IsNullOrEmpty(stringValue))
							{
								stringValue = stringValue.Trim().Replace(" ", string.Empty);
							}
							clipIndexParameter.stringValue = stringValue;
							break;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}
}