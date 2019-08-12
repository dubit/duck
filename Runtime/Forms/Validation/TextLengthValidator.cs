using UnityEngine;

namespace DUCK.Forms.Validation
{
	public sealed class TextLengthValidator : TextValidator
	{
		[SerializeField]
		private int min;
		[SerializeField]
		private int max;

		public override bool Validate()
		{
			return CheckStringLengthIsWithinRange(TextField.Text, min, max);
		}

		public static bool CheckStringLengthIsWithinRange(string text, int min, int max)
		{
			var length = text.Length;
			return length >= min && length <= max;
		}
	}
}
