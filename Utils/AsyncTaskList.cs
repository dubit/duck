using System;
using System.Collections.Generic;

namespace DUCK.Utils
{
	public class AsyncTaskList
	{
		private readonly List<Action<Action>> tasks = new List<Action<Action>>();
		private int currentTaskIndex;

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
			tasks.Add(taskFunction);
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
			currentTaskIndex = 0;
			RunNextTask(onComplete);
		}

		private void RunNextTask(Action onComplete)
		{
			if (currentTaskIndex < tasks.Count)
			{
				tasks[currentTaskIndex](() =>
				{
					currentTaskIndex++;
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