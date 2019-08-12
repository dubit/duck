using System;

namespace DUCK.Controllers
{
	public abstract class AbstractStateController : AbstractController
	{
		public StateMachine StateMachine { get; private set; }

		public override void Initialize(AbstractController parent = null, object args = null)
		{
			base.Initialize(parent, args);
			StateMachine = parent as StateMachine;
			if (StateMachine == null)
			{
				throw new Exception("State controller: "+ name + " should be instantiated by a state machine!");
			}
		}
	}
}
