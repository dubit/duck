using System;

namespace DUCK.DebugMenu
{
	/// <summary>
	/// IDebugMenuSummoner represents any object that can be used to summon the debug menu,
	/// add these to the DebugMenu using the AddSummoner function, then invoke this event to summon the debug menu.
	/// </summary>
	public interface IDebugMenuSummoner
	{
		event Action OnSummonRequested;
	}
}