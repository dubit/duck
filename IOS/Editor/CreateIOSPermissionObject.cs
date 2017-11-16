using DUCK.Editor;

namespace DUCK.IOS
{
	public class CreateIOSPermissionsObject
	{
		[MenuItem("Assets/Create/IOSPermissionsObject")]
		public static void CreateIOSPermissionsObject()
		{
			ScriptableObjectUtility.CreateAsset<IOSPermissionsObject>();
		}
	}
}
