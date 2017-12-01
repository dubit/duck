namespace DUCK.Form.Validation
{
	public class NullOrEmptyValidator : AbstractValidator
	{
		public override bool Validate()
		{
			return !string.IsNullOrEmpty(FormField.GetStringValue());
		}
	}
}