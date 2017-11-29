using System;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Form.Fields
{
	public class RadioField : AbstractFormField
	{
		[SerializeField]
		private RadioButton[] radioButtons;

		private RadioButton selectedRadioButton;

		protected override void Awake()
		{
			base.Awake();

			if (radioButtons.Length == 0)
			{
				throw new Exception("Radio field needs to have atleast one radio button.");
			}

			radioButtons.ForEach(radioButton => radioButton.OnSelected += () => SelectRadio(radioButton));

			SelectRadio(radioButtons[0]);
		}

		private void SelectRadio(RadioButton selected)
		{
			radioButtons.ForEach(radioButton => radioButton.SetSelected(radioButton == selected));
			selectedRadioButton = selected;
		}

		public override object GetValue()
		{
			return selectedRadioButton.Id;
		}
	}
}