using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Fields
{
	[RequireComponent(typeof(InputField))]
	public class TextField : AbstractFormField
	{
		public string Text
		{
			get { return inputField.text; }
		}

		[SerializeField]
		private bool clearOnReset;

		private InputField inputField;

		protected override void Awake()
		{
			base.Awake();
			inputField = GetComponent<InputField>();
		}

		public override object GetValue()
		{
			return Text;
		}

		public override void ResetField()
		{
			base.ResetField();

			if (clearOnReset)
			{
				inputField.text = string.Empty;
			}
		}
	}
}