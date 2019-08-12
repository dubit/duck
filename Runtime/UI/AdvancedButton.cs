using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DUCK.UI
{
	/// <summary>
	/// The advanced button allows you to add a listener that only gets called once.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Advanced Button", 31)]
	public sealed class AdvancedButton : Button
	{
		[SerializeField]
		private ButtonClickedEvent onOneShotClick = new ButtonClickedEvent();
		public ButtonClickedEvent OnOneShotClick { get { return onOneShotClick; } }

		private bool isDone;

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				OneShotPress();
			}
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			OneShotPress();
			base.OnSubmit(eventData);
		}

		/// <summary>
		/// Reset the one shots
		/// </summary>
		public void ResetOneShot()
		{
			isDone = false;
		}

		private void OneShotPress()
		{
			if (isDone || !IsActive() || !IsInteractable()) return;

			isDone = true;
			onOneShotClick.Invoke();
		}
	}
}