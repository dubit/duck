using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DUCK.Serialization
{
	public interface ICreatorFunctionStore
	{
		string[] Keys { get; }
		bool Exists(string key);
		ICreatorFunctionInfo Get(string key);
	}

	public class CreatorFunctionStore<T> : ICreatorFunctionStore
	{
		public string[] Keys
		{
			get { return creatorFunctionInfos.Keys.ToArray(); }
		}

		private readonly Dictionary<string, CreatorFunctionInfo<T>> creatorFunctionInfos;

		public CreatorFunctionStore()
		{
			creatorFunctionInfos = new Dictionary<string, CreatorFunctionInfo<T>>();
		}

		public void Add(string key, CreatorFunctionInfo<T> functionInfo)
		{
			creatorFunctionInfos.Add(key, functionInfo);
		}

		public bool Exists(string key)
		{
			return creatorFunctionInfos.ContainsKey(key);
		}

		public ICreatorFunctionInfo Get(string key)
		{
			return creatorFunctionInfos.ContainsKey(key) ? creatorFunctionInfos[key] : null;
		}

		public CreatorFunctionInfo<T> this[string key]
		{
			get { return Get(key) as CreatorFunctionInfo<T>; }
			set { Add(key, value); }
		}
	}

	public interface ICreatorFunctionInfo
	{
		List<Type> SerializedArgTypes { get; }
		List<Type> RealArgTypes { get; }
		List<string> ArgNames { get; }
	}

	public class CreatorFunctionInfo<T> : ICreatorFunctionInfo
	{
		public List<Type> RealArgTypes { get; private set; }
		public List<Type> SerializedArgTypes { get; private set; }
		public List<string> ArgNames { get; private set; }

		private Func<MethodBase, object[], object[]> mapArgs;
		private MethodBase method;

		public CreatorFunctionInfo(MethodBase method,
			Func<Type, Type> mapArgTypes = null,
			Func<MethodBase, object[], object[]> mapArgs = null)
		{
			this.method = method;
			this.mapArgs = mapArgs;
			var parameterInfos = method.GetParameters();
			ArgNames = parameterInfos.Select(a => a.Name).ToList();
			RealArgTypes = parameterInfos.Select(a => a.ParameterType).ToList();
			var serializedArgTypes = parameterInfos.Select(a => a.ParameterType);
			if (mapArgTypes != null)
			{
				serializedArgTypes = serializedArgTypes.Select(mapArgTypes);
			}
			SerializedArgTypes = serializedArgTypes.ToList();
		}

		public T Invoke(object[] args)
		{
			args = mapArgs != null ? mapArgs(method, args) : args;
			return (T) method.Invoke(null, args);
		}
	}
}