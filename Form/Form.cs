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
		private List<FieldConfig> fieldConfigs;

		public event Action<Dictionary<string, AbstractFormField>> OnSubmitForm;

		private void Awake()
		{
			if (submitButton != null)
			{
				SetSubmitButton(submitButton);
			}
		}

		/// <summary>
		/// Validate the fields and if successful invoke the OnSubmitForm event.
		/// Also handles clearing fields on submission if applicable.
		/// </summary>
		public void SubmitForm()
		{
			var isValid = true;

			foreach (var fieldConfig in fieldConfigs)
			{
				var isFormValid = fieldConfig.field.Validate();

				if (!isFormValid && isValid)
				{
					isValid = false;
				}
			}

			foreach (var fieldConfig in fieldConfigs)
			{
				if (fieldConfig.clearOnSubmit)
				{
					fieldConfig.field.Clear();
				}
			}

			if (isValid)
			{
				OnSubmitForm.SafeInvoke(fieldConfigs.ToDictionary(config => config.field.FieldName, config => config.field));
			}
		}


		/// <summary>
		/// Set the submit button.
		/// if a submit button is already set, it will unscribed and be replaced.
		/// </summary>
		/// <param name="button">The submit button to be set.</param>
		public void SetSubmitButton(Button button)
		{
			if (submitButton != null)
			{
				submitButton.onClick.RemoveListener(SubmitForm);
			}
			submitButton = button;
			submitButton.onClick.AddListener(SubmitForm);
		}

		/// <summary>
		/// Add a form field to the form.
		/// Intended for creating a form at runtime.
		/// </summary>
		/// <param name="formField">The form field to add</param>
		/// <param name="clearOnSubmit">If the field should be cleared on submission</param>
		public void AddFormField(AbstractFormField formField, bool clearOnSubmit = false)
		{
			fieldConfigs.Add(new FieldConfig()
			{
				field = formField,
				clearOnSubmit = clearOnSubmit
			});
		}

		/// <summary>
		/// Remove a form field from the form.
		/// Intended for creating a form at runtime.
		/// </summary>
		/// <param name="formField">The form field to remove.</param>
		/// <returns>If the field was removed successfully or if the field was not in the form.</returns>
		public bool RemoveFormField(AbstractFormField formField)
		{
			var fieldConfig = fieldConfigs.FirstOrDefault(config => config.field == formField);
			return fieldConfig == null || fieldConfigs.Remove(fieldConfig);
		}


		/// <summary>
		/// Get all fields in the form.
		/// </summary>
		/// <returns>An array of form fields</returns>
		public AbstractFormField[] GetAllFields()
		{
			return fieldConfigs.Select(config => config.field).ToArray();
		}

		/// <summary>
		/// Get a form field by field name.
		/// </summary>
		/// <param name="fieldName">The field name</param>
		/// <returns>The form field matching the field name.</returns>
		public AbstractFormField GetField(string fieldName)
		{
			var first = fieldConfigs.FirstOrDefault(config => config.field.FieldName == fieldName);
			return first != null ? first.field : null;
		}

		/// <summary>
		/// Set all fields to default state.
		/// </summary>
		public void Reset()
		{
			fieldConfigs.ForEach(config => config.field.Reset());
		}
	}
}