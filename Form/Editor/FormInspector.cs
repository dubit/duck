using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DUCK.Form.Editor
{
	[CustomEditor(typeof(Form))]
	public class FormInspector : UnityEditor.Editor
	{
		private SerializedProperty submitButton;
		private ReorderableList list;

		private void OnEnable()
		{
			submitButton = serializedObject.FindProperty("submitButton");
			list = new ReorderableList(serializedObject, serializedObject.FindProperty("formFields"),
				true, true, true, true)
			{
				drawHeaderCallback = DrawReorderableListHeader,
				drawElementCallback = DrawReorderableListElement
			};
		}

		private void DrawReorderableListHeader(Rect rect)
		{
			GUI.Label(rect, "Form Fields");
		}

		private void DrawReorderableListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				element, GUIContent.none);
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Space(10);
			serializedObject.Update();
			list.DoLayoutList();
			GUILayout.Space(5);
			EditorGUILayout.PropertyField(submitButton);
			serializedObject.ApplyModifiedProperties();
		}
	}
}