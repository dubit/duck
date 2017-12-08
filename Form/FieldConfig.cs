using System;
using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form
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