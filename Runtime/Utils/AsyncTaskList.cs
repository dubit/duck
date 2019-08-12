using System;
using System.Collections.Generic;

namespace DUCK.Utils
{
	public class AsyncTaskList
	{
		private readonly Queue<Action<Action>> tasks = new Queue<Action<Action>>();
		private bool hasRunBeenCalled;

		/// <summary>
		/// Adds an async task to the task list. The Action provided takes another action called next.
		/// Call this next function to indicate the task is complete and the next one in the chain, should be called
		/// Eg:
		/// taskList.Add(next => {
		///   // do some work, then call next when complete.
		///   next();
		/// });
		/// </summary>
		/// <param name="taskFunction">The task function</param>
		public void Add(Action<Action> taskFunction)
		{
			tasks.Enqueue(taskFunction);
		}

		/// <summary>
		/// Adds a synchronous task to the task list.
		/// This is a simple function that is executed as part of async task list.
		/// After the function has been called, the next task will begin instantly
		/// </summary>
		/// <param name="taskFunction">The task function</param>
		public void Add(Action taskFunction)
		{
			Add(next =>
			{
				taskFunction();
				next();
			});
		}

		/// <summary>
		/// Runs the task list, in order and calls the onComplete callback when all tasks have been completed.
		/// </summary>
		/// <param name="onComplete">Optional on complete callback</param>
		public void Run(Action onComplete = null)
		{
			if (hasRunBeenCalled)
			{
				throw new Exception("Cannot run more than once on an instance of AsyncTaskList");
			}

			hasRunBeenCalled = true;
			RunNextTask(onComplete);
		}

		private void RunNextTask(Action onComplete)
		{
			if (tasks.Count > 0)
			{
				var nextTask = tasks.Dequeue();

				nextTask(() =>
				{
					RunNextTask(onComplete);
				});
			}
			else
			{
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}
	}
}