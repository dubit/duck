using UnityEditor;
using UnityEngine;

namespace DUCK.Utils.Editor
{
	public class PrefabReplacer : EditorWindow
	{
		[SerializeField]
		private GameObject targetPrefab;

		[MenuItem("DUCK/Prefab Replacer")]
		private static void CreateReplaceWithPrefab()
		{
			GetWindow<PrefabReplacer>("Prefab Replacer");
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void OnGUI()
		{
			EditorGUILayout.Space();

			targetPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", targetPrefab, typeof(GameObject), false);

			if (targetPrefab == null)
			{
				EditorGUILayout.HelpBox("Please select a prefab above", MessageType.Info);
			}
			else
			{
				EditorGUILayout.Space();
			}

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.HelpBox("You have selected: " + Selection.objects.Length + " objects.", MessageType.None);

			GUI.enabled = targetPrefab != null;

			if (GUILayout.Button("Replace"))
			{
				var selection = Selection.gameObjects;

				for (var i = selection.Length - 1; i >= 0; --i)
				{
					var selected = selection[i];

					GameObject newObject;
					if (PrefabUtility.GetPrefabType(targetPrefab) == PrefabType.Prefab)
					{
						newObject = (GameObject)PrefabUtility.InstantiatePrefab(targetPrefab);
					}
					else
					{
						newObject = Instantiate(targetPrefab);
						newObject.name = targetPrefab.name;
					}

					if (newObject == null)
					{
						Debug.LogError("Cannot instantiating prefab");
						break;
					}

					Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
					newObject.transform.SetParent(selected.transform.parent);
					newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
					newObject.transform.localPosition = selected.transform.localPosition;
					newObject.transform.localRotation = selected.transform.localRotation;
					newObject.transform.localScale = selected.transform.localScale;
					Undo.DestroyObjectImmediate(selected);
				}
			}

			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}
	}
}