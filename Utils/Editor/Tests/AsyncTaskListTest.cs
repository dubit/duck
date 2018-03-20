using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DUCK.Utils.Editor.Tests
{
	[TestFixture]
	public class AsyncTaskListTest
	{
		[Test]
		public void ExpectConstructorToNotThrow()
		{
			Assert.DoesNotThrow(() =>
			{
				var tasks = new AsyncTaskList();
			});
		}

		[Test]
		public void ExpectAddNotToThrow()
		{
			Assert.DoesNotThrow(() =>
			{
				var tasks = new AsyncTaskList();
				tasks.Add(() => { });
			});
		}

		[Test]
		public void ExpectSyncTaskToBeCalled()
		{
			var wasCalled = false;

			var tasks = new AsyncTaskList();
			tasks.Add(() => { wasCalled = true; });
			tasks.Run();

			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void ExpectAsyncTaskToBeCalled()
		{
			var wasCalled = false;

			var tasks = new AsyncTaskList();
			tasks.Add((next) =>
			{
				wasCalled = true;
				next();
			});

			Assert.IsFalse(wasCalled);

			tasks.Run();

			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void ExpectAllTasksToRunSequentially()
		{
			// test that the execution order is correct

			var resultList = new List<int>();

			Func<int, Action> createTask = (number) =>
			{
				return () =>
				{
					resultList.Add(number);
				};
			};

			var tasks = new AsyncTaskList();
			tasks.Add(createTask(0));
			tasks.Add(createTask(1));
			tasks.Add(createTask(2));
			tasks.Add(createTask(3));
			tasks.Run();

			Assert.AreEqual(resultList[0], 0);
			Assert.AreEqual(resultList[1], 1);
			Assert.AreEqual(resultList[2], 2);
			Assert.AreEqual(resultList[3], 3);
		}

		[Test]
		public void ExpectAsyncTasksOnlyToContinueAfterNextIsCalled()
		{
			Action goToNextTask = null;
			var hasStartedTask1 = false;
			var hasCompleteTask1 = false;

			var tasks = new AsyncTaskList();

			// add task 1 (async)
			tasks.Add(next =>
			{
				hasStartedTask1 = true;
				// Do some work, and when we have done then we will call next
				goToNextTask = next;
			});

			// add task 2
			tasks.Add(() =>
			{
				hasCompleteTask1 = true;
			});

			// run the tasks.
			tasks.Run();

			// We should now be waiting for task 1 to complete
			Assert.IsTrue(hasStartedTask1);
			Assert.IsFalse(hasCompleteTask1);

			// now let's complete task 1
			goToNextTask();

			// We should now have completed all tasks.
			Assert.IsTrue(hasCompleteTask1);
		}

		[Test]
		public void ExpectOnCompleteCallbackToBeCalledAfterAllTasksAreComplete()
		{
			Action goToNextTask = null;
			var wasOnCompleteCalled = false;

			var tasks = new AsyncTaskList();

			tasks.Add(next =>
			{
				// Do some work, and when we have done then we will call next
				goToNextTask = next;
			});

			// run the tasks.
			tasks.Run(() =>
			{
				wasOnCompleteCalled = true;
			});

			Assert.IsFalse(wasOnCompleteCalled);

			// now let's complete task 1, which should call on complete
			goToNextTask();

			Assert.IsTrue(wasOnCompleteCalled);
		}

		[Test]
		public void ExpectToThrowWhenCallingRunTwiceInParallel()
		{
			var tasks = new AsyncTaskList();

			tasks.Add(next =>
			{
				// Do nothing here to leave this task list suspended
			});

			// run the tasks.
			tasks.Run();

			// now it should throw as we attempt to run it again
			Assert.Throws<Exception>(() =>
			{
				tasks.Run();
			});
		}
	}
}