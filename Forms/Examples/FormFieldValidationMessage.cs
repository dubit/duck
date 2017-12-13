using DUCK.Forms.Fields;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Examples
{
	[RequireComponent(typeof(Text))]
	public class FormFieldValidationMessage : MonoBehaviour
	{
		[SerializeField]
		private AbstractFormField field;

		private Text textElement;

		private void Awake()
		{
			textElement = GetComponent<Text>();

			field.OnFieldReset += RemoveMessage;
			field.OnValidationFailed += validator => DisplayMessage(validator.Error);
			field.OnValidationSuccess += RemoveMessage;
		}

		private void Start()
		{
			gameObject.SetActive(false);
		}

		public void DisplayMessage(string message)
		{
			textElement.text = message;
			gameObject.SetActive(true);
		}

		public void RemoveMessage()
		{
			textElement.text = string.Empty;
			gameObject.SetActive(false);
		}
	}
}
