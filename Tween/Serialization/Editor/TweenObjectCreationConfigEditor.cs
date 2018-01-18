using System;
using System.Linq;
using DUCK.Serialization;
using DUCK.Serialization.Editor;
using DUCK.Tween.Easings;
using DUCK.Utils.Editor.EditorGUIHelpers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Tween.Serialization.Editor
{
	public static class TweenObjectCreationConfigEditor
	{
		private static readonly ObjectCreationConfigEditor configEditor;

		static TweenObjectCreationConfigEditor()
		{
			configEditor = new ObjectCreationConfigEditor();
			configEditor.CustomArgDrawFuncs.Add(typeof(Func<float, float>), DrawEasingSelector);
		}

		public static void Draw(ObjectCreationConfig config, Object parentObject)
		{
			configEditor.Draw(config, parentObject, TweenCreatorFunctions.Store);

			ReportWarningForAnyNullObjectReferences(config.Args);
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