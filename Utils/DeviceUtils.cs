using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace DUCK.Utils
{
	public static class DeviceUtils
	{
		/// <summary>
		/// Uses Unity-Android interop methods to determine if device can be considered a tablet.
		/// Returns true if device is defined as LARGE or XLARGE, false for other sizes or non-Android devices.
		/// Additional information at http://developer.android.com/reference/android/content/res/Configuration.html#screenLayout
		/// </summary>
		/// <returns></returns>
		public static bool IsAndroidTablet()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			var configObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
				.GetStatic<AndroidJavaObject>("currentActivity")
				.Call<AndroidJavaObject>("getResources")
				.Call<AndroidJavaObject>("getConfiguration");

			var configClass = new AndroidJavaClass("android.content.res.Configuration");
			var sizeMask = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_MASK");

			var undefined = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_UNDEFINED");
			var smallScreen = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_SMALL");
			var normalScreen = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_NORMAL");
			var largeScreen = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_LARGE");
			var xlargeScreen = configClass.GetStatic<int>("SCREENLAYOUT_SIZE_XLARGE");

			var size = configObject.Get<int>("screenLayout") & sizeMask;

			// Define tablet screens as being either large or xlarge
			if (size == largeScreen || size == xlargeScreen)
			{
				return true;
			}
#endif
			return false;
		}

		/// <summary>
		/// Returns true if device is identified in Unity as an iPad, false for other sizes or non-iOS devices.
		/// </summary>
		/// <returns></returns>
		public static bool IsIpad()
		{
#if UNITY_IOS
			return (Device.generation == DeviceGeneration.iPadUnknown ||
				Device.generation == DeviceGeneration.iPad1Gen ||
				Device.generation == DeviceGeneration.iPad2Gen ||
				Device.generation == DeviceGeneration.iPad3Gen ||
				Device.generation == DeviceGeneration.iPad4Gen ||
				Device.generation == DeviceGeneration.iPadMini1Gen ||
				Device.generation == DeviceGeneration.iPadMini2Gen ||
				Device.generation == DeviceGeneration.iPadMini3Gen ||
				Device.generation == DeviceGeneration.iPadMini4Gen ||
				Device.generation.ToString ().StartsWith ("iPad"));
#else
			return false;
#endif
		}

		/// <summary>
		/// Returns true if device is identified in Unity as an iPhone, false for other sizes or non-iOS devices.
		/// </summary>
		/// <returns></returns>
		public static bool IsIphone()
		{
#if UNITY_IOS
			return (Device.generation == DeviceGeneration.iPhoneUnknown ||
				Device.generation == DeviceGeneration.iPhone3G ||
				Device.generation == DeviceGeneration.iPhone3GS ||
				Device.generation == DeviceGeneration.iPhone4 ||
				Device.generation == DeviceGeneration.iPhone4S ||
				Device.generation == DeviceGeneration.iPhone5 ||
				Device.generation == DeviceGeneration.iPhone5C ||
				Device.generation == DeviceGeneration.iPhone5S ||
				Device.generation == DeviceGeneration.iPhone6 ||
				Device.generation == DeviceGeneration.iPhone6Plus ||
				Device.generation == DeviceGeneration.iPhone6S ||
				Device.generation == DeviceGeneration.iPhone6SPlus ||
				Device.generation.ToString().StartsWith("iPhone"));
#else
			return false;
#endif
		}

		public static string Platform
		{
			get
			{
				if (Application.isEditor)
				{
					return "Editor";
				}
				else
				{
					var platformName = Application.platform.ToString();

					if (platformName.ToLower().Contains("mac"))
					{
						return "Mac OSX";
					}
					else if (platformName.ToLower().Contains("windows"))
					{
						return ("Windows");
					}
					else if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						return IsIpad() ? "iPad" : "iPhone";
					}
					else
					{
						return platformName;
					}
				}
			}
		}

		public static string OsVersion
		{
			get
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					return SystemInfo.operatingSystem.ToString().Split('/')[0].Replace("Android OS ", "");
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return SystemInfo.operatingSystem.Replace("iPhone OS ", "");
				}
				return SystemInfo.operatingSystem;
			}
		}
	}
}