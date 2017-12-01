using UnityEngine;
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

		protected override void OnClear()
		{
			toggleField.isOn = false;
		}
	}
}