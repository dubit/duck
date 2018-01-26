using DUCK.Serialization;
using UnityEngine;

namespace DUCK.Tween.Serialization
{
	[CreateAssetMenu(menuName = "Tween Config (Fancy™)", order = 230)]
	public class TweenConfig : ScriptableObject
	{
		[SerializeField]
		private ObjectCreationConfig config = new ObjectCreationConfig();
		public ObjectCreationConfig Config
		{
			get { return config; }
			set { config = value; }
		}
	}
}