using System;
using UnityEngine;

namespace DUCK.IOS
{
	[Serializable]
	public class IOSPermission
	{
		public string key;
		public string value;
	}

	public class IOSPermissionsObject : ScriptableObject
	{
		public IOSPermission[] Permissions { get { return permissions; } }

		[SerializeField]
		private IOSPermission[] permissions;
	}
}
