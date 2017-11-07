using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace DUCK.Serialization
{
	[Serializable]
	public partial class ArgsList : ISerializationCallbackReceiver
	{
		private static readonly Dictionary<string, SupportedType> supportedTypes;
		private static readonly Type[] supportedTypesArray;
		private static readonly Dictionary<string, Type> componentTypes;

		static ArgsList()
		{
			var supportedTypesList = new SupportedType[]
			{
				SupportedType.Create(i => i.stringValues, (i, v) => i.stringValues = v),
				SupportedType.Create(i => i.intValues, (i, v) => i.intValues = v),
				SupportedType.Create(i => i.floatValues, (i, v) => i.floatValues = v),
				SupportedType.Create(i => i.boolValues, (i, v) => i.boolValues = v),
				SupportedType.Create(i => i.gameObjectArgs, (i, v) => i.gameObjectArgs = v),
				SupportedType.Create(i => i.vector2Args, (i, v) => i.vector2Args = v),
				SupportedType.Create(i => i.vector3Args, (i, v) => i.vector3Args = v),
				SupportedType.Create(i => i.vector4Args, (i, v) => i.vector4Args = v),
				SupportedType.Create(i => i.colorArgs, (i, v) => i.colorArgs = v),
			};

			supportedTypesArray = supportedTypesList.Select(t => t.Type).ToArray();
			supportedTypes = supportedTypesList.ToDictionary(t => t.Type.Name, t => t);
			componentTypes = new Dictionary<string, Type>();

			// Get every type that extends component
			var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Component)))
				.Concat(typeof(Component).Assembly.GetTypes()
					.Where(t => t.IsSubclassOf(typeof(Component)))
				);

			foreach (var type in types)
			{
				componentTypes.Add(type.Name, type);
			}
		}

		public IList<Type> ArgTypes
		{
			get { return argTypes; }
		}

		public IList<object> AllArgs
		{
			get { return args; }
		}

		private ReadOnlyCollection<Type> argTypes;
		private List<object> args;

		public ArgsList(params Type[] types)
		{
			if (types.Length > 0)
			{
				SetTypes(types);
			}
		}

		public void SetTypes(IList<Type> types)
		{
			if (types == null) throw new ArgumentNullException("types");
			if (!types.Any()) throw new ArgumentException("types must be a list greater than 0");

			var oldArgs = args;
			args = new List<object>();

			foreach (var argType in types)
			{
				if (!ValidateArgType(argType))
				{
					throw new ArgumentException("Cannot handle arguments of type: " + argType.Name);
				}

				var defaultValue = argType.IsValueType ? Activator.CreateInstance(argType) : null;
				args.Add(defaultValue);
			}

			if (oldArgs != null && argTypes != null)
			{
				for (int i = 0; i < oldArgs.Count; i++)
				{
					if (i < types.Count)
					{
						if (types[i] == argTypes[i])
						{
							args[i] = oldArgs[i];
						}
					}
				}
			}

			argTypes = new ReadOnlyCollection<Type>(types);
		}

		public object this[int index]
		{
			get
			{
				if (index >= argTypes.Count || index < 0) throw new ArgumentOutOfRangeException("index");
				return args[index];
			}

			set
			{
				if (index >= argTypes.Count || index < 0) throw new ArgumentOutOfRangeException("index");

				if (value != null)
				{
					var argType = value.GetType();
					if (!ValidateArgTypeForIndex(argType, index))
					{
						throw new ArgumentException(argType.Name + " is not the correct type for index " + index);
					}
				}
				else
				{
					var argType = argTypes[index];
					if (argType.IsValueType)
					{
						throw new ArgumentException("Null cannot be set against a value type. (index = " + index + ", type = " +
						                            argType.Name);
					}
				}

				args[index] = value;
			}
		}

		public void Set<T>(int index, T arg)
		{
			var argType = typeof(T);
			if (index >= argTypes.Count || index < 0) throw new ArgumentOutOfRangeException("index");
			if (!ValidateArgTypeForIndex(argType, index))
			{
				throw new ArgumentException(argType.Name + " is not the correct type for index " + index);
			}

			args[index] = arg;
		}

		public T Get<T>(int index)
		{
			var argType = typeof(T);
			if (index >= argTypes.Count || index < 0) throw new ArgumentOutOfRangeException("index");
			if (!ValidateArgTypeForIndex(argType, index))
			{
				throw new ArgumentException(argType.Name + " is not the correct type for index " + index);
			}

			return (T) args[index];
		}

		private bool ValidateArgTypeForIndex(Type type, int index)
		{
			return argTypes.Count > index && argTypes[index] == type;
		}

		private bool ValidateArgType(Type type)
		{
			return supportedTypesArray.Contains(type) || type.IsSubclassOf(typeof(Component));
		}
	}
}