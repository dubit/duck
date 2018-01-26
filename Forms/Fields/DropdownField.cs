using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Fields
{
	[RequireComponent(typeof(Dropdown))]
	public class DropdownField : AbstractFormField
	{
		[SerializeField]
		private int defaultValue;

		private Dropdown dropdown;

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