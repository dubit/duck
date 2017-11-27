using UnityEngine;

namespace DUCK.Form.Validation
{
	public class CheckboxIsOnValidator : CheckboxValidator
	{
		[SerializeField]
		private bool isOn;

		public override bool Validate()
		{
			return isOn == checkboxField.Checked;
		}
	}
}
