using DUCK.Editor;
using UnityEditor;

namespace DUCK.Tween.Serialization.Editor
{
	public class CreateTweenConfigScriptableObject
	{
		[MenuItem("DUCK/Tween/Create Tween Config")]
		public static void CreateTweenConfig()
		{
			ScriptableObjectUtility.CreateAsset<TweenConfigScriptableObject>();
		}
	}
}