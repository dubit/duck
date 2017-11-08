using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DUCK.Utils.Editor.EditorGUIHelpers
{
	public static partial class EditorGUILayoutHelpers
	{
		private static readonly Dictionary<Type, Func<string, object, object>> drawerFunctions =
			new Dictionary<Type, Func<string, object, object>>
			{
				{typeof(string), (l, v) => EditorGUILayout.TextField(l, (string) v)},
				{typeof(int), (l, v) => EditorGUILayout.IntField(l, (int) v)},
				{typeof(float), (l, v) => EditorGUILayout.FloatField(l, (float) v)},
				{typeof(double), (l, v) => EditorGUILayout.DoubleField(l, (double) v)},
				{typeof(bool), (l, v) => EditorGUILayout.Toggle(l, (bool) v)},
				{typeof(Vector2), (l, v) => EditorGUILayout.Vector2Field(l, (Vector2) v)},
				{typeof(Vector3), (l, v) => EditorGUILayout.Vector3Field(l, (Vector3) v)},
				{typeof(Vector4), (l, v) => EditorGUILayout.Vector4Field(l, (Vector4) v)},
				{typeof(Color), (l, v) => EditorGUILayout.ColorField(l, (Color) v)},
				{typeof(Rect), (l, v) => EditorGUILayout.RectField(l, (Rect) v)},
				{typeof(Bounds), (l, v) => EditorGUILayout.BoundsField(l, (Bounds) v)},
			};

		public static T FieldByType<T>(T obj)
		{
			return (T) FieldByType(obj, typeof(T));
		}

		public static T FieldByType<T>(string label, T obj)
		{
			return (T) FieldByType(label, obj, typeof(T));
		}

		public static object FieldByType(object obj, Type type)
		{
			return FieldByType(type.Name, obj, type);
		}

		public static object FieldByType(string label, object obj, Type type)
		{
			// special case for object fields
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				return EditorGUILayout.ObjectField(label, (UnityEngine.Object) obj, type, true);
			}

			// check we can deal with this type of field
			if (!drawerFunctions.ContainsKey(type))
			{
				throw new Exception("Type: " + type.Name + " is not supported");
			}

			return drawerFunctions[type](label, obj);
		}
	}
}
