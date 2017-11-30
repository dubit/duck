using System;
using System.Linq;
using UnityEngine;

namespace DUCK.Tween.Serialization
{
	public class TweenBuilder : MonoBehaviour
	{
		private const string NULL_COMPONENT_ERROR = "Cannot build tween for {0}. Argument {1} ({2}) was not serialized and could not find the component on the TweenBuilder";

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
			var animation = BuildTween(config, gameObject);
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

		public static AbstractAnimation BuildTween(TweenConfig config, GameObject defaultTarget)
		{
			var creatorFunction = TweenCreatorFunctionStore.Get(config.CreatorFunctionKey);

			// For any null args that are game objects or components, let's try to use this object
			var args = config.Args.AllArgs.ToArray();

			for (int i = 0; i < args.Length; i++)
			{
				var argType = creatorFunction.RealArgTypes[i];
				if (args[i] == null)
				{
					if (argType == typeof(GameObject))
					{
						args[i] = defaultTarget;
					}

					if (argType.IsSubclassOf(typeof(Component)))
					{
						var component = defaultTarget.GetComponent(argType);
						if (component == null)
						{
							throw new Exception(string.Format(NULL_COMPONENT_ERROR, config.CreatorFunctionKey, i, argType.Name));
						}
						args[i] = component;
					}
				}
			}

			var tween = creatorFunction.CreationMethod.Invoke(args);
			return tween;
		}
	}
}