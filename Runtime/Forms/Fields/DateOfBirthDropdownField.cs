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

		private Color originalCaptionColor;
		private int currentYear;
		private int defaultDay;
		private int defaultMonth;
		private int defaultYear;

		protected override void Awake()
		{
			base.Awake();

			originalCaptionColor = dayDropdown.captionText.color;

			defaultDay = dayDropdown.value;
			defaultMonth = monthDropdown.value;
			defaultYear = yearDropdown.value;

			dayDropdown.onValueChanged.AddListener(value => { HandleValueChanged(dayDropdown); });
			monthDropdown.onValueChanged.AddListener(value =>
			{
				HandleValueChanged(monthDropdown);
				UpdateDays();
			});
			yearDropdown.onValueChanged.AddListener(value =>
			{
				HandleValueChanged(yearDropdown);
				UpdateDays();
			});

			monthDropdown.AddOptions(DateOfBirthUtils.Months.ToList());
			dayDropdown.AddOptions(DateOfBirthUtils.Days.Select(day => day.ToString()).ToList());
			yearDropdown.AddOptions(DateOfBirthUtils.Years.Select(year => year.ToString()).ToList());

			currentYear = DateTime.Now.Year;

			UpdateDays();
		}

		public override object GetValue()
		{
			return new DateOfBirth(GetYear(), GetMonth(), GetDay());
		}

		protected override void SetDefaultValue()
		{
			dayDropdown.value = defaultDay;
			monthDropdown.value = defaultMonth;
			yearDropdown.value = defaultYear;
		}

		private void HandleValueChanged(Dropdown dropdown)
		{
			dropdown.captionText.color = onValueChangedColor;
		}

		private void UpdateDays()
		{
			var daysInMonth = DateTime.DaysInMonth(GetYear(), GetMonth());
			if (daysInMonth == dayDropdown.options.Count) return;

			var dayValue = dayDropdown.value;
			dayDropdown.ClearOptions();
			var days = new int[daysInMonth];
			for (var i = 0; i < daysInMonth; i++)
			{
				days[i] = i + 1;
			}
			dayDropdown.AddOptions(days.Select(day => day.ToString()).ToList());

			// Update the value to be within the new range of days or reset it if its invalid.
			if (dayValue < daysInMonth)
			{
				dayDropdown.value = dayValue;
			}
			else
			{
				dayDropdown.value = 0;
				dayDropdown.captionText.color = originalCaptionColor;
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