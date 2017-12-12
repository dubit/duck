using DUCK.Forms.Fields;
using UnityEngine;

namespace DUCK.Forms.Validation
{
	[RequireComponent(typeof(AbstractFormField))]
	public abstract class AbstractValidator : MonoBehaviour
	{
		public AbstractFormField FormField { get; private set; }
		public string Error { get { return errorMessage; } }

		[SerializeField]
		private string errorMessage;

		protected virtual void Awake()
		{
			FormField = GetComponent<AbstractFormField>();
		}

		public abstract bool Validate();

		public void SetErrorMessage(string message)
		{
			errorMessage = message;
		}
	}
}
