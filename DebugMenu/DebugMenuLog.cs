using DUCK.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.DebugMenu
{
	/// <summary>
	/// A script which manages a list of loge entries for display to QA / user
	/// </summary>
	public class DebugMenuLog : MonoBehaviour
	{
		private enum LogType
		{
			Default = 12171705,
			Warning = 16753920,
			Error = 16711680
		}

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private RectTransform container;
		
		[SerializeField]
		private RectTransform entryPrefab;

		[SerializeField]
		private Button clearButton;

		private static DebugMenuLog instance;

		private void Awake()
		{
			if (instance != null)
			{
				throw new Exception("Cannot have two instances of DebugMenuLog.");
			}

			instance = this;
			entryPrefab.gameObject.SetActive(false);
			clearButton.interactable = false;
		}

		private void OnEnable()
		{
			StartCoroutine(ScrollToBottom());
		}

		public static void Log(string text)
		{
			Log(text, LogType.Default);
		}

		public static void LogWarning(string text)
		{
			Log(text, LogType.Warning);
		}

		public static void LogError(string text)
		{
			Log(text, LogType.Error);
		}

		private static void Log(string text, LogType logType)
		{
			if (instance == null) return;

			instance.AddLogEntry(text, logType);
		}

		public void Clear()
		{
			foreach (var rectTransform in container.GetComponentsInChildren<RectTransform>())
			{
				if (rectTransform != container && rectTransform != entryPrefab)
				{
					Destroy(rectTransform.gameObject);
				}
			}

			clearButton.interactable = false;
		}

		private void AddLogEntry(string text, LogType logType)
		{
			clearButton.interactable = true;

			var newLogEntry = Instantiate<RectTransform>(entryPrefab, container, false);
			newLogEntry.transform.SetAsLastSibling();

			var textComponent = newLogEntry.GetComponent<Text>();
			textComponent.text = text;

			var iconComponent = newLogEntry.GetComponentInChildren<Image>();
			iconComponent.color = ColorUtils.IntToColor((int)logType);

			newLogEntry.gameObject.SetActive(true);

			StartCoroutine(ScrollToBottom());
		}

		private IEnumerator ScrollToBottom()
		{
			yield return new WaitForEndOfFrame();
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}
}