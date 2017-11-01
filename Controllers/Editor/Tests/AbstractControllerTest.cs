using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DUCK.Controllers.Editor.Tests
{
	[TestFixture]
	public class AbstractControllerTest
	{
		private class TestController : AbstractController
		{
		}

		private AbstractController abstractController;

		[SetUp]
		public void SetUp()
		{
			abstractController = new GameObject().AddComponent<TestController>();
			abstractController.Initialize();
		}

		[Test]
		public void ExpectAddChildControllerToSucceed()
		{
			var childController = abstractController.AddChildController<TestController>();
			Assert.IsNotNull(childController);
			Assert.AreEqual(1, abstractController.ChildCount);
		}

		[Test]
		public void ExpectChildDestroyToSucceed()
		{
			var childController = abstractController.AddChildController<TestController>();
			childController.Destroy();
			Assert.AreEqual(0, abstractController.ChildCount);
		}

		[Test]
		public void ExpectOnDestroyHandlerToBeCalled()
		{
			var isOnDestroyedCalled = false;
			var childController = abstractController.AddChildController<TestController>();
			childController.OnDestroyed += c => { isOnDestroyedCalled = true; };
			childController.Destroy();
			Assert.IsTrue(isOnDestroyedCalled);
		}

		[Test]
		public void ExpectThrowWhenInitializedMoreThanOnce()
		{
			var childController = abstractController.AddChildController<TestController>();
			Assert.Throws<Exception>(() => childController.Initialize());
		}

		[Test]
		public void ExpectAddChildControllerToSetParentOnChild()
		{
			var childController = abstractController.AddChildController<TestController>();
			Assert.IsTrue(childController.HasParent);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(abstractController.gameObject);
		}
	}
}