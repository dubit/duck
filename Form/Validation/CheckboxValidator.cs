using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(CheckboxField))]
	public abstract class CheckboxValidator : AbstractValidator
	{
		protected CheckboxField checkboxField;

		protected override void Awake()
		{
			base.Awake();
			checkboxField = (CheckboxField)FormField;
		}
	}
}
