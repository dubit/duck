using DUCK.Form.Fields;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Examples
{
	public class CustomRadioButton : RadioButton
	{
		[SerializeField]
		private Button selectButton;

		[SerializeField]
		private Toggle toggle;

		private void Awake()
		{
			selectButton.onClick.AddListener(InvokeOnSelected);
		}

		public override void SetSelected(bool selected)
		{
			base.SetSelected(selected);

			toggle.isOn = selected;
		}
	}
}
