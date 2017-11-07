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
				if (argLists.ContainsKey(t))
				{
					return argLists[t];
				}
				return argLists[t] = new List<object>();
			};

			for (int i = 0; i < argTypes.Count; i++)
			{
				var argType = argTypes[i];
				var list = lazyGetList(argType);
				if (argType.IsSubclassOf(typeof(Component)))
				{
					// TODO: This null checking is needed, and needs test coverage
					var arg = args[i] != null ? ((Component) args[i]).gameObject : null;
					list.Add(arg);
					typeOrderList.Add(COMPONENT_PREFIX + argType.Name);
				}
				else
				{
					list.Add(args[i]);
					typeOrderList.Add(argType.Name);
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
				serializedLists.Add(supportedType.Type.Name, list);
			}

			for (var i = 0; i < typeOrder.Length; i++)
			{
				var typeName = typeOrder[i];
				List<object> list;
				Type argType;
				Func<object, object> argConverter = o => o;

				if (typeName.StartsWith(COMPONENT_PREFIX))
				{
					// strip off prefix
					typeName = typeName.Replace(COMPONENT_PREFIX, "");
					if (!componentTypes.ContainsKey(typeName))
					{
						// TODO: How to handle this error (found a component type not in the assembly on deserialize)
						// for now let's throw, but we may want to handle it more elegantly
						throw new Exception("ArgsList cannot deserialize component item of type: " + typeName + ", it was not found in the assemblies");
					}

					argType = componentTypes[typeName];
					localArgTypes.Add(argType);
					list = serializedLists[typeof(GameObject).Name];
					argConverter = o => ((GameObject)o).GetComponent(argType);
				}
				else
				{
					if (!supportedTypes.ContainsKey(typeName))
					{
						// TODO: How to handle this error (on deserialize we find a type that is not supported)
						// for now let's throw, but we may want to handle it more elegantly
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
					args.Add(argConverter(arg));
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
					i => getList(i).Cast<object>().ToList(),
					(i, v) => setList(i, v.Cast<T>().ToArray()));
			}
		}
	}
}