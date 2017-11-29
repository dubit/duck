using System;
using System.Collections.Generic;
using System.Linq;
using DUCK.Serialization;
using DUCK.Serialization.Editor;
using DUCK.Tween.Easings;
using DUCK.Utils.Editor.EditorGUIHelpers;

namespace DUCK.Tween.Serialization.Editor
{
	public class TweenConfigEditor
	{
		public static void Draw(TweenConfig tweenConfig)
		{
			ArgsList args = tweenConfig.Args;
			
			tweenConfig.CreatorFunctionKey = EditorGUILayoutHelpers.OptionSelectorField(
				"Creator Function",
				tweenConfig.CreatorFunctionKey,
				TweenCreatorFunctionStore.Keys);

			if (string.IsNullOrEmpty(tweenConfig.CreatorFunctionKey))
			{
				// TODO: draw warning
				return;
			}

			var creatorFunction = TweenCreatorFunctionStore.Get(tweenConfig.CreatorFunctionKey);
			// TODO: catch if function cannot be found

			if (tweenConfig.Args == null) tweenConfig.Args = args = new ArgsList();
			args.SetTypes(creatorFunction.SerializedArgTypes);

			var indexOfEasingArg = creatorFunction.RealArgTypes.IndexOf(typeof(Func<float, float>));
			var customDrawFuncs = new Dictionary<int, ArgsListEditor.CustomArgDrawFunc>();
			if (indexOfEasingArg >= 0)
			{
				customDrawFuncs[indexOfEasingArg] = DrawEasingSelector;
			}

			ArgsListEditor.Draw(args, creatorFunction.ArgNames, customDrawFuncs);
		}

		private static object DrawEasingSelector(string label, object value)
		{
			var options = EasingFunctionStore.EasingFunctions.Keys.ToArray();
			return EditorGUILayoutHelpers.OptionSelectorField(label, (string) value, options);
		}
	}
}