using DUCK.Form.Fields;

namespace DUCK.Form.Validation
{
	public abstract class TextValidator : AbstractValidator
	{
		protected TextField textField;

		protected virtual void Awake()
		{
			textField = GetComponent<TextField>();
		}
	}
}
