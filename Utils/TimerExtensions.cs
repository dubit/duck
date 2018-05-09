namespace DUCK.Utils
{
	public static class TimerExtensions
	{
		/// <summary>
		/// Stops a timer if the reference is valid and it is running.
		/// </summary>
		/// <param name="timer">The target Timer</param>
		public static void SafeStop(this Timer timer)
		{
			if (timer != null && timer.IsRunning)
			{
				timer.Stop();
			}
		}
	}
}
