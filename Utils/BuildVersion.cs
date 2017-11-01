#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif
using UnityEngine;

namespace DUCK.Utils
{
	public class BuildVersion : ScriptableObject
	{
		internal const string AssetPath = "BuildVersion";

		public string DateStamp = "debug/Editor";

#if UNITY_EDITOR
		public void UpdateForNewBuild()
		{
			DateStamp = System.DateTime.Now.ToString("dd/MM/yyyy");
		}
#endif

		private static BuildVersion buildVersion;
		public static BuildVersion GetBuildVersion()
		{
			if (buildVersion == null)
			{
				buildVersion = (Resources.Load(AssetPath) as BuildVersion) ?? CreateInstance<BuildVersion>();
			}

			return buildVersion;
		}

		public override string ToString()
		{
			return DateStamp;
		}
	}

#if UNITY_EDITOR
	public class BuildVersionAutoIncrement : IPreprocessBuild
	{
		public int callbackOrder { get { return 0; } }

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			const string root = "Assets";
			const string folder = "Resources";
			const string folderPath = root + "/" + folder + "/";
			const string fullPath = folderPath + BuildVersion.AssetPath + ".asset";

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