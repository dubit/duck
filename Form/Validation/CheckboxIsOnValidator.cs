using UnityEngine;

namespace DUCK.Forms.Validation
{
	public class CheckboxIsOnValidator : CheckboxValidator
	{
		[SerializeField]
		private bool isOn;

		public override bool Validate()
		{
			return isOn == checkboxField.IsChecked;
		}
	}
}
