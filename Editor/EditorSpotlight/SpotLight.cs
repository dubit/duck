using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

/*
 * Based on github.com/marijnz/unity-editor-spotlight/
 */

/* TODO:
 *	* Different sorting mechanisms
 *	* Fuzzy search
 *	* Asset type search
 *	* different shortcuts for ping, open, select
 *	* Scroll down through more than 10 hits
 */

namespace DUCK.Editor.EditorSpotlight
{
	public class SpotLight : EditorWindow, IHasCustomMenu
	{
		private const string PLACEHOLDER_INPUT = "Open Asset...";
		private const string SEARCH_HISTORY_KEY = "SearchHistoryKey";
		private const int BASE_HEIGHT = 90;

		[MenuItem("Window/Spotlight %#k")]
		private static void Init()
		{
			var window = CreateInstance<SpotLight>();
			window.titleContent = new GUIContent("Spotlight");
			var pos = window.position;
			pos.height = BASE_HEIGHT;
			pos.xMin = Screen.currentResolution.width / 2 - 500 / 2;
			pos.yMin = Screen.currentResolution.height * .3f;
			window.position = pos;
			window.EnforceWindowSize();
			window.ShowUtility();

			window.Reset();
		}

		[Serializable]
		private class SearchHistory : ISerializationCallbackReceiver
		{
			public Dictionary<string, int> Clicks { get; private set; }

			[SerializeField]
			private List<string> clickKeys = new List<string>();

			[SerializeField]
			private List<int> clickValues = new List<int>();

			public SearchHistory()
			{
				Clicks = new Dictionary<string, int>();
			}

			public void OnBeforeSerialize()
			{
				clickKeys.Clear();
				clickValues.Clear();

				foreach (var pair in Clicks)
				{
					clickKeys.Add(pair.Key);
					clickValues.Add(pair.Value);
				}
			}

			public void OnAfterDeserialize()
			{
				Clicks.Clear();
				for (var i = 0; i < clickKeys.Count; i++)
				{
					Clicks.Add(clickKeys[i], clickValues[i]);
				}
			}
		}

		private List<string> hits = new List<string>();
		private string input;
		private int selectedIndex = 0;
		private SearchHistory history;

		private void Reset()
		{
			input = "";
			hits.Clear();
			if (EditorPrefs.HasKey(SEARCH_HISTORY_KEY))
			{
				var json = EditorPrefs.GetString(SEARCH_HISTORY_KEY);
				history = JsonUtility.FromJson<SearchHistory>(json);
			}
			else
			{
				history = new SearchHistory();
			}

			Focus();
		}

		private void OnLostFocus()
		{
			Close();
		}

		private void OnGUI()
		{
			EnforceWindowSize();
			HandleEvents();

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			GUILayout.BeginVertical();
			GUILayout.Space(15);

			GUI.SetNextControlName("SpotlightInput");
			var prevInput = input;
			input = GUILayout.TextField(input, Styles.inputFieldStyle, GUILayout.Height(60));
			EditorGUI.FocusTextInControl("SpotlightInput");

			if (input != prevInput)
			{
				ProcessInput();
			}

			selectedIndex = Mathf.Clamp(selectedIndex, 0, hits.Count - 1);

			if (string.IsNullOrEmpty(input))
			{
				GUI.Label(GUILayoutUtility.GetLastRect(), PLACEHOLDER_INPUT, Styles.placeholderStyle);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(6);

			VisualizeHits();

			GUILayout.Space(6);
			GUILayout.EndHorizontal();

			GUILayout.Space(15);
			GUILayout.EndVertical();
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void ProcessInput()
		{
			input = input.ToLower();
			if (string.IsNullOrEmpty(input))
			{
				hits = new List<string>();
				return;
			}

			var assetHits = AssetDatabase.FindAssets(input) ?? new string[0];
			hits = assetHits.ToList();

			// Sort the search hits
			hits.Sort((x, y) =>
			{
				// Generally, use click history
				int xScore;
				history.Clicks.TryGetValue(x, out xScore);
				int yScore;
				history.Clicks.TryGetValue(y, out yScore);

				// Value files that actually begin with the search input higher
				if (xScore != 0 && yScore != 0)
				{
					var xName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(x)).ToLower();
					var yName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(y)).ToLower();
					if (xName.StartsWith(input) && !yName.StartsWith(input))
						return -1;
					if (!xName.StartsWith(input) && yName.StartsWith(input))
						return 1;
				}

				return yScore - xScore;
			});

			hits = hits.Take(10).ToList();
		}

