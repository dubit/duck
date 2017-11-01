using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Utils
{
	public class FrameRateController : MonoBehaviour
	{
		[Header("Settings")]

		[SerializeField]
		private bool enableFrameRateCap;
		public bool EnableFrameRateCap
		{
			get { return enableFrameRateCap; }
			set
			{
				enableFrameRateCap = value;
				if (enableFrameRateCap) TargetFPS = targetFPS;
			}
		}

		[SerializeField]
		[Range(5, 300)]
		private int targetFPS = 30;
		public int TargetFPS
		{
			get { return targetFPS; }
			set
			{
				targetFPS = value;
				if (enableFrameRateCap) Application.targetFrameRate = targetFPS;
			}
		}

		[Space(10.0f)]

		[Header("On Screen FPS")]

		[SerializeField]
		private Text fpsText;

		[SerializeField]
		private bool showFPS;
		public bool ShowFPS
		{
			get { return fpsText != null && showFPS; }
			set
			{
				showFPS = value;
				if (fpsText != null) fpsText.enabled = showFPS;
			}
		}

		[SerializeField]
		[Range(0.1f, 5.0f)]
		private float refreshRate = 0.5f;

		/// <summary>
		/// The Current FPS -- Note: This is not a real time value.
		/// The update based on the refresh rate.
		/// </summary>
		public float CurrentFPS { get; private set; }

		private int totalFrames;
		private float elapsedTime;

		private void Start()
		{
			RefreshSettings();
		}

		/// <summary>
		/// Apply the settings by invoke the setters
		/// This function was designed for an customised inspector so that all the settings will work in Editor runtime
		/// (In Editor, the SerializeField will not trigger any setters or getters)
		/// </summary>
		public void RefreshSettings()
		{
			EnableFrameRateCap = enableFrameRateCap;
			ShowFPS = showFPS;
			TargetFPS = targetFPS;
		}

		private void LateUpdate()
		{
			elapsedTime += Time.unscaledDeltaTime;
			++totalFrames;
			if (elapsedTime >= refreshRate)
			{
				CurrentFPS = totalFrames / elapsedTime;

				// Update the FPS to the text
				if (showFPS)
				{
					fpsText.text = string.Format("FPS: {0:0.#}", CurrentFPS);
				}
				elapsedTime = totalFrames = 0;
			}
		}
	}
}