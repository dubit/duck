using System.Text.RegularExpressions;
using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
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
