using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
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
