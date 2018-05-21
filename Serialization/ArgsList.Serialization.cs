using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace DUCK.Serialization
{
	public partial class ArgsList
	{
		private const string COMPONENT_PREFIX = "c:";
		private const string ENUM_PREFIX = "e:";
		private const string SCRIPTABLE_OBJECT_PREFIX = "so:";

		[SerializeField]
		private string[] typeOrder;

		[SerializeField]
		private string[] stringValues;

		[SerializeField]
		private int[] intValues;

		[SerializeField]
		private float[] floatValues;

		[SerializeField]
		private bool[] boolValues;

		[SerializeField]
		private GameObject[] gameObjectArgs;

		[SerializeField]
		private Vector2[] vector2Args;

		[SerializeField]
		private Vector3[] vector3Args;

		[SerializeField]
		private Vector4[] vector4Args;

		[SerializeField]
		private Color[] colorArgs;

		[SerializeField]
		private ScriptableObject[] scriptableObjectArgs;

		public void OnBeforeSerialize()
		{
			if (argTypes == null) return;

			var argLists = new Dictionary<Type, List<object>>();
			var typeOrderList = new List<string>();

			Func<Type, List<object>> lazyGetList = (t) =>
			{
				if (t.IsSubclassOf(typeof(Component)))
				{
					t = typeof(GameObject);
				}
				else if (t.IsSubclassOf(typeof(Enum)))
				{
					t = typeof(int);
				}
				else if (t.IsSubclassOf(typeof(ScriptableObject)))
				{
					t = typeof(ScriptableObject);
				}
				else if (argLists.ContainsKey(t))
				{
					return argLists[t];
				}
				return argLists.ContainsKey(t) ? argLists[t] : argLists[t] = new List<object>();
			};

			for (int i = 0; i < argTypes.Count; i++)
			{
				var argType = argTypes[i];
				var arg = args[i];
				var list = lazyGetList(argType);
				list.Add(arg);

				if (argType.IsSubclassOf(typeof(Component)))
				{
					typeOrderList.Add(COMPONENT_PREFIX + argType.FullName);
				}
				else if (argType.IsSubclassOf(typeof(Enum)))
				{
					typeOrderList.Add(ENUM_PREFIX + argType.FullName);
				}
				else if (argType.IsSubclassOf(typeof(ScriptableObject)))
				{
					typeOrderList.Add(SCRIPTABLE_OBJECT_PREFIX + argType.FullName);
				}
				else
				{
					typeOrderList.Add(argType.FullName);
				}
			}

			typeOrder = typeOrderList.ToArray();

			foreach (var supportedType in supportedTypes)
			{
				supportedType.Value.SetList(this, lazyGetList(supportedType.Value.Type));
			}
		}

		public void OnAfterDeserialize()
		{
			var serializedLists = new Dictionary<string, List<object>>();
			args = new List<object>();
			var localArgTypes = new List<Type>();

			// Convert all serialized arrays to lists.
			foreach (var supportedType in supportedTypes.Values)
			{
				var list = supportedType.GetList(this);
				serializedLists.Add(supportedType.Type.FullName, list);
			}

			for (var i = 0; i < typeOrder.Length; i++)
			{
				var typeName = typeOrder[i];
				List<object> list;
				Type argType;

				if (typeName.StartsWith(COMPONENT_PREFIX))
				{
					// strip off prefix
					typeName = typeName.Replace(COMPONENT_PREFIX, "");
					if (!componentTypes.ContainsKey(typeName))
					{
						throw new Exception("ArgsList cannot deserialize component item of type: " + typeName + ", it was not found in the assemblies");
					}

					argType = componentTypes[typeName];
					localArgTypes.Add(argType);
					list = serializedLists[typeof(GameObject).FullName];
				}
				else if (typeName.StartsWith(ENUM_PREFIX))
				{
					// strip off prefix
					typeName = typeName.Replace(ENUM_PREFIX, "");

					if (!enumTypes.ContainsKey(typeName))
					{
						throw new Exception("ArgsList cannot deserialize enum item of type: " + typeName + ", it was not found in the assemblies");
					}

					argType = enumTypes[typeName];
					localArgTypes.Add(argType);
					list = serializedLists[typeof(int).FullName];
				}
				else if (typeName.StartsWith(SCRIPTABLE_OBJECT_PREFIX))
				{
					// strip off prefix
					typeName = typeName.Replace(SCRIPTABLE_OBJECT_PREFIX, "");

					if (!scriptableObjectTypes.ContainsKey(typeName))
					{
						throw new Exception("ArgsList cannot deserialize ScriptableObject item of type: " + typeName + ", it was not found in the assemblies");
					}

					argType = scriptableObjectTypes[typeName];
					localArgTypes.Add(argType);
					list = serializedLists[typeof(ScriptableObject).FullName];
				}
				else
				{
					if (!supportedTypes.ContainsKey(typeName))
					{
						throw new Exception("ArgsList cannot deserialize item of type: " + typeName + ", because it's not supported");
					}

					argType = supportedTypes[typeName].Type;
					localArgTypes.Add(argType);
					list = serializedLists[typeName];
				}

				if (list.Count > 0)
				{
					var arg = list[0];
					list.RemoveAt(0);
					args.Add(arg);
				}
				else
				{
					// if there was no arg found, we must create default value for it
					args.Add(argType.IsValueType ? Activator.CreateInstance(argType) : null);
				}
			}

			argTypes = new ReadOnlyCollection<Type>(localArgTypes);
		}

		class SupportedType
		{
			public Type Type { get; private set; }
			private readonly Func<ArgsList, List<object>> getList;
			private readonly Action<ArgsList, List<object>> setList;

			private SupportedType(Type type, Func<ArgsList, List<object>> getList, Action<ArgsList, List<object>> setList)
			{
				Type = type;
				this.getList = getList;
				this.setList = setList;
			}

			public List<object> GetList(ArgsList instance)
			{
				return getList(instance);
			}

			public void SetList(ArgsList instance, List<object> list)
			{
				setList(instance, list);
			}

			public static SupportedType Create<T>(Func<ArgsList, T[]> getList, Action<ArgsList, T[]> setList)
			{
				return new SupportedType(
					typeof(T),
					i =>
					{
						var list = getList(i);
						return list != null ? list.Cast<object>().ToList() : new List<object>();
					},
					(i, v) => setList(i, v.Cast<T>().ToArray()));
			}
		}
	}
}