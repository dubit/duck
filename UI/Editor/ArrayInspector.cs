using System;
using UnityEditor;
using UnityEngine;

namespace Dubit.DUCK.Editor.Inspectors
{
	public abstract class ArrayInspector : UnityEditor.Editor
	{
		protected abstract string arrayPropertyName
		{
			get;
		}

		protected virtual int arrayFoldoutBitmask
		{
			get
			{
				return -1;
			}

			set
			{
				// Default: do nothing
			}
		}

		protected abstract Action<SerializedProperty> drawArrayElement
		{
			get;
		}

		protected SerializedProperty serializedArray;

		protected void OnEnable()
		{
			serializedArray = serializedObject.FindProperty(arrayPropertyName);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (serializedArray != null)
			{
				arrayFoldoutBitmask = DrawArray(serializedArray, drawArrayElement, arrayFoldoutBitmask);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public static int DrawArray(SerializedProperty arrayProperty, Action<SerializedProperty> drawArrayElement, int selectionBitmask = -1)
		{
			if (arrayProperty == null || arrayProperty.arraySize < 0)
				return -1;

			for (var i = 0; i < arrayProperty.arraySize; i++)
			{
				selectionBitmask = DrawArrayElement(arrayProperty, i, drawArrayElement, selectionBitmask);
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format("{0} elements ({1})", arrayProperty.arraySize, arrayProperty.arrayElementType));

			var fullBitmask = 0;
			for (var i = 0; i < arrayProperty.arraySize; i++)
			{
				fullBitmask = fullBitmask | (1 << i);
			}

			EditorGUI.BeginDisabledGroup(selectionBitmask == fullBitmask);
			if (GUILayout.Button("< >"))
			{
				selectionBitmask = fullBitmask;
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(selectionBitmask <= 0);
			if (GUILayout.Button("><"))
			{
				selectionBitmask = 0;
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();

			if (GUILayout.Button("+"))
			{
				arrayProperty.arraySize++;
			}

			EditorGUILayout.EndHorizontal();

			return selectionBitmask;
		}

		private static int DrawArrayElement(SerializedProperty arrayProperty, int index, Action<SerializedProperty> drawArrayElement, int selectionBitmask = -1)
		{
			var elementProperty = arrayProperty.GetArrayElementAtIndex(index);

			if (elementProperty != null)
			{
				var label = string.Format("Element {0}", index);
				bool foldOut;
				if (selectionBitmask >= 0)
				{
					foldOut = (selectionBitmask & (1 << index)) > 0;
					var newFoldout = EditorGUILayout.Foldout(foldOut, label);

					if (newFoldout && !foldOut)
					{
						selectionBitmask = selectionBitmask | (1 << index);
					}
					else if (foldOut && !newFoldout)
					{
						selectionBitmask = selectionBitmask & ~(1 << index);
					}

					foldOut = newFoldout;
				}
				else
				{
					EditorGUILayout.LabelField(label);
					foldOut = true;
				}

				if (foldOut)
				{
					DrawElementContainer(arrayProperty, elementProperty, index, drawArrayElement);
				}
			}

			return selectionBitmask;
		}

		private static void DrawElementContainer(SerializedProperty arrayProperty, SerializedProperty elementProperty, int index, Action<SerializedProperty> drawArrayElement)
		{
			var buttonWidth = 60f;

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical("Box");

			if (drawArrayElement != null)
			{
				drawArrayElement(elementProperty);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("Box");

			EditorGUI.BeginDisabledGroup(index <= 0);
			if (GUILayout.Button("▲", GUILayout.Width(buttonWidth)))
			{
				arrayProperty.MoveArrayElement(index, index - 1);
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(index >= arrayProperty.arraySize-1);
			if (GUILayout.Button("▼", GUILayout.Width(buttonWidth)))
			{
				arrayProperty.MoveArrayElement(index, index + 1);
			}
			EditorGUI.EndDisabledGroup();

			if (GUILayout.Button("X", GUILayout.Width(buttonWidth)))
			{
				arrayProperty.DeleteArrayElementAtIndex(index);
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
	}
}
