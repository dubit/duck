using UnityEditor;
using UnityEditor.UI;

namespace DUCK.UI.Editor
{
	[CustomEditor(typeof(AdvancedButton))]
	public class AdvancedButtonEditor : ButtonEditor
	{
		private SerializedProperty onOneClickProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			onOneClickProperty = serializedObject.FindProperty("onOneShotClick");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			serializedObject.Update();
			EditorGUILayout.PropertyField(onOneClickProperty);
			serializedObject.ApplyModifiedProperties();
		}
	}
}