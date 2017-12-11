using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Fields
{
	[RequireComponent(typeof(Dropdown))]
	public class DropdownField : AbstractFormField
	{
		private Dropdown dropdown;
		[SerializeField]
		private int defaultValue;

		protected override void Awake()
		{
			base.Awake();
			dropdown = GetComponent<Dropdown>();
			defaultValue = dropdown.value;
		}

		public override object GetValue()
		{
			return dropdown.options[dropdown.value];
		}

		public override string GetStringValue()
		{
			var dropdownOptionsData = (Dropdown.OptionData) GetValue();
			return dropdownOptionsData.text;
		}

		protected override void SetDefaultValue()
		{
			dropdown.value = defaultValue;
		}
	}
}