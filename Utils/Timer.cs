using System;
using UnityEngine;

namespace DUCK.Utils
{
	/// <summary>
	/// A self updating timer that invokes actions when finished
	/// </summary>
	public sealed class Timer
	{
		/// <summary>
		/// Invokes a callback after the specified duration.
		/// </summary>
		/// <param name="duration">The timer will run for this amount of time</param>
		/// <param name="onComplete">Will be invoked as soon as the timer ends</param>
		/// <returns>The instance of Timer</returns>
		public static Timer SetTimeout(float duration, Action onComplete = null)
		{
			var timer = new Timer(duration);
			timer.Start(onComplete);
			return timer;
		}

		/// <summary>
		/// The amount of loops left to run.
		/// 0 means no loops left; -1 means infinite loop.
		/// Will clamp to -1 if less than -1.
		/// </summary>
		public int LoopCount { get { return loopCount; } set { loopCount = Mathf.Max(value, -1); } }
		private int loopCount;

		/// <summary>
		/// The duration of the timer.
		/// Duration less than 0 or equal to 0 will make the timer finish instantly.
		/// </summary>
		public float Duration
		{
			get { return duration; }
			set
			{
				if (value < 0f) Debug.LogWarning("Time duration should not be negative.");
				duration = value;
			}
		}
		private float duration;

		/// <summary>
		/// Define if the timer will affect by the Time.timeScale or not.
		/// </summary>
		public bool Scaleable { get; set; }

		public float TimeLeft { get; private set; }
		public float ElapsedTime { get { return Duration - TimeLeft; } }
		public bool IsLooping { get { return LoopCount > 0 || LoopCount < 0; } }
		public bool IsRunning { get { return TimeLeft > 0 && !IsPaused; } }
		public bool IsPaused { get; private set; }

		private readonly MonoBehaviourService monoService;
		private bool isUpdating;
		private Action onLoopComplete;
		private Action onComplete;

		/// <summary>
		/// Creates a new timer but does not start it.
		/// </summary>
		public Timer(float duration)
		{
			TimeLeft = Duration = duration;
			monoService = MonoBehaviourService.Instance;
		}

		/// <summary>
		/// Start the timer.
		/// Will reset the timer anyhow.
		/// </summary>
		/// <param name="onComplete">Call back when the timer finished</param>
		public void Start(Action onComplete = null)
		{
			this.onComplete = onComplete;
			IsPaused = false;
			Reset();
			AddToUpdateQueue();
		}

		/// <summary>
		/// Start the timer.
		/// Override the existing duration.
		/// Will reset the timer anyhow.
		/// </summary>
		/// <param name="duration">The new duration of the timer</param>
		/// <param name="onComplete">Call back when the timer finished</param>
		public void Start(float duration, Action onComplete = null)
		{
			Duration = duration;
			Start(onComplete);
		}

		/// <summary>
		/// Start the timer with loops.
		/// Will reset the timer anyhow.
		/// </summary>
		/// <param name="loopCount">Total loop times. 0 is no loop, -1 is infinite loops.</param>
		/// <param name="onLoopComplete">Call back on the end of each loops</param>
		/// <param name="onComplete">Call back when the timer finished</param>
		public void Start(int loopCount, Action onLoopComplete = null, Action onComplete = null)
		{
			Start(Duration, loopCount, onLoopComplete, onComplete);
		}

		/// <summary>
		/// Start the timer with loops.
		/// Override the existing duration.
		/// Will reset the timer anyhow.
		/// </summary>
		/// <param name="duration">The new duration of the timer</param>
		/// <param name="loopCount">Total loop times. 0 is no loop, -1 is infinite loops.</param>
		/// <param name="onLoopComplete">Call back on the end of each loops</param>
		/// <param name="onComplete">Call back when the timer finished</param>
		public void Start(float duration, int loopCount, Action onLoopComplete = null, Action onComplete = null)
		{
			LoopCount = loopCount;
			this.onLoopComplete = onLoopComplete;
			Start(duration, onComplete);
		}

		/// <summary>
		/// Stops the timer without invoke any onComplete callbacks.
		/// Will empty all loops too.
		/// </summary>
		public void Stop()
		{
			TimeLeft = 0;
			LoopCount = 0;
			RemoveFromUpdateQueue();
		}

		/// <summary>
		/// Pauses the timer
		/// </summary>
		public void Pause()
		{
			IsPaused = true;
			RemoveFromUpdateQueue();
		}

		/// <summary>
		/// Unpause the timer
		/// </summary>
		public void Unpause()
		{
			IsPaused = false;
			AddToUpdateQueue();
		}

		/// <summary>
		/// Reset the timer, back to its full duration.
		/// This function will not affact loop counts.
		/// In addition, this function will not stop, pause or unpause the timer.
		/// </summary>
		public void Reset()
		{
			TimeLeft = Duration;
		}

		private void AddToUpdateQueue()
		{
			if (!isUpdating)
			{
				isUpdating = true;
				monoService.OnUpdate += OnUpdateHandler;
			}
		}

		private void RemoveFromUpdateQueue()
		{
			isUpdating = false;
			monoService.OnUpdate -= OnUpdateHandler;
		}

		private void OnUpdateHandler()
		{
			if (IsRunning)
			{
				TimeLeft -= Scaleable ? Time.deltaTime : Time.unscaledDeltaTime;
				if (TimeLeft <= 0f)
				{
					TimeLeft = 0f;
					if (IsLooping)
					{
						// Faster than LoopCount--, stop arguing with this little piece of fancy code
						--LoopCount;
						onLoopComplete.SafeInvoke();

						// All loops are finished
						if (LoopCount == 0)
						{
							Stop();
							onComplete.SafeInvoke();
						}
						// Start next loop
						else
						{
							Reset();
						}
					}
					else
					{
						Stop();
						onComplete.SafeInvoke();
					}
				}
			}
			// This should not occured since we are not waiting for the next update
			// The timer should have been stopped already
			else
			{
				Stop();
			}
		}
	}
}