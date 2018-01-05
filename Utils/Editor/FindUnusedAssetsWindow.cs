using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Utils.Editor
{
	public class FindUnusedAssetsWindow : EditorWindow
	{
		private const string EDITOR_PREF_SEEN_WARNING_ASSETS = "warningAssets";
		private const string EDITOR_PREF_INCLUDE_VENDOR = "includeVendor";
		private const string EDITOR_PREF_INCLUDE_PLUGINS = "includePlugins";
		private const string EDITOR_PREF_INCLUDE_DUCK = "includeDuck";
		private const string EDITOR_PREF_INCLUDE_IGNORED = "includeIgnored";

		private class Asset
		{
			public string Extension { get; private set; }
			public string Path { get; private set; }
			public string Name { get; private set; }
			public Object Obj { get; private set; }
			public bool IsIgnored { get; set; }

			public Asset(string path, Object obj)
			{
				Path = path;
				Obj = obj;
				if (obj != null)
				{
					Name = obj.name;
				}
				var indexOfPoint = path.LastIndexOf('.');
				var length = path.Length - indexOfPoint;
				if (indexOfPoint >= 0 && length >= 0)
				{
					Extension = path.Substring(indexOfPoint, length);
				}
				else
				{
					Extension = "Unknown";
				}
			}
		}

		private static bool seenWarningAssets;
		private static bool includeVendor;
		private static bool includePlugins;
		private static bool includeDuck;
		private static bool includeIgnored;
		private static bool hasEditorLagBeenFound;
		private static Dictionary<string, List<Asset>> assetsByExtension;
		private static Asset[] unusedAssets;
		private static Vector2 scrollPosition;
		private static string helpMessage;
		private static FindUnusedAssetsWindow window;
		private static int gridSelectionIndex;
		private static string[] gridSelectionButtonNames;
		private static string searchValue;

		[MenuItem("DUCK/Find Unused Assets")]
		private static void Init()
		{
			window = (FindUnusedAssetsWindow) GetWindow(typeof(FindUnusedAssetsWindow));
			window.Show();
			window.titleContent = new GUIContent("Find Unused Assets");
			LoadFilters();
			hasEditorLagBeenFound = true;
		}

		[DidReloadScripts]
		private static void ReloadWindow()
		{
			LoadFilters();
			Search();
		}

		private static void Search()
		{
			List<string> usedAssets = FindUsedAssets();

			if (usedAssets != null)
			{
				unusedAssets = FindUnusedObjects(FindAllProjectAssets(), usedAssets).OrderBy(a => a.Obj != null).ToArray();
				assetsByExtension = new Dictionary<string, List<Asset>>();
				unusedAssets.ForEach(a =>
				{
					a.IsIgnored = IsAssetIgnored(a);
					if (!assetsByExtension.ContainsKey(a.Extension))
					{
						assetsByExtension[a.Extension] = new List<Asset>();
					}
					assetsByExtension[a.Extension].Add(a);
				});
				gridSelectionButtonNames = new string[assetsByExtension.Keys.Count + 1];
				gridSelectionButtonNames[0] = string.Format("({0}) {1}", unusedAssets.Length, "All");
				for (var i = 0; i < assetsByExtension.Count; i++)
				{
					var element = assetsByExtension.ElementAt(i);
					gridSelectionButtonNames[i + 1] = string.Format("({0}) {1}", element.Value.Count, element.Key);
				}
				hasEditorLagBeenFound = true;
			}
			else
			{
				hasEditorLagBeenFound = false;
			}
		}

		private static void LoadFilters()
		{
			includeVendor = EditorPrefs.GetBool(EDITOR_PREF_INCLUDE_VENDOR);
			includePlugins = EditorPrefs.GetBool(EDITOR_PREF_INCLUDE_PLUGINS);
			includeDuck = EditorPrefs.GetBool(EDITOR_PREF_INCLUDE_DUCK);
			includeIgnored = EditorPrefs.GetBool(EDITOR_PREF_INCLUDE_IGNORED);
			seenWarningAssets = EditorPrefs.GetBool(EDITOR_PREF_SEEN_WARNING_ASSETS);
		}

		private static void SaveFilters()
		{
			EditorPrefs.SetBool(EDITOR_PREF_INCLUDE_VENDOR, includeVendor);
			EditorPrefs.SetBool(EDITOR_PREF_INCLUDE_PLUGINS, includePlugins);
			EditorPrefs.SetBool(EDITOR_PREF_INCLUDE_DUCK, includeDuck);
			EditorPrefs.SetBool(EDITOR_PREF_INCLUDE_IGNORED, includeIgnored);
		}

		private void OnGUI()
		{
			DrawNotifications();
			EditorGUILayout.HelpBox("Does not include Scenes or Meta data.", MessageType.Info);
			GUILayout.BeginHorizontal();
			includeDuck = GUILayout.Toggle(includeDuck, "Include DUCK");
			includePlugins = GUILayout.Toggle(includePlugins, "Include Plugins");
			includeVendor = GUILayout.Toggle(includeVendor, "Include Vendor");
			includeIgnored = GUILayout.Toggle(includeIgnored, "Include Ignored");

			if (GUILayout.Button("Search"))
			{
				Search();
				SaveFilters();
			}
			GUILayout.EndHorizontal();

			if (!hasEditorLagBeenFound)
			{
				EditorGUILayout.HelpBox(
					"Could not find Used Assets list in the Editor Log! - You need to build your project before doing this!",
					MessageType.Warning);
				EditorGUILayout.HelpBox("If this problem persists then perhaps there is a Unity version mismatch?",
					MessageType.Warning);
			}
			else if (unusedAssets != null)
			{
				// Draw total unused assets found.
				GUILayout.Label(string.Format("Found a total of {0} unused assets!", unusedAssets.Length), EditorStyles.boldLabel);
				// Draw Extension Tabs
				DrawExtensionTabs();
				var extensionIndex = gridSelectionIndex - 1;
				extensionIndex = Mathf.Clamp(extensionIndex, -1, assetsByExtension.Count - 1);
				var extension = extensionIndex >= 0 ? assetsByExtension.ElementAt(extensionIndex).Key : string.Empty;
				gridSelectionIndex = extensionIndex + 1;
				var assetsListToShow = string.IsNullOrEmpty(extension) ? unusedAssets : assetsByExtension[extension].ToArray();
				// Search bar
				DrawSearchBar();
				// Seperator
				GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(0.01f));
				// Draw assets in a scroll view
				scrollPosition = GUILayout.BeginScrollView(scrollPosition);
				foreach (var asset in assetsListToShow)
				{
					DrawAsset(asset);
				}
				GUILayout.EndScrollView();
			}
		}

		private static void DrawNotifications()
		{
			if (!seenWarningAssets)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(
					"Assets that are not directly referenced via an inspector but are used in a scene will not get used by the build but still might be required by the project as a template asset.",
					MessageType.Warning);
				if (GUILayout.Button("Confirm", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
				{
					EditorPrefs.SetBool(EDITOR_PREF_SEEN_WARNING_ASSETS, true);
					seenWarningAssets = true;
				}
				GUILayout.EndHorizontal();
			}
		}

		private static void DrawSearchBar()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Search", GUILayout.ExpandWidth(false));
			searchValue = GUILayout.TextField(searchValue, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		private static void IgnoreAsset(Asset asset, bool ignore)
		{
			asset.IsIgnored = ignore;
			EditorPrefs.SetBool(asset.Path, ignore);
		}

		private static bool IsAssetIgnored(Asset asset)
		{
			return EditorPrefs.HasKey(asset.Path) && EditorPrefs.GetBool(asset.Path);
		}

		private void DrawExtensionTabs()
		{
			gridSelectionIndex = GUILayout.SelectionGrid(gridSelectionIndex, gridSelectionButtonNames, 3);
			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		private static bool IsSearchMatch(string path)
		{
			if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(searchValue))
			{
				return true;
			}
			return path.Contains(searchValue);
		}

		private static void DrawAsset(Asset asset)
		{
			if (asset == null || !includeIgnored && asset.IsIgnored) return;
			if (!IsSearchMatch(asset.Path)) return;

			GUILayout.BeginHorizontal();
			if (asset.Obj != null)
			{
				var originalColor = GUI.color;
				if (asset.IsIgnored)
				{
					GUI.color = Color.green;
				}
				GUILayout.Label(asset.Name);
				GUI.color = originalColor;
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(asset.IsIgnored ? "Unignore" : "Ignore"))
				{
					IgnoreAsset(asset, !asset.IsIgnored);
				}
				if (GUILayout.Button("Select"))
				{
					Selection.activeObject = asset.Obj;
				}
				if (GUILayout.Button("Delete"))
				{
					AssetDatabase.DeleteAsset(asset.Path);
					Search();
				}
			}
			else
			{
				GUILayout.Label(asset.Path);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(0.1f);
			GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(0.01f));
		}

		private static IEnumerable<Asset> FindUnusedObjects(IEnumerable<string> assetList, ICollection<string> usedAssets)
		{
			return from assetPath in assetList
				where !string.IsNullOrEmpty(assetPath) && !IsEditorPath(assetPath) && !usedAssets.Contains(assetPath)
				select new Asset(assetPath, AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
		}

		private static bool IsEditorPath(string path)
		{
			return path.IndexOf("/editor/", StringComparison.CurrentCultureIgnoreCase) > 0;
		}

		private static string[] FindAllProjectAssets()
		{
			// Find all assets in the project
			string[] allAssets = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
			// Filter assets
			allAssets = FilterAssets(allAssets);
			// Remove scene files to create the final list of assets.
			allAssets = Array.FindAll(allAssets, name => !name.EndsWith(".unity"));

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
					isValid &= !asset.Contains("duck");
					isValid &= !asset.Contains("Duck");
				}
				if (!includePlugins)
				{
					isValid &= !asset.Contains("Plugins");
				}
				if (!includeVendor)
				{
					isValid &= !asset.Contains("Vendor");
				}
				if (!includeVendor)
				{
					isValid &= !asset.Contains(".git");
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