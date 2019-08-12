using System;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Forms.Fields
{
	public class RadioField : AbstractFormField
	{
		[SerializeField]
		private RadioButton[] radioButtons;
		[SerializeField]
		private bool hasDefaultValue;
		[SerializeField]
		private int defaultValue;

		private RadioButton selectedRadioButton;

		protected override void Awake()
		{
			base.Awake();

			if (radioButtons.Length == 0)
			{
				throw new Exception("Radio field needs to have atleast one radio button.");
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
			if (hasDefaultValue)
			{
				SelectRadio(radioButtons[defaultValue]);
			}
			else
			{
				foreach (var radioButton in radioButtons)
				{
					radioButton.SetSelected(false);
				}
			}
		}
	}
}