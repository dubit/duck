using DUCK.Editor;
using UnityEditor;

namespace DUCK.Http.Editor
{
	public static class CreateHttpConfigMenu
	{
		[MenuItem("Create/Http/Config")]
		private static void CreateHttpConfig()
		{
			ScriptableObjectUtility.CreateAsset<HttpConfig>();
		}
	}
}