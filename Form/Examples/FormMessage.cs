using DUCK.Form.Fields;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Examples
{
	[RequireComponent(typeof(Text))]
	public class FormMessage : MonoBehaviour
	{
		[SerializeField]
		private AbstractFormField field;

		private Text textElement;

		private void Awake()
		{
			textElement = GetComponent<Text>();

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