		private void HandleEvents()
		{
			var current = Event.current;

			if (current.type == EventType.KeyDown)
			{
				if (current.keyCode == KeyCode.UpArrow)
				{
					current.Use();
					selectedIndex--;
				}
				else if (current.keyCode == KeyCode.DownArrow)
				{
					current.Use();
					selectedIndex++;
				}
				else if (current.keyCode == KeyCode.Return)
				{
					OpenSelectedAssetAndClose();
					current.Use();
				}
				else if (Event.current.keyCode == KeyCode.Escape)
				{
					Close();
				}
			}
		}

		private void VisualizeHits()
		{
			var current = Event.current;

			var windowRect = position;
			windowRect.height = BASE_HEIGHT;

			GUILayout.BeginVertical();
			GUILayout.Space(5);

			if (hits.Count == 0)
			{
				windowRect.height += EditorGUIUtility.singleLineHeight;
				if (!string.IsNullOrEmpty(input))
				{
					GUILayout.Label("No hits");
				}
			}

			for (var i = 0; i < hits.Count; i++)
			{
				var style = i % 2 == 0 ? Styles.entryOdd : Styles.entryEven;

				GUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2),
					GUILayout.ExpandWidth(true));

				var elementRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				GUILayout.EndHorizontal();

				windowRect.height += EditorGUIUtility.singleLineHeight * 2;

				if (current.type == EventType.Repaint)
				{
					style.Draw(elementRect, false, false, i == selectedIndex, false);
					var assetPath = AssetDatabase.GUIDToAssetPath(hits[i]);
					var icon = AssetDatabase.GetCachedIcon(assetPath);

					var iconRect = elementRect;
					iconRect.x = 30;
					iconRect.width = 25;
					GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

					var assetName = Path.GetFileName(assetPath);
					var coloredAssetName = new StringBuilder();

					var start = assetName.ToLower().IndexOf(input);
					var end = start + input.Length;

					var highlightColor = EditorGUIUtility.isProSkin
						? Styles.PRO_SKIN_HIGHLIGHT_COLOR
						: Styles.PERSONAL_SKIN_HIGHLIGHT_COLOR;

					var normalColor = EditorGUIUtility.isProSkin
						? Styles.PRO_SKIN_NORMAL_COLOR
						: Styles.PERSONAL_SKIN_NORMAL_COLOR;

					// Sometimes the AssetDatabase finds assets without the search input in it.
					if (start == -1)
					{
						coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>", normalColor, assetName));
					}
					else
					{
						if (0 != start)
							coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>",
								normalColor, assetName.Substring(0, start)));

						coloredAssetName.Append(
							string.Format("<color=#{0}><b>{1}</b></color>", highlightColor, assetName.Substring(start, end - start)));

						if (end != assetName.Length - end)
						{
							coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>",
								normalColor, assetName.Substring(end, assetName.Length - end)));
						}
					}

					var labelRect = elementRect;
					labelRect.x = 60;
					GUI.Label(labelRect, coloredAssetName.ToString(), Styles.resultLabelStyle);
				}

				if (current.type == EventType.MouseDown && elementRect.Contains(current.mousePosition))
				{
					selectedIndex = i;
					if (current.clickCount == 2)
					{
						OpenSelectedAssetAndClose();
					}
					else
					{
						Selection.activeObject = GetSelectedAsset();
						EditorGUIUtility.PingObject(Selection.activeGameObject);
					}

					Repaint();
				}
			}

			windowRect.height += 5;
			position = windowRect;

			GUILayout.EndVertical();
		}

		private void OpenSelectedAssetAndClose()
		{
			Close();

			// Selection.activeObject = GetSelectedAsset();
			//EditorGUIUtility.PingObject(GetSelectedAsset());
			//AssetDatabase.OpenAsset(GetSelectedAsset());

			var guid = hits[selectedIndex];
			if (!history.Clicks.ContainsKey(guid))
				history.Clicks[guid] = 0;

			history.Clicks[guid]++;
			EditorPrefs.SetString(SEARCH_HISTORY_KEY, JsonUtility.ToJson(history));
		}

		private UnityEngine.Object GetSelectedAsset()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(hits[selectedIndex]);
			return (AssetDatabase.LoadMainAssetAtPath(assetPath));
		}

		private void EnforceWindowSize()
		{
			var pos = position;
			pos.width = 500;
			position = pos;
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Reset history"), false, () =>
			{
				EditorPrefs.SetString(SEARCH_HISTORY_KEY, JsonUtility.ToJson(new SearchHistory()));
				Reset();
			});

			menu.AddItem(new GUIContent("Output history"), false, () =>
			{
				var json = EditorPrefs.GetString(SEARCH_HISTORY_KEY, JsonUtility.ToJson(new SearchHistory()));
				Debug.Log(json);
			});
		}
	}
}