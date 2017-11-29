using System;
using DUCK.Form.Validation;
using DUCK.Utils;
using UnityEngine;

namespace DUCK.Form.Fields
{
	[DisallowMultipleComponent]
	public abstract class AbstractFormField : MonoBehaviour
	{
		public string FieldName
		{
			get { return fieldName; }
		}

		[SerializeField]
		private string fieldName;
		private AbstractValidator[] validators;

		public event Action OnReset;
		public event Action OnValidationSuccess;
		public event Action<AbstractValidator> OnValidationFailed;

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
				if(!validator.Validate())
				{
					OnValidationFailed.SafeInvoke(validator);
					return false;
				}
			}

			OnValidationSuccess.SafeInvoke();
			return true;
		}

		public virtual void ResetField()
		{
			OnReset.SafeInvoke();
		}
	}
}