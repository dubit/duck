using UnityEditor;

namespace DUCK.Tween.Serialization.Editor
{
	[CustomEditor(typeof(TweenBuilder))]
	public class TweenBuilderEditor : UnityEditor.Editor
	{
		private TweenBuilder tweenBuilder;

		public void OnEnable()
		{
			tweenBuilder = target as TweenBuilder;
		}

		public override void OnInspectorGUI()
		{
			tweenBuilder.PlayOnStart = EditorGUILayout.Toggle("Play on Start", tweenBuilder.PlayOnStart);

			tweenBuilder.Repeat = EditorGUILayout.Toggle("Repeat?", tweenBuilder.Repeat);

			if (tweenBuilder.Repeat)
			{
				tweenBuilder.RepeatCount = EditorGUILayout.IntField("Repeat Count", tweenBuilder.RepeatCount);
			}

			tweenBuilder.UseReferencedConfig = EditorGUILayout.Toggle("Use referenced Config", tweenBuilder.UseReferencedConfig);

			if (tweenBuilder.UseReferencedConfig)
			{
				tweenBuilder.TweenConfig = (TweenConfig)
					EditorGUILayout.ObjectField("Config", tweenBuilder.TweenConfig,
						typeof(TweenConfig), false);
			}
			else
			{
				TweenObjectCreationConfigEditor.Draw(tweenBuilder.ObjectCreationConfig);
			}
		}

		// TODO: provide support for handles for positions
	}
}