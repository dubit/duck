using UnityEditor;

namespace DUCK.Tween.Serialization.Editor
{
	[CustomEditor(typeof(TweenConfigScriptableObject))]
	public class TweenConfigScriptableObjectEditor : UnityEditor.Editor
	{
		private TweenConfigScriptableObject tweenConfigScriptableObject;

		public void OnEnable()
		{
			tweenConfigScriptableObject = target as TweenConfigScriptableObject;
		}

		public override void OnInspectorGUI()
		{
			TweenConfigEditor.Draw(tweenConfigScriptableObject.Config);
		}
	}
}