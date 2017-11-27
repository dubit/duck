using UnityEngine;

namespace DUCK.Form.Validation
{
	public abstract class AbstractValidator : MonoBehaviour
	{
		public string Error { get { return errorMessage; } }

		[SerializeField]
		private string errorMessage;

		public abstract bool Validate();
	}
}
