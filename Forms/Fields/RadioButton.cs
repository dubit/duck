using System;
using DUCK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.Forms.Fields
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

		protected void NotifySelected()
		{
			if (OnSelected != null)
			{
				OnSelected.Invoke();
			}
		}
	}
}