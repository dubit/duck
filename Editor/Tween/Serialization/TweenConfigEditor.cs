using UnityEditor;

namespace DUCK.Tween.Serialization.Editor
{
	[CustomEditor(typeof(TweenConfig))]
	public class TweenConfigEditor : UnityEditor.Editor
	{
		private TweenConfig tweenConfig;

		public void OnEnable()
		{
			tweenConfig = target as TweenConfig;
		}

		public override void OnInspectorGUI()
		{
			TweenObjectCreationConfigEditor.Draw(tweenConfig.Config, tweenConfig);
		}
	}
}