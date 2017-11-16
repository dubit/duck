using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace DUCK.IOS
{
	public class AddIOSPermissions
	{
		[PostProcessBuild]
		public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget != BuildTarget.iOS) return;
			
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
					switch(permission.permissionType)
					{
						case PermissionVariableType.Boolean:
							rootDict.SetBoolean(permission.key, bool.Parse(permission.value));
							break;
						case PermissionVariableType.String:
							rootDict.SetString(permission.key, permission.value);
							break;
						case PermissionVariableType.Integer:
							rootDict.SetInteger(permission.key, int.Parse(permission.value));
							break;
					}
				}
			}
			
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}
}