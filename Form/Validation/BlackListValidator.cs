using System;
using System.Collections.Generic;
using System.Linq;
using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(AbstractFormField))]
	public class BlackListValidator : AbstractValidator
	{
		public List<string> BlackList
		{
			get { return blackListedWords; }
		}

		[SerializeField]
		private bool ignoreCase = true;
		[SerializeField]
		private List<string> blackListedWords;

		public override bool Validate()
		{
			return !CheckIsBlackListed(blackListedWords, FormField.GetStringValue(), ignoreCase);
		}

		public static bool CheckIsBlackListed(IEnumerable<string> blacklist, string wordToCheck, bool ignoreCase = false)
		{
			var culture = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
			return blacklist.Any(word => wordToCheck.Equals(word, culture));
		}
	}
}