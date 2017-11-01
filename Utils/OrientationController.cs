using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DUCK.Utils
{
	public class OrientationConfig
	{
		public bool AllowPortrait { get; private set; }
		public bool AllowPortraitUpsideDown { get; private set; }
		public bool AllowLandscapeRight { get; private set; }
		public bool AllowLandscapeLeft { get; private set; }

		/// <summary>
		/// Sets up the config values for a particular allowed orientation.
		/// If more then one orientation is allowed then the device will be set to auto rotate as needed
		/// If just one is chosen device is locked to that orientation
		/// </summary>
		/// <param name="allowPortrait"></param>
		/// <param name="allowPortraitUpsideDown"></param>
		/// <param name="allowLandscapeLeft"></param>
		/// <param name="allowLandscapeRight"></param>
		public OrientationConfig(bool allowPortrait, bool allowPortraitUpsideDown, bool allowLandscapeLeft, bool allowLandscapeRight)
		{
			AllowPortrait = allowPortrait;
			AllowPortraitUpsideDown = allowPortraitUpsideDown;
			AllowLandscapeLeft = allowLandscapeLeft;
			AllowLandscapeRight = allowLandscapeRight;

			if (!AllowPortrait && !AllowPortraitUpsideDown && !AllowLandscapeLeft && !AllowLandscapeRight)
			{
				throw new Exception("One orientation must be allowed");
			}
		}
	}

	/// <summary>
	/// Sets the orientation on a per scene basis.
	/// This happens automatically when a new scene has been loaded
	/// If scene that is loaded not on list default is used.
	/// Default can be changed
	/// </summary>
	public class OrientationController : GenericSingleton<OrientationController>
	{
		private const float IOS_WAIT_TIME = 1.0f;

		private Dictionary<string, OrientationConfig> orientations;
		private OrientationConfig defaultOrientation;
		private int currentAllowedOrientations;
		private ScreenOrientation currentInitialOrientation;

		private OrientationController()
		{
			MonoBehaviourService.Instance.OnLevelLoaded += LevelLoadedHandler;
			orientations = new Dictionary<string, OrientationConfig>();
			defaultOrientation = new OrientationConfig(true, true, true, true);
		}

		public void ChangeDefault(OrientationConfig newDefault)
		{
			if (newDefault != null)
			{
				defaultOrientation = newDefault;
			}
			else
			{
				throw new ArgumentNullException("newDefault");
			}
		}

		public void AddOrientation(string sceneName, OrientationConfig newOrientation)
		{
			if (newOrientation == null)
			{
				throw new ArgumentNullException("newOrientation");
			}

			if (!orientations.ContainsKey(sceneName))
			{
				orientations.Add(sceneName, newOrientation);
			}
			else
			{
				throw new Exception("Cannot add orientation for scene: " + sceneName + " because it already has one specified");
			}
		}

		private void LevelLoadedHandler()
		{
			var sceneName = SceneManager.GetActiveScene().name;
			var targetOrientation = orientations.ContainsKey(sceneName) ? orientations[sceneName] : defaultOrientation;

			currentAllowedOrientations = 0;
			currentInitialOrientation = ScreenOrientation.AutoRotation;

			CheckOrientation(targetOrientation.AllowLandscapeLeft, ScreenOrientation.LandscapeLeft);
			CheckOrientation(targetOrientation.AllowPortraitUpsideDown, ScreenOrientation.PortraitUpsideDown);
			CheckOrientation(targetOrientation.AllowLandscapeRight, ScreenOrientation.LandscapeRight);
			CheckOrientation(targetOrientation.AllowPortrait, ScreenOrientation.Portrait);

			//if all orientations are allowed then dont bother changing orientation to begin with
			if (currentAllowedOrientations != 4)
			{
				Screen.orientation = currentInitialOrientation;
			}

			//Setting two orientations in same frame on IOS does not work, we have to wait a small of amount of time to set another.
#if UNITY_IOS
			MonoBehaviourService.Instance.StartCoroutine(WaitThenUpdateAutoRotation (targetOrientation));
#else
			SetOrientationToAutoRotate(targetOrientation);
#endif
		}

		private IEnumerator WaitThenUpdateAutoRotation(OrientationConfig config)
		{
			yield return new WaitForSeconds(IOS_WAIT_TIME);
			SetOrientationToAutoRotate(config);
		}

		private void CheckOrientation(bool allowed, ScreenOrientation orientation)
		{
			if (allowed)
			{
				currentAllowedOrientations++;
				currentInitialOrientation = orientation;
			}
		}

		private void SetOrientationToAutoRotate(OrientationConfig config)
		{
			SetAutoRotateSettings(config);
			Screen.orientation = ScreenOrientation.AutoRotation;
		}

		private void SetAutoRotateSettings(OrientationConfig config)
		{
			Screen.autorotateToPortrait = config.AllowPortrait;
			Screen.autorotateToPortraitUpsideDown = config.AllowPortraitUpsideDown;
			Screen.autorotateToLandscapeLeft = config.AllowLandscapeLeft;
			Screen.autorotateToLandscapeRight = config.AllowLandscapeRight;
		}
	}
}
