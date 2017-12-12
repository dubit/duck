using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
{
	[RequireComponent(typeof(TextField))]
	public abstract class TextValidator : AbstractValidator
	{
		protected TextField textField;

		protected override void Awake()
		{
			base.Awake();
			textField = (TextField)FormField;
		}
	}
}
