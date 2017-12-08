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

		public event Action OnFieldCleared;
		public event Action OnFieldReset;
		public event Action OnValidationSuccess;
		public event Action<AbstractValidator> OnValidationFailed;

		protected virtual void Awake()
		{
			validators = GetComponents<AbstractValidator>();
		}

		public abstract object GetValue();
		protected abstract void SetDefaultValue();

		/// <summary>
		/// Clear on submit if applicable.
		/// </summary>
		public void Clear()
		{
			SetDefaultValue();
			OnFieldCleared.SafeInvoke();
		}

		/// <summary>
		/// Resets the field to the default value.
		/// </summary>
		public void Reset()
		{
			SetDefaultValue();
			OnFieldReset.SafeInvoke();
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
	}
}