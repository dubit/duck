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

		protected override void Awake()
		{
			base.Awake();
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
			var value = FormField.GetStringValue();
			if (!caseSensitive)
			{
				value = value.ToLower();
			}
			return blackListedWords.All(word => word != value);
		}
	}
}