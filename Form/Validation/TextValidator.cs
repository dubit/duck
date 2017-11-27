using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(TextField))]
	public abstract class TextValidator : AbstractValidator
	{
		protected TextField textField;

		protected virtual void Awake()
		{
			textField = GetComponent<TextField>();
		}
	}
}
