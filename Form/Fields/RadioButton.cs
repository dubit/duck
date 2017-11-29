using System;
using DUCK.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace DUCK.Form.Fields
{
	[RequireComponent(typeof(Button))]
	public class RadioButton : MonoBehaviour
	{
		public string Id
		{
			get { return id; }
		}

		[SerializeField]
		private string id;

		public event Action OnSelected;

		public virtual void SetSelected(bool selected)
		{
		}

		protected void InvokeOnSelected()
		{
			OnSelected.SafeInvoke();
		}
	}
}