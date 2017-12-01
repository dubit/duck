using System;
using DUCK.Form.Examples;
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
		[SerializeField]
		private bool clearOnSubmit;

		private AbstractValidator[] validators;

		public event Action OnFieldCleared;
		public event Action OnValidationSuccess;
		public event Action<AbstractValidator> OnValidationFailed;

		protected virtual void Awake()
		{
			validators = GetComponents<AbstractValidator>();
		}

		public abstract object GetValue();
		protected abstract void OnClear();

		public void Clear()
		{
			OnClear();
			OnFieldCleared.SafeInvoke();
		}

		public virtual string GetStringValue()
		{
			var value = GetValue();
			return value != null ? value.ToString() : string.Empty;
		}

		public bool Validate()
		{
			foreach (var validator in validators)
			{
				if (!validator.Validate())
				{
					OnValidationFailed.SafeInvoke(validator);
					return false;
				}
			}

			OnValidationSuccess.SafeInvoke();
			return true;
		}

		public virtual void HandleFormSubmit()
		{
			if (clearOnSubmit)
			{
				Clear();
			}
		}
	}
}