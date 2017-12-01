using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form.Examples
{
	[RequireComponent(typeof(Button))]
	public sealed class ClearForm : MonoBehaviour
	{
		[SerializeField]
		private Form form;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(form.Clear);
		}
	}
}
