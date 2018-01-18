using System;
using System.Collections.Generic;
using DUCK.Utils.Editor.EditorGUIHelpers;
using UnityEditor;

namespace DUCK.Serialization.Editor
{
	public class ObjectCreationConfigEditor
	{
		public Dictionary<Type, ArgsListEditor.CustomArgDrawFunc> CustomArgDrawFuncs { get; private set; }

		public ObjectCreationConfigEditor()
		{
			CustomArgDrawFuncs = new Dictionary<Type, ArgsListEditor.CustomArgDrawFunc>();
		}

		public void Draw(ObjectCreationConfig config, ICreatorFunctionStore creatorFunctions)
		{
			ArgsList args = config.Args;

			config.CreatorFunctionKey = EditorGUILayoutHelpers.OptionSelectorField(
				"Creator Function",
				config.CreatorFunctionKey,
				creatorFunctions.Keys);

			var creatorFunctionKey = config.CreatorFunctionKey;

			if (string.IsNullOrEmpty(creatorFunctionKey))
			{
				EditorGUILayout.HelpBox("No creator function selected, nothing will be executed!", MessageType.Info);
				return;
			}

			if (!creatorFunctions.Exists(creatorFunctionKey))
			{
				var message = "Creator function with the key " + creatorFunctionKey + " could not be found!";
				EditorGUILayout.HelpBox(message, MessageType.Error);
				return;
			}

			var creatorFunction = creatorFunctions.Get(creatorFunctionKey);

			if (config.Args == null) config.Args = args = new ArgsList();
			args.SetTypes(creatorFunction.SerializedArgTypes);

			var customDrawFuncs = new Dictionary<int, ArgsListEditor.CustomArgDrawFunc>();
			if (CustomArgDrawFuncs != null)
			{
				foreach (var kvp in CustomArgDrawFuncs)
				{
					for (int i = 0; i < creatorFunction.RealArgTypes.Count; i++)
					{
						if (creatorFunction.RealArgTypes[i] == kvp.Key)
						{
							customDrawFuncs[i] = kvp.Value;
						}
					}
				}
			}

			ArgsListEditor.Draw(args, creatorFunction.ArgNames, customDrawFuncs);
		}
	}
}