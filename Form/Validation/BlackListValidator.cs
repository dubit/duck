using System.Linq;
using DUCK.Form.Fields;
using UnityEngine;

namespace DUCK.Form.Validation
{
	[RequireComponent(typeof(AbstractFormField))]
	public class BlackListValidator : AbstractValidator
	{
		[SerializeField]
		private bool caseSensitive = true;
		[SerializeField]
		private string[] blackListedWords;

		private AbstractFormField formField;

		private void Awake()
		{
			formField = GetComponent<AbstractFormField>();

			if (!caseSensitive)
			{
				for (var i = 0; i < blackListedWords.Length; i++)
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
