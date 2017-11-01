using System;
using UnityEngine;

namespace DUCK.DebugMenu
{
	/// <summary>
	/// A simple implementation of IDebugMenuSummoner, that summons the debug menu when F7 is pressed, or on a device,
	/// when 6 fingers are pressed down
	/// </summary>
	public class DefaultDebugMenuSummoner : MonoBehaviour, IDebugMenuSummoner
	{
		private const int TOUCH_AMOUNT = 6;
		private const KeyCode ACTIVATE_KEY = KeyCode.F7;

		public event Action OnSummonRequested;
		
		public void Update()
		{
			if (Input.touchCount == TOUCH_AMOUNT || Input.GetKeyDown(ACTIVATE_KEY))
			{
				if (OnSummonRequested != null)
				{
					OnSummonRequested.Invoke();
				}
			}
		}
	}
}