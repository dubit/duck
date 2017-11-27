using DUCK.Form.Validation;
using UnityEngine;

namespace DUCK.Form.Fields
{
	public abstract class AbstractFormField : MonoBehaviour
	{
		public string FieldName
		{
			get { return fieldName; }
		}

		[SerializeField]
		private string fieldName;
		[SerializeField]
		private FormMessage formMessage;
		private AbstractValidator[] validators;

		protected virtual void Awake()
		{
			validators = GetComponents<AbstractValidator>();
		}

		public abstract object GetValue();

		public string GetStringValue()
		{
			return GetValue().ToString();
		}

		public bool Validate()
		{
			foreach (var validator in validators)
			{
				var isValid = validator.Validate();
				if (!isValid)
				{
					ShowMessage(validator.Error);
					return false;
				}
			}

			return true;
		}

		public virtual void ResetField()
		{
		}

		public void RemoveMessage()
		{
			if (formMessage != null)
			{
				formMessage.RemoveMessage();
			}
		}

		public void ShowMessage(string message)
		{
			if (formMessage != null)
			{
				formMessage.DisplayMessage(message);
			}
		}
	}
}