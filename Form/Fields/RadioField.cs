﻿using System;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Form.Fields
{
	public class RadioField : AbstractFormField
	{
		[SerializeField]
		private RadioButton[] radioButtons;

		private RadioButton selectedRadioButton;

		[SerializeField, Tooltip("Enable to select the first option on awake.")]
		private bool firstIsDefault;

		protected override void Awake()
		{
			base.Awake();

			if (radioButtons.Length == 0)
			{
				throw new Exception("Radio field needs to have at least one radio button.");
			}

			radioButtons.ForEach(radioButton =>
			{
				radioButton.SetSelected(false);
				radioButton.OnSelected += () => SelectRadio(radioButton);
			});

			SetDefaultValue();
		}

		public override object GetValue()
		{
			return selectedRadioButton ? selectedRadioButton.Id : null;
		}

		private void SelectRadio(RadioButton selected)
		{
			radioButtons.ForEach(radioButton => radioButton.SetSelected(radioButton == selected));
			selectedRadioButton = selected;
		}

		protected override void SetDefaultValue()
		{
			if (firstIsDefault)
			{
				SelectRadio(radioButtons[0]);
			}
			else
			{
				radioButtons.ForEach(radioButton => { radioButton.SetSelected(false); });
			}
		}
	}
}