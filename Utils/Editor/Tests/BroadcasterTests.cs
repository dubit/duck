using NUnit.Framework;
using UnityEngine;

namespace DUCK.Utils.Editor.Tests
{
	interface IEventHandler
	{
		void HandleTheEvent();
	}

	class TestBehaviour : MonoBehaviour, IEventHandler
	{
		public bool WasCalled { get; private set; }

		public void HandleTheEvent()
		{
			WasCalled = true;
		}
	}

	[TestFixture]
	public class BroadcasterTests
	{
		[Test]
		public void ExpectBroadcasterToExecuteFunctionOnTargetObject()
		{
			var obj = new GameObject();
			var behaviour = obj.AddComponent<TestBehaviour>();

			Broadcaster.BroadcastTo<IEventHandler>(obj, o => o.HandleTheEvent());

			Assert.IsTrue(behaviour.WasCalled);

			Object.DestroyImmediate(obj);
		}

		[Test]
		public void ExpectBroadcasterToExecuteAllFunctionsOnTargetObject()
		{
			var obj = new GameObject();
			var behaviour = obj.AddComponent<TestBehaviour>();
			var behaviour2 = obj.AddComponent<TestBehaviour>();

			Broadcaster.BroadcastTo<IEventHandler>(obj, o => o.HandleTheEvent());

			Assert.IsTrue(behaviour.WasCalled);
			Assert.IsTrue(behaviour2.WasCalled);

			Object.DestroyImmediate(obj);
		}

		[Test]
		public void ExpectBroadcasterToExecuteFunctionOnChildObject()
		{
			var obj = new GameObject();
			var child = new GameObject();
			child.transform.SetParent(obj.transform);
			var behaviour = child.AddComponent<TestBehaviour>();

			Broadcaster.BroadcastTo<IEventHandler>(obj, o => o.HandleTheEvent());

			Assert.IsTrue(behaviour.WasCalled);

			Object.DestroyImmediate(obj);
		}

		[Test]
		public void ExpectBroadcasterNotToExecuteFunctionOnChildObjectIfParameterWasSet()
		{
			var obj = new GameObject();
			var child = new GameObject();
			child.transform.SetParent(obj.transform);
			var behaviour = child.AddComponent<TestBehaviour>();

			Broadcaster.BroadcastTo<IEventHandler>(obj, o => o.HandleTheEvent(), false);

			Assert.IsFalse(behaviour.WasCalled);

			Object.DestroyImmediate(obj);
		}
	}
}