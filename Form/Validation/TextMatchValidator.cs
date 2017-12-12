using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
{
	public class TextMatchValidator : TextValidator
	{
		[SerializeField]
		private TextField textFieldToMatch;

		public override bool Validate()
		{
			return textField.Text == textFieldToMatch.Text;
		}
	}
}
