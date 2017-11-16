using System;
using UnityEngine;

namespace DUCK.IOS
{
	public enum PermissionVariableType
	{
		String,
		Integer,
		Boolean,
	}

	[Serializable]
	public class IOSPermission
	{
		public string key;
		public string value;
		public PermissionVariableType permissionType;
	}

	public class IOSPermissionsObject : ScriptableObject
	{
		public IOSPermission[] Permissions { get { return permissions; } }

		[SerializeField]
		private IOSPermission[] permissions;
	}
}
