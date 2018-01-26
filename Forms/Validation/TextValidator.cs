using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
{
	[RequireComponent(typeof(TextField))]
	public abstract class TextValidator : AbstractValidator
	{
		protected TextField TextField { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			TextField = (TextField)FormField;
		}
	}
}
