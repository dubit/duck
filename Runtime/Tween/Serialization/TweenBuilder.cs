using System;
using System.Linq;
using DUCK.Serialization;
using UnityEngine;

namespace DUCK.Tween.Serialization
{
	public class TweenBuilder : MonoBehaviour
	{
		private const string NULL_COMPONENT_ERROR =
				"Cannot build tween for {0}. Argument {1} ({2}) was not serialized and could not find the component on the TweenBuilder"
			;

		[SerializeField]
		private bool playOnStart;

		[SerializeField]
		private bool playOnEnable;

		[SerializeField]
		private bool repeat;

		[SerializeField]
		private int repeatCount;

		[SerializeField]
		private bool useReferencedConfig;

		[SerializeField]
		private TweenConfig tweenConfig;

		[SerializeField]
		private ObjectCreationConfig objectCreationConfig = new ObjectCreationConfig();
		public ObjectCreationConfig ObjectCreationConfig
		{
			get { return objectCreationConfig; }
			set { objectCreationConfig = value; }
		}

		private void Start()
		{
			if (playOnStart)
			{
				Play();
			}
		}

		private void OnEnable()
		{
			if (playOnEnable)
			{
				Play();
			}
		}

		public void Play()
		{
			var config = useReferencedConfig ? tweenConfig.Config : ObjectCreationConfig;
			var tween = BuildTween(config, gameObject);
			if (tween != null)
			{
				if (repeat)
				{
					tween.Play(repeatCount);
				}
				else
				{
					tween.Play();
				}
			}
		}

#if UNITY_EDITOR
		public bool PlayOnStart
		{
			get { return playOnStart; }
			set { playOnStart = value; }
		}

		public bool PlayOnEnable
		{
			get { return playOnEnable; }
			set { playOnEnable = value; }
		}

		public bool Repeat
		{
			get { return repeat; }
			set { repeat = value; }
		}

		public int RepeatCount
		{
			get { return repeatCount; }
			set { repeatCount = value; }
		}

		public bool UseReferencedConfig
		{
			get { return useReferencedConfig; }
			set { useReferencedConfig = value; }
		}

		public TweenConfig TweenConfig
		{
			get { return tweenConfig; }
			set { tweenConfig = value; }
		}
#endif

		public static AbstractAnimation BuildTween(ObjectCreationConfig config, GameObject defaultTarget)
		{
			var creatorFunctions = TweenCreatorFunctions.Store;

			var creatorFunctionKey = config.CreatorFunctionKey;
			if (!creatorFunctions.Exists(creatorFunctionKey))
			{
				Debug.LogError("Cannot build tween: Creator function `" + creatorFunctionKey + "` could not be found!");
				return null;
			}

			var creatorFunction = creatorFunctions[creatorFunctionKey];

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

			return creatorFunction.Invoke(args);
		}
	}
}