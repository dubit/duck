using UnityEngine;

namespace DUCK.Tween.Serialization
{
	[CreateAssetMenu(menuName = "Tween Config (Fancy™)", order = 230)]
	public class TweenConfigScriptableObject : ScriptableObject
	{
		[SerializeField]
		private TweenConfig config = new TweenConfig();
		public TweenConfig Config
		{
			get { return config; }
			set { config = value; }
		}
	}
}