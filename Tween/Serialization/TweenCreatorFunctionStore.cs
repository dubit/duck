using System;
using System.Linq;
using System.Reflection;
using DUCK.Serialization;
using DUCK.Tween.Easings;
using DUCK.Tween.Extensions;
using DUCK.Utils;

namespace DUCK.Tween.Serialization
{
	public static class TweenCreatorFunctions
	{
		public static CreatorFunctionStore<AbstractAnimation> Store { get; private set; }

		private static Type[] constructorTypes =
		{
			typeof(MoveAnimation),
			typeof(RotateAnimation),
			typeof(ScaleAnimation),
			typeof(RendererColorFadeAnimation),
			typeof(UIColorFadeAnimation),
			typeof(RendererFadeAnimation),
			typeof(UIFadeAnimation),
		};

		private static Type[] extensionMethodTypes =
		{
			typeof(TransformAnimationExtensions),
			typeof(RectTransformAnimationExtensions),
			typeof(RendererAnimationExtensions),
			typeof(UIComponentAnimationExtensions),
		};

		static TweenCreatorFunctions()
		{
			Store = new CreatorFunctionStore<AbstractAnimation>();

			// Grab all constructors of listed types and use these.
			constructorTypes.ForEach(t =>
			{
				t.GetConstructors()
					.Where(MethodFilter)
					.ForEach(c =>
					{
						var key = StringifyConstructor(c);
						var func = new CreatorFunctionInfo<AbstractAnimation>(c, MapArgTypes, MapArgs);
						Store.Add(key, func);
					});
			});

			// Grab all extension methods of the following types
			extensionMethodTypes.ForEach(t =>
			{
				t.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.Where(MethodFilter)
					.ForEach(m =>
					{
						var key = StringifyExtMethod(m);
						var func = new CreatorFunctionInfo<AbstractAnimation>(m, MapArgTypes, MapArgs);
						Store.Add(key, func);
					});
			});
		}

		private static bool MethodFilter(MethodBase method)
		{
			var parameters = method.GetParameters();
			return parameters.All(p =>
			{
				// All types must supported by Args list or be a Func<float,float> (easing function)
				return ArgsList.IsSupportedType(p.ParameterType) || p.ParameterType == typeof(Func<float, float>);
			});
		}

		private static string StringifyConstructor(ConstructorInfo ctor)
		{
			var name = ctor.DeclaringType.Name;
			return ".ctor:" + name + "(" + StringifyMethodParams(ctor) + ")";
		}

		private static string StringifyExtMethod(MethodInfo method)
		{
			var name = method.Name;
			return ".ext:" + name + "(" + StringifyMethodParams(method) + ")";
		}

		private static string StringifyMethodParams(MethodBase method)
		{
			var parameters = method.GetParameters().Select(p => p.ParameterType.Name).ToArray();
			var paramsString = "";
			for (var i = 0; i < parameters.Length; i++)
			{
				paramsString += parameters[i] + (i == parameters.Length - 1 ? "" : ",");
			}
			return paramsString;
		}

		private static object[] MapArgs(MethodBase func, object[] args)
		{
			var paramaterInfos = func.GetParameters();
			for (var i = 0; i < paramaterInfos.Length; i++)
			{
				if (paramaterInfos[i].ParameterType == typeof(Func<float, float>))
				{
					var easingFunctionKey = (string) args[i];
					if (EasingFunctionStore.EasingFunctions.ContainsKey(easingFunctionKey))
					{
						args[i] = EasingFunctionStore.EasingFunctions[easingFunctionKey];
					}
					else
					{
						args[i] = (Func<float, float>) Ease.Linear.None;
					}
				}
			}

			return args;
		}

		private static Type MapArgTypes(Type type)
		{
			if (type == typeof(Func<float, float>))
			{
				return typeof(string);
			}
			return type;
		}
	}
}