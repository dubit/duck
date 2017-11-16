using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace DUCK.IOS
{
	public class AddIOSPermissions
	{
		[PostProcessBuild]
		public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				// Get plist
				string plistPath = pathToBuiltProject + "/Info.plist";
				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));

				// Get root
				PlistElementDict rootDict = plist.root;

				var guids = AssetDatabase.FindAssets("t:IOSPermissionsObject");
				foreach (string guid in guids)
				{
					var permissionsFile = AssetDatabase.LoadAssetAtPath<IOSPermissionsObject>(AssetDatabase.GUIDToAssetPath(guid));

					foreach(var permission in permissionsFile.Permissions)
					{
						rootDict.SetString(permission.key, permission.value);
					}
				}

				// Write to file
				File.WriteAllText(plistPath, plist.WriteToString());
			}
		}
	}
}