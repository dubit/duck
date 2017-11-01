using System;
using System.Collections.Generic;
using DUCK.Utils;

namespace DUCK.Controllers
{
	/// <summary>
	/// Basic state machine for AbstractControllers
	/// </summary>
	public class StateMachine : AbstractController
	{
		public AbstractStateController CurrentStateController { get; private set; }

		private Dictionary<Type, Action> triggers;

		public event Action<AbstractStateController> OnStateChanged;

		public override void Initialize(AbstractController parent = null, object args = null)
		{
			base.Initialize(parent, args);
			triggers = new Dictionary<Type, Action>();
		}

		public void AddStateChangeTrigger<T>(Action trigger) where T : AbstractStateController
		{
			if (triggers.ContainsKey(typeof(T)))
			{
				throw new Exception("StateMachine - Trigger for state already added");
			}
			triggers.Add(typeof(T), trigger);
		}

		public void RemoveStateChangedTrigger<T>() where T : AbstractStateController
		{
			if (!triggers.ContainsKey(typeof(T)))
			{
				throw new Exception("StateMachine - Trigger for state does not exist");
			}
			triggers.Remove(typeof(T));
		}

		/// <summary>
		/// Deactivates the current state and creates the new state.
		/// </summary>
		/// <typeparam name="T">The new state to be created</typeparam>
		/// <param name="args">Arguments to be passed to the new state</param>
		public T SwitchState<T>(object args = null) where T : AbstractStateController
		{
			if (CurrentStateController != null)
			{
				CurrentStateController.Destroy();
			}

			CurrentStateController = AddChildController<T>(args);
			OnStateChanged.SafeInvoke(CurrentStateController);
			GetTriggerForState<T>().SafeInvoke();

			return (T) CurrentStateController;
		}

		/// <summary>
		/// Deactivates the current state and creates the new state.
		/// </summary>
		/// <typeparam name="T">The new state to be created</typeparam>
		/// <param name="resourcePath">Resource path of the new state</param>
		/// <param name="args">Arguments to be passed to the new state</param>
		public T SwitchState<T>(string resourcePath, object args = null, bool worldPositionStays = true)
			where T : AbstractStateController
		{
			if (CurrentStateController != null)
			{
				CurrentStateController.Destroy();
			}

			CurrentStateController = AddResourceChildController<T>(resourcePath, args, worldPositionStays);
			OnStateChanged.SafeInvoke(CurrentStateController);
			GetTriggerForState<T>().SafeInvoke();

			return (T) CurrentStateController;
		}

		private Action GetTriggerForState<T>() where T : AbstractStateController
		{
			Action action = null;
			if (triggers.ContainsKey(typeof(T)))
			{
				action = triggers[typeof(T)];
			}
			return action;
		}
	}
}
