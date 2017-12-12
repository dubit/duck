using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Fields
{
	[RequireComponent(typeof(InputField))]
	public class TextField : AbstractFormField
	{
		public string Text
		{
			get { return inputField.text; }
			private set { inputField.text = value; }
		}

		private InputField inputField;
		private string defaultValue;

		protected override void Awake()
		{
			base.Awake();
			inputField = GetComponent<InputField>();
			defaultValue = inputField.text;
		}

		public override object GetValue()
		{
			return Text;
		}

		protected override void SetDefaultValue()
		{
			Text = defaultValue;
		}
	}
}