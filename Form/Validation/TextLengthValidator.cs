using UnityEngine;

namespace DUCK.Form.Validation
{
	public sealed class TextLengthValidator : TextValidator
	{
		[SerializeField]
		private int min;
		[SerializeField]
		private int max;

		public override bool Validate()
		{
			var textLength = textField.Text.Length;
			return textLength >= min && textLength <= max;
		}
	}
}
