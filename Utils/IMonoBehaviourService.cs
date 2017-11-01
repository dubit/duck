using System;

namespace DUCK.Utils
{
	public interface IMonoBehaviourService
	{
		event Action OnUpdate;
		event Action OnLateUpdate;
		event Action OnFixedUpdate;

		event Action<bool> ApplicationPause;
		event Action ApplicationQuit;
		event Action<bool> ApplicationFocus;

		event Action OnLevelLoaded;
	}
}
