using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(CheckboxField))]
	public abstract class CheckboxValidator : AbstractValidator
	{
		protected CheckboxField checkboxField;

		protected virtual void Awake()
		{
			checkboxField = GetComponent<CheckboxField>();
		}
	}
}
