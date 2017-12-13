using DUCK.Forms.Fields;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Examples
{
	public class BasicRadioButton : RadioButton
	{
		[SerializeField]
		private Button selectButton;

		[SerializeField]
		private Toggle toggle;

		private void Awake()
		{
			selectButton.onClick.AddListener(NotifySelected);
		}

		public override void SetSelected(bool selected)
		{
			base.SetSelected(selected);

			toggle.isOn = selected;
		}
	}
}
