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
		private bool caseSensitive = true;
		[SerializeField]
		private List<string> blackListedWords;

		private AbstractFormField formField;

		private void Awake()
		{
			formField = GetComponent<AbstractFormField>();

			if (!caseSensitive)
			{
				for (var i = 0; i < blackListedWords.Count; i++)
				{
					blackListedWords[i] = blackListedWords[i].ToLower();
				}
			}
		}

		public override bool Validate()
		{
			var value = formField.GetStringValue();
			if (!caseSensitive)
			{
				value = value.ToLower();
			}
			return blackListedWords.All(word => word != value);
		}
	}
}