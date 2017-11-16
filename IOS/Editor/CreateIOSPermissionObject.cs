using DUCK.Editor;
using UnityEditor;

namespace DUCK.IOS
{
	public class CreateIOSPermissionsObject
	{
		[MenuItem("Assets/Create/IOSPermissionsObject")]
		public static void CreateScriptableObject()
		{
			ScriptableObjectUtility.CreateAsset<IOSPermissionsObject>();
		}
	}
}
