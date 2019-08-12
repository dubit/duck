#if UNITY_EDITOR && ENABLE_BUILD_VERSION
using UnityEditor;
using UnityEditor.Build;
#endif
using UnityEngine;

namespace DUCK.Utils
{
	public class BuildVersion : ScriptableObject
	{
		internal const string ASSET_NAME = "BuildVersion";

		[SerializeField]
		private string dateStamp = "01/01/1900";

#if UNITY_EDITOR && ENABLE_BUILD_VERSION
		public void UpdateForNewBuild()
		{
			dateStamp = System.DateTime.Now.ToString("dd/MM/yyyy");
		}
#endif

		private static BuildVersion buildVersion;
		public static BuildVersion GetBuildVersion()
		{
			return buildVersion ?? (buildVersion = Resources.Load(ASSET_NAME) as BuildVersion ?? CreateInstance<BuildVersion>());
		}

		public override string ToString()
		{
			return dateStamp;
		}
	}

#if UNITY_EDITOR && ENABLE_BUILD_VERSION
	public class BuildVersionAutoIncrement : IPreprocessBuild
	{
		public int callbackOrder { get { return 0; } }

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			const string root = "Assets";
			const string folder = "Resources";
			const string folderPath = root + "/" + folder;
			const string fullPath = folderPath + "/" + BuildVersion.ASSET_NAME + ".asset";

			var buildVersion = ScriptableObject.CreateInstance<BuildVersion>();
			buildVersion.UpdateForNewBuild();

			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				AssetDatabase.CreateFolder(root, folder);
			}

			AssetDatabase.DeleteAsset(fullPath);
			AssetDatabase.CreateAsset(buildVersion, fullPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
#endif
}