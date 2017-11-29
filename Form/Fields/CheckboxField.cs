﻿using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Fields
{
	[RequireComponent(typeof(Toggle))]
	public class CheckboxField : AbstractFormField
	{
		public bool IsChecked
		{
			get { return toggleField.isOn; }
		}

		[SerializeField]
		private bool clearOnReset;

		private Toggle toggleField;

		protected override void Awake()
		{
			base.Awake();
			toggleField = GetComponent<Toggle>();
		}

		public override object GetValue()
		{
			return IsChecked;
		}

		public override void ResetField()
		{
			base.ResetField();

			if (clearOnReset)
			{
				toggleField.isOn = false;
			}
		}
	}
}