using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Fields
{
	public class DateOfBirthDropdownField : AbstractFormField
	{
		[SerializeField]
		private Dropdown dayDropdown;
		[SerializeField]
		private Dropdown monthDropdown;
		[SerializeField]
		private Dropdown yearDropdown;

		[SerializeField]
		private Color onValueChangedColor;

		private int currentYear;

		private int defaultDay;
		private int defaultMonth;
		private int defaultYear;

		protected override void Awake()
		{
			base.Awake();

			defaultDay = dayDropdown.value;
			defaultMonth = monthDropdown.value;
			defaultYear = yearDropdown.value;

			dayDropdown.onValueChanged.AddListener(value => { dayDropdown.captionText.color = onValueChangedColor; });
			monthDropdown.onValueChanged.AddListener(value =>
			{
				monthDropdown.captionText.color = onValueChangedColor;
				HandleValueChanged(value);
			});
			yearDropdown.onValueChanged.AddListener(value =>
			{
				yearDropdown.captionText.color = onValueChangedColor;
				HandleValueChanged(value);
			});

			monthDropdown.AddOptions(DateOfBirthUtils.Months.ToList());
			dayDropdown.AddOptions(DateOfBirthUtils.Days.Select(day => day.ToString()).ToList());
			yearDropdown.AddOptions(DateOfBirthUtils.Years.Select(year => year.ToString()).ToList());

			currentYear = DateTime.Now.Year;

			HandleValueChanged(monthDropdown.value);
		}

		public override object GetValue()
		{
			return new DateOfBirth(GetDay(), GetMonth(), GetYear());
		}

		protected override void SetDefaultValue()
		{
			dayDropdown.value = defaultDay;
			monthDropdown.value = defaultMonth;
			yearDropdown.value = defaultYear;
		}

		private void HandleValueChanged(int value)
		{
			var dayValue = dayDropdown.value;
			dayDropdown.ClearOptions();
			var daysInMonth = DateTime.DaysInMonth(GetYear(), GetMonth());
			var days = new int[daysInMonth];
			for (var day = 0; day < daysInMonth; day++)
			{
				days[day] = day + 1;
			}
			dayDropdown.AddOptions(days.Select(day => day.ToString()).ToList());
			if (daysInMonth > dayValue)
			{
				dayDropdown.value = dayValue;
			}
		}

		private int GetYear()
		{
			return currentYear - yearDropdown.value;
		}

		private int GetMonth()
		{
			return monthDropdown.value + 1;
		}

		private int GetDay()
		{
			return dayDropdown.value + 1;
		}
	}
}