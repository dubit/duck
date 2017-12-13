using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DUCK.Forms.Editor
{
	/// <summary>
	/// Custom inspector for Form.cs, easy to use reorderable list for the field configs.
	/// </summary>
	[CustomEditor(typeof(Form))]
	public class FormInspector : UnityEditor.Editor
	{
		private const int CLEAR_ON_RESET_COLUMN_WIDTH = 90;
		private SerializedProperty submitButton;
		private ReorderableList list;

		private void OnEnable()
		{
			submitButton = serializedObject.FindProperty("submitButton");
			var fieldConfigs = serializedObject.FindProperty("fieldConfigs");

			list = new ReorderableList(serializedObject, fieldConfigs,
				true, true, true, true)
			{
				drawHeaderCallback = DrawReorderableListHeader,
				drawElementCallback = DrawReorderableListElement
			};
		}

		private void DrawReorderableListHeader(Rect rect)
		{
			var titleWidth = rect.width * 0.7f;
			GUI.Label(new Rect(rect.x, rect.y, titleWidth, rect.height), "Form Fields");
			GUI.Label(new Rect(rect.x + rect.width - CLEAR_ON_RESET_COLUMN_WIDTH, rect.y, CLEAR_ON_RESET_COLUMN_WIDTH, rect.height), "Clear on Submit");
		}

		private void DrawReorderableListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			const int checkboxWidth = 20;
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var field = element.FindPropertyRelative("field");
			var clearOnSubmit = element.FindPropertyRelative("clearOnSubmit");
			rect.y += 2;
			var fieldWidth = rect.width - checkboxWidth * 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), field, GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - checkboxWidth, rect.y, checkboxWidth, EditorGUIUtility.singleLineHeight), clearOnSubmit, GUIContent.none);
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