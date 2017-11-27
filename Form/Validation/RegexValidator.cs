using System.Text.RegularExpressions;
using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(TextField))]
	public sealed class RegexValidator : TextValidator
	{
		[SerializeField]
		private string pattern;

		public override bool Validate()
		{
			return new Regex(pattern).Match(textField.Text).Success;
		}
	}
}
