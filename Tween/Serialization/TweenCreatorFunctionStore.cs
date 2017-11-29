using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DUCK.Tween.Easings;
using DUCK.Tween.Extensions;
using DUCK.Utils;

namespace DUCK.Tween.Serialization
{
	/// <summary>
	/// Holds info about functions that can be used to create tweens.
	/// This is used in conjunction with the TweenBuilder to provide editor gui to create tweens at editor time.
	/// </summary>
	public static class TweenCreatorFunctionStore
	{
		public static string[] Keys
		{
			get { return tweenCreatorFunctionInfos.Keys.ToArray(); }
		}

		private static readonly Dictionary<string, TweenCreatorFunctionInfo> tweenCreatorFunctionInfos;

		static TweenCreatorFunctionStore()
		{
			tweenCreatorFunctionInfos = new Dictionary<string, TweenCreatorFunctionInfo>();

			// Grab all constructors of listed types and use these.
			new[]
			{
				typeof(MoveAnimation),
				typeof(RotateAnimation),
				typeof(ScaleAnimation),
				typeof(RendererColorFadeAnimation),
				typeof(UIColorFadeAnimation),
				typeof(RendererFadeAnimation),
				typeof(UIFadeAnimation),
			}.ForEach(t =>
			{
				t.GetConstructors().ForEach(c =>
				{
					var key = StringifyConstructor(c);
					var func = TweenCreatorFunctionInfo.FromMethod(c);
					tweenCreatorFunctionInfos.Add(key, func);
				});
			});

			// Grab all extension methods of the following types
			new[]
			{
				typeof(TransformAnimationExtensions),
				typeof(RectTransformAnimationExtensions),
				typeof(RendererAnimationExtensions),
				typeof(UIComponentAnimationExtensions),
			}.ForEach(t =>
			{
				t.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.ForEach(m =>
					{
						var key = StringifyExtMethod(m);
						var func = TweenCreatorFunctionInfo.FromMethod(m);
						tweenCreatorFunctionInfos.Add(key, func);
					});
			});
		}

		public static TweenCreatorFunctionInfo Get(string key)
		{
			return tweenCreatorFunctionInfos.ContainsKey(key) ? tweenCreatorFunctionInfos[key] : null;
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
	}

	public class TweenCreatorFunctionInfo
	{
		public List<Type> RealArgTypes { get; private set; }
		public List<Type> SerializedArgTypes { get; private set; }
		public List<string> ArgNames { get; private set; }
		public Func<object[], AbstractAnimation> CreationMethod { get; private set; }

		public static TweenCreatorFunctionInfo FromMethod(MethodBase method)
		{
			var info = new TweenCreatorFunctionInfo();
			var parameterInfos = method.GetParameters();
			info.ArgNames = parameterInfos.Select(a => a.Name).ToList();
			info.RealArgTypes = parameterInfos.Select(a => a.ParameterType).ToList();
			info.SerializedArgTypes = parameterInfos.Select(a => a.ParameterType).Select(MapArgTypes).ToList();
			info.CreationMethod = args => (AbstractAnimation) method.Invoke(null, MapArgs(method, args));
			return info;
		}

		private static object[] MapArgs(MethodBase func, object[] args)
		{
			var paramaterInfos = func.GetParameters();
			for (var i = 0; i < paramaterInfos.Length; i++)
			{
				// TODO: abstract this out so we can plug in any arg converters
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