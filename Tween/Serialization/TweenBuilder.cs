using System.Linq;
using UnityEngine;

namespace DUCK.Tween.Serialization
{
	public class TweenBuilder : MonoBehaviour
	{
		[SerializeField]
		private bool playOnStart;

		[SerializeField]
		private bool useReferencedConfig;

		[SerializeField]
		private TweenConfigScriptableObject tweenConfigScriptableObject;

		[SerializeField]
		private TweenConfig tweenConfig = new TweenConfig();
		public TweenConfig TweenConfig
		{
			get { return tweenConfig; }
			set { tweenConfig = value; }
		}

		private void Start()
		{
			if (playOnStart)
			{
				Play();
			}
		}

		public void Play()
		{
			var config = useReferencedConfig ? tweenConfigScriptableObject.Config : TweenConfig;
			var creatorFunction = TweenCreatorFunctionStore.Get(config.CreatorFunctionKey);
			
			// TODO: catch null targets and try to use this object

			var animation = creatorFunction.CreationMethod.Invoke(config.Args.AllArgs.ToArray());
			animation.Play();
		}

#if UNITY_EDITOR
		public bool PlayOnStart
		{
			get { return playOnStart; }
			set { playOnStart = value; }
		}

		public bool UseReferencedConfig
		{
			get { return useReferencedConfig; }
			set { useReferencedConfig = value; }
		}

		public TweenConfigScriptableObject TweenConfigScriptableObject
		{
			get { return tweenConfigScriptableObject; }
			set { tweenConfigScriptableObject = value; }
		}
#endif
	}
}