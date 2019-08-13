using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Editor.Window
{
	public partial class LocalisationWindow
	{
		private class TablesGui
		{
			[MenuItem("Assets/Create/YourClass")]
			public static void CreateTable()
			{
				ScriptableObjectUtility.CreateAsset<LocalisationTable>();
			}
			
			private List<TableDrawer> tableDrawers;

			public void Refresh()
			{
				var localisationTablePaths = AssetDatabase.FindAssets($"t:{nameof(LocalisationTable)}")
					.Select(AssetDatabase.GUIDToAssetPath)
					.Where(p => !p.Contains("Tests/Data/Test"))
					.ToList();

				tableDrawers = new List<TableDrawer>(localisationTablePaths.Count);

				foreach (var tablePath in localisationTablePaths)
				{
					var table = AssetDatabase.LoadAssetAtPath<LocalisationTable>(tablePath);
					tableDrawers.Add(new TableDrawer(table));
					Resources.UnloadAsset(table);
				}
			}

			public void Draw()
			{
				// Draw title
				EditorGUILayout.Space();
				GUILayout.Label("Localisation tables", EditorStyles.boldLabel);

				// Draw refresh/create buttons
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Refresh")) Refresh();
				if (GUILayout.Button("Create new")) CreateNewTable();
				EditorGUILayout.EndHorizontal();

				if (tableDrawers == null)
				{
					Refresh();
					return;
				}

				foreach (var tableDrawer in tableDrawers)
				{
					tableDrawer.Draw();
				}
			}

			void CreateNewTable()
			{
				
			}
			
			
		}
	}
}

 
public static class ScriptableObjectUtility
{
	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
 
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (System.IO.Path.GetExtension (path) != "") 
		{
			path = path.Replace (System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
 
		AssetDatabase.CreateAsset (asset, assetPathAndName);
 
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}