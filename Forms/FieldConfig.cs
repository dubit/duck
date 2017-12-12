using System;
using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms
{
	[Serializable]
	public class FieldConfig
	{
		[SerializeField]
		public AbstractFormField field;
		[SerializeField]
		public bool clearOnSubmit;
	}
}