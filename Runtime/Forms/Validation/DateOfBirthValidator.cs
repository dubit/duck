using System;
using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
{
	[RequireComponent(typeof(DateOfBirthDropdownField))]
	public class DateOfBirthValidator : AbstractValidator
	{
		private DateOfBirthDropdownField dateOfBirthField;

		protected override void Awake()
		{
			base.Awake();

			dateOfBirthField = GetComponent<DateOfBirthDropdownField>();
		}

		public override bool Validate()
		{
			var dateOfBirth = (DateOfBirth) dateOfBirthField.GetValue();
			DateTime dateTime;
			DateTime.TryParse(string.Format("{0}/{1}/{2}", dateOfBirth.month, dateOfBirth.day, dateOfBirth.year), out dateTime);
			return dateTime != DateTime.MinValue;
		}
	}
}
