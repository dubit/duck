﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Utils.Editor
{
	public class FindUnusedAssetsWindow : EditorWindow
	{
		private class Asset
		{
			public string Path { get; private set; }
			public string Name { get; private set; }
			public Object Obj { get; private set; }

			public Asset(string path, Object obj)
			{
				Path = path;
				Obj = obj;
				if (obj != null)
				{
					Name = obj.name;
				}
			}
		}

		private const float WIDTH = 420;
		private static bool includeVendor;
		private static bool includePlugins;
		private static bool includeDuck;
		private static bool hasEditorLagBeenFound;
		private static List<Asset> unusedAssets;
		private static Vector2 scrollPosition;
		private static FindUnusedAssetsWindow window;

		[MenuItem("DUCK/Find Unused Assets")]
		private static void Init()
		{
			window = (FindUnusedAssetsWindow) GetWindow(typeof(FindUnusedAssetsWindow));
			window.Show();
			window.maxSize = new Vector2(WIDTH, 1080);
			window.minSize = new Vector2(100, WIDTH);
			window.titleContent = new GUIContent("Find Unused Assets");

			hasEditorLagBeenFound = true;
		}

		private static void Search()
		{
			List<string> usedAssets = FindUsedAssets();

			if (usedAssets != null)
			{
				unusedAssets = FindUnusedObjects(FindAllProjectAssets(), usedAssets).OrderBy(a => a.Obj != null).ToList();
				hasEditorLagBeenFound = true;
			}
			else
			{
				hasEditorLagBeenFound = false;
			}
		}

		private void OnGUI()
		{
			EditorGUILayout.HelpBox("Does not include Scenes or Meta data.", MessageType.Info);
			GUILayout.BeginHorizontal();
			includeDuck = GUILayout.Toggle(includeDuck, "Include DUCK");
			includePlugins = GUILayout.Toggle(includePlugins, "Include Plugins");
			includeVendor = GUILayout.Toggle(includeVendor, "Include Vendor");
			if (GUILayout.Button("Search"))
			{
				Search();
			}
			GUILayout.EndHorizontal();

			if (!hasEditorLagBeenFound)
			{
				EditorGUILayout.HelpBox("Editor log not found! - You need to build your project before doing this!",
					MessageType.Warning);
			}
			else if (unusedAssets != null)
			{
				GUILayout.Label(string.Format("Found {0} unused assets!", unusedAssets.Count), EditorStyles.boldLabel);
				var scrollViewHeight = EditorGUIUtility.singleLineHeight * unusedAssets.Count;
				var scrollViewPositionY = EditorGUIUtility.singleLineHeight * 6;
				var scrollViewPosition = new Rect(0, scrollViewPositionY, WIDTH, window.position.height - scrollViewPositionY);
				var scrollView = new Rect(0, 0, WIDTH, scrollViewHeight);
				scrollPosition = GUI.BeginScrollView(scrollViewPosition, scrollPosition, scrollView, false, true);
				for (var i = 0; i < unusedAssets.Count; i++)
				{
					var asset = unusedAssets[i];
					if (asset != null)
					{
						var positionY = EditorGUIUtility.singleLineHeight * i;
						GUILayout.BeginHorizontal();
						if (asset.Obj != null)
						{
							GUI.Label(new Rect(10, positionY, 250, EditorGUIUtility.singleLineHeight), asset.Name);
							if (GUI.Button(new Rect(260, positionY, 70, EditorGUIUtility.singleLineHeight), "Select"))
							{
								Selection.activeObject = asset.Obj;
							}
							if (GUI.Button(new Rect(330, positionY, 70, EditorGUIUtility.singleLineHeight), "Delete"))
							{
								AssetDatabase.DeleteAsset(asset.Path);
							}
						}
						else
						{
							GUI.Label(new Rect(10, positionY, 250, EditorGUIUtility.singleLineHeight), asset.Path);
						}
						GUILayout.EndHorizontal();
					}
				}
				GUI.EndScrollView();
			}
		}

		private static IEnumerable<Asset> FindUnusedObjects(IEnumerable<string> assetList, ICollection<string> usedAssets)
		{
			return (from assetPath in assetList
				where !string.IsNullOrEmpty(assetPath) && !IsEditorPath(assetPath) && !usedAssets.Contains(assetPath)
				select new Asset(assetPath, AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object))))
				.ToList();
		}

		private static bool IsEditorPath(string path)
		{
			return path.IndexOf("/editor/", StringComparison.CurrentCultureIgnoreCase) > 0;
		}

		private static string[] FindAllProjectAssets()
		{
			// Find all assets in the project
			string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
			// Filter assets
			string[] tmpAssets2 = FilterAssets(tmpAssets1);
			// Remove scene files to create the final list of assets.
			string[] allAssets = Array.FindAll(tmpAssets2, name => !name.EndsWith(".unity"));

			// Convert directory paths to asset paths
			for (var i = 0; i < allAssets.Length; i++)
			{
				allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets", StringComparison.Ordinal) + 1);
				allAssets[i] = allAssets[i].Replace(@"\", "/");
			}

			return allAssets.ToArray();
		}

		private static string[] FilterAssets(string[] assets)
		{
			var filteredAssets = new List<string>();
			foreach (var asset in assets)
			{
				var isValid = !asset.EndsWith(".meta");

				if (!includeDuck)
				{
					isValid &= !asset.Contains("DUCK");
				}
				if (!includePlugins)
				{
					isValid &= !asset.Contains("Plugins");
				}
				if (!includeVendor)
				{
					isValid &= !asset.Contains("Vendor");
				}

				if (isValid)
				{
					filteredAssets.Add(asset);
				}
			}
			return filteredAssets.ToArray();
		}

		private static List<string> FindUsedAssets()
		{
			var usedAssets = new List<string>();

			try
			{
				var fileStream = new FileStream(GetEditorLogPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				var streamReader = new StreamReader(fileStream);

				var foundAssetsList = false;
				var reachedEndOfAssetList = false;
				while (!streamReader.EndOfStream && !foundAssetsList)
				{
					var line = streamReader.ReadLine();
					if (line != null && line.Contains("Used Assets and files from the Resources folder, sorted by uncompressed size:"))
					{
						foundAssetsList = true;
					}
				}

				while (!streamReader.EndOfStream && !reachedEndOfAssetList)
				{
					var line = streamReader.ReadLine();
					if (string.IsNullOrEmpty(line))
					{
						reachedEndOfAssetList = true;
					}
					else
					{
						var path = line.Substring(line.IndexOf("% ", StringComparison.Ordinal) + 2);
						usedAssets.Add(path);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Error: " + e);
			}

			return usedAssets.Count == 0 ? null : usedAssets;
		}

		private static string GetEditorLogPath()
		{
			var unityEditorLogfile = string.Empty;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				const string windowsLogFile = "\\Unity\\Editor\\Editor.log";
				unityEditorLogfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + windowsLogFile;
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				const string osxLogFile = "/Library/Logs/Unity/Editor.log";
				unityEditorLogfile = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + osxLogFile;
			}

			return unityEditorLogfile;
		}
	}
}