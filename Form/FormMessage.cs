using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form
{
	[RequireComponent(typeof(Text))]
	public class FormMessage : MonoBehaviour
	{
		private Text textElement;

		private void Awake()
		{
			textElement = GetComponent<Text>();
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
