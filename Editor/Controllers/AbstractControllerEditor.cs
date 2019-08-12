using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DUCK.Controllers.Editor
{
	[CustomEditor(typeof(AbstractController), true)]
	public class AbstractControllerEditor : UnityEditor.Editor
	{
		private AbstractController controller;

		private Type controllerType;
		private MethodInfo[] methods;

		private void OnEnable()
		{
			controller = (AbstractController) target;
			controllerType = controller.GetType();

			methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m =>
			{
				var parameters = m.GetParameters();

				return
					// filter out any property getters
					!m.Name.Contains("_")
					// Non static
					&& !m.IsStatic
					// No type parameters
					&& m.GetGenericArguments().Length == 0
					// No parameters
					&& (parameters.Length == 0 || parameters.All(p => p.IsOptional));
			}).ToArray();
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();

			foreach (var methodInfo in methods)
			{
				var attributes = methodInfo.GetCustomAttributes(typeof(DrawInspectorButtonAttribute), false);
				if (attributes.Length == 0) continue;

				if (GUILayout.Button(methodInfo.Name))
				{
					methodInfo.Invoke(controller, null);
				}
			}
		}
	}
}