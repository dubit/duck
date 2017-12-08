﻿using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Fields
{
	/// <summary>
	/// Text field for Unity's text InputField.
	/// </summary>
	[RequireComponent(typeof(InputField))]
	public class TextField : AbstractFormField
	{
		public string Text
		{
			get { return inputField.text; }
			private set { inputField.text = value; }
		}

		private InputField inputField;

		protected override void Awake()
		{
			base.Awake();
			inputField = GetComponent<InputField>();
		}

		public override object GetValue()
		{
			return Text;
		}

		protected override void SetDefaultValue()
		{
			Text = string.Empty;
		}
	}
}