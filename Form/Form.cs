using System;
using System.Collections.Generic;
using System.Linq;
using DUCK.Form.Fields;
using DUCK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Form
{
	public class Form : MonoBehaviour
	{
		[SerializeField]
		private Button submitButton;
		[SerializeField]
		private List<AbstractFormField> serializedFormFields;

		private Dictionary<string, AbstractFormField> formFields;

		public event Action<Dictionary<string, AbstractFormField>> OnSubmitForm;

		private void Awake()
		{
			if (submitButton != null)
			{
				SetSubmitButton(submitButton);
			}

			formFields = new Dictionary<string, AbstractFormField>();

			foreach (var formField in serializedFormFields)
			{
				formFields.Add(formField.FieldName, formField);
			}
		}

		public void SubmitForm()
		{
			var isValid = true;

			foreach (var formField in serializedFormFields)
			{
				var isFormValid = formField.Validate();

				if (!isFormValid && isValid)
				{
					isValid = false;
				}
			}

			foreach (var formField in serializedFormFields)
			{
				formField.Clear();
			}

			if (isValid)
			{
				OnSubmitForm.SafeInvoke(formFields);
			}
		}

		public void SetSubmitButton(Button button)
		{
			if (submitButton != null)
			{
				submitButton.onClick.RemoveListener(SubmitForm);
			}
			submitButton = button;
			submitButton.onClick.AddListener(SubmitForm);
		}

		public void AddFormField(AbstractFormField formField)
		{
			formFields.Add(formField.FieldName, formField);
		}

		public bool RemoveFormField(AbstractFormField formField)
		{
			return formFields.Remove(formField.FieldName);
		}

		public AbstractFormField[] GetAllFields()
		{
			return formFields.Values.ToArray();
		}

		public AbstractFormField GetField(string fieldName)
		{
			return formFields.ContainsKey(fieldName) ? formFields[fieldName] : null;
		}

		public void Reset()
		{
			serializedFormFields.ForEach(formField => formField.Reset());
		}
	}
}