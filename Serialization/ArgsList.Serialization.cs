using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Dubit.DUCK.Serialization
{
	public partial class ArgsList
	{
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


		public void OnBeforeSerialize()
		{
			var argLists = new Dictionary<string, List<object>>();
			var typeOrderList = new List<string>();

			Func<string, List<object>> lazyGetList = (t) =>
			{
				if (argLists.ContainsKey(t))
				{
					return argLists[t];
				}
				return argLists[t] = new List<object>();
			};

			for (int i = 0; i < argTypes.Count; i++)
			{
				var argType = argTypes[i];
				var list = lazyGetList(argType.Name);
				list.Add(args[i]);
				typeOrderList.Add(argType.Name);
			}

			typeOrder = typeOrderList.ToArray();

			foreach (var supportedType in supportedTypes)
			{
				supportedType.Value.SetList(this, lazyGetList(supportedType.Key));
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

			for (int i = 0; i < typeOrder.Length; i++)
			{
				var typeName = typeOrder[i];
				if (!supportedTypes.ContainsKey(typeName))
				{
					// TODO: How to handle this error (on deserialize we find a type that is not supported)
					// for now let's throw, but we may want to handle it more elegantly
					throw new Exception("ArgsList cannot deserialize item of type: " + typeName + ", because it's not supported");
				}

				var supportedType = supportedTypes[typeName];
				localArgTypes.Add(supportedType.Type);

				var list = serializedLists[typeName];
				if (list.Count > 0)
				{
					var arg = list[0];
					list.RemoveAt(0);
					args.Add(arg);
				}
				else
				{
					// if there was no arg found, we must create default value for it
					args.Add(Activator.CreateInstance(supportedType.Type));
				}
			}

			argTypes = new ReadOnlyCollection<Type>(localArgTypes);
		}

		private List<object> GetList(string typeName)
		{
			return null;
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