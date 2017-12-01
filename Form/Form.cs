﻿using System;
using System.Collections.Generic;
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
		private AbstractFormField[] formFields;

		private Dictionary<string, AbstractFormField> fields;

		public event Action<Dictionary<string, AbstractFormField>> OnSubmitForm;

		private void Awake()
		{
			submitButton.onClick.AddListener(HandleSubmitClicked);

			fields = new Dictionary<string, AbstractFormField>();

			foreach (var formField in formFields)
			{
				fields.Add(formField.FieldName, formField);
			}
		}

		private void HandleSubmitClicked()
		{
			var isValid = true;

			foreach (var formField in formFields)
			{
				var isFormValid = formField.Validate();

				if (!isFormValid && isValid)
				{
					isValid = false;
				}
			}
			
			foreach (var formField in formFields)
			{
				formField.HandleFormSubmit();
			}

			if (isValid)
			{
				OnSubmitForm.SafeInvoke(fields);
			}
		}

		public AbstractFormField[] GetAllFields()
		{
			return formFields;
		}

		public AbstractFormField GetField(string fieldName)
		{
			return fields.ContainsKey(fieldName) ? fields[fieldName] : null;
		}

		public void Clear()
		{
			formFields.ForEach(formField => formField.Clear());
		}
	}
}