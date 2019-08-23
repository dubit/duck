using System.Collections;
using System.Collections.Concurrent;
using DUCK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.DebugMenu
{
	/// <summary>
	/// Manages a list of log entries displayed in the debug menu
	/// </summary>
	public class DebugMenuLog : MonoBehaviour
	{
		private enum LogType
		{
			Default = 12171705,
			Warning = 16753920,
			Error = 16711680
		}

		private struct PendingLog
		{
			public readonly string LogMessage;
			public readonly LogType LogType;

			public PendingLog(string logMessage, LogType logType)
			{
				LogMessage = logMessage;
				LogType = logType;
			}
		}

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private RectTransform container;

		[SerializeField]
		private RectTransform entryPrefab;

		[SerializeField]
		private Button clearButton;

		private ConcurrentQueue<PendingLog> pendingLogs;

		private static DebugMenuLog instance;

		private void Awake()
		{
			entryPrefab.gameObject.SetActive(false);
			clearButton.interactable = false;

			Initialise();
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

		/// <summary>
		/// The logs are passed back to the main thread through a queue as instantiating the new log
		/// must be done on the main thread, however the queue itself is not properly concurrent.
		/// </summary>
		private static void Log(string text, LogType logType)
		{
			if (instance == null) return;

			instance.pendingLogs.Enqueue(new PendingLog(text, logType));
		}

		public void Initialise()
		{
			if (instance == null)
			{
				instance = this;
				pendingLogs = new ConcurrentQueue<PendingLog>();
			}
		}

		/// <summary>
		/// Called from a Clear button OnClick in the prefab
		/// </summary>
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

			var newLogEntry = Instantiate(entryPrefab, container, false);
			newLogEntry.transform.SetAsLastSibling();

			var textComponent = newLogEntry.GetComponent<Text>();
			textComponent.text = text;

			var iconComponent = newLogEntry.GetComponentInChildren<Image>();
			iconComponent.color = ColorUtils.IntToColor((int)logType);

			newLogEntry.gameObject.SetActive(true);
		}

		private IEnumerator ScrollToBottom()
		{
			yield return new WaitForEndOfFrame();
			scrollRect.verticalNormalizedPosition = 0f;
		}

		private void Update()
		{
			if (pendingLogs.IsEmpty) return;

			while (pendingLogs.Count > 0)
			{
				if (pendingLogs.TryDequeue(out var pendingLog))
				{
					AddLogEntry(pendingLog.LogMessage, pendingLog.LogType);
				}
			}

			StartCoroutine(ScrollToBottom());
		}
	}
}
