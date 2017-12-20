using System;
using System.Collections.Generic;
using System.Linq;
using DUCK.Serialization;
using DUCK.Serialization.Editor;
using DUCK.Tween.Easings;
using DUCK.Utils.Editor.EditorGUIHelpers;
using UnityEditor;
using UnityEngine;

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

			var creatorFunctionKey = tweenConfig.CreatorFunctionKey;

			if (string.IsNullOrEmpty(creatorFunctionKey))
			{
				EditorGUILayout.HelpBox("No creator function selected, nothing will be executed!", MessageType.Info);
				return;
			}

			if (!TweenCreatorFunctionStore.Exists(creatorFunctionKey))
			{
				var message = "Creator function with the key " + creatorFunctionKey + " could not be found!";
				EditorGUILayout.HelpBox(message, MessageType.Error);
				return;
			}

			var creatorFunction = TweenCreatorFunctionStore.Get(creatorFunctionKey);

			if (tweenConfig.Args == null) tweenConfig.Args = args = new ArgsList();
			args.SetTypes(creatorFunction.SerializedArgTypes);

			var indexOfEasingArg = creatorFunction.RealArgTypes.IndexOf(typeof(Func<float, float>));
			var customDrawFuncs = new Dictionary<int, ArgsListEditor.CustomArgDrawFunc>();
			if (indexOfEasingArg >= 0)
			{
				customDrawFuncs[indexOfEasingArg] = DrawEasingSelector;
			}

			ArgsListEditor.Draw(args, creatorFunction.ArgNames, customDrawFuncs);

			ReportWarningForAnyNullObjectReferences(args);
		}

		private static void ReportWarningForAnyNullObjectReferences(ArgsList args)
		{
			for (int i = 0; i < args.AllArgs.Count; i++)
			{
				var type = args.ArgTypes[i];
				if (type == typeof(GameObject) || type.IsSubclassOf(typeof(Component)))
				{
					if (args.AllArgs[i] == null)
					{
						var message = "TweenConfig contains one or more GameObject or Component references that are not set."
							 + "\r\nWhen attempting to build the tween, the TweenBuilder will use the current GameObject or attempt to find the correct components on the current GameObject";
						EditorGUILayout.HelpBox(message, MessageType.Info);
						return;
					}
				}
			}
		}

		private static object DrawEasingSelector(string label, object value)
		{
			var options = EasingFunctionStore.EasingFunctions.Keys.ToArray();
			return EditorGUILayoutHelpers.OptionSelectorField(label, (string) value, options);
		}
	}
}