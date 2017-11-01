namespace DUCK.Tween
{
	/// <summary>
	/// ParalleledAnimation is used to play a set of animations in parallel.
	/// The animations maybe different durations, or sequences themselves. 
	/// It will not complete until all child animations in the queue have completed.
	/// </summary>
	public class ParalleledAnimation : AnimationCollection
	{
		protected override void PlayQueuedAnimations()
		{
			Animations.RemoveAll(animation => !animation.IsValid);
			Animations.ForEach(animation => animation.Play(HandleEachAnimationComplete));

			// In case of all animations have been removed
			if (Animations.Count == 0)
			{
				HandleEachAnimationComplete();
			}
		}

		private void HandleEachAnimationComplete()
		{
			// Bump up the number and check if all the animations are finished or not
			if (++NumberOfAnimationsCompleted >= Animations.Count)
			{
				NotifyAnimationComplete();
			}
		}
	}
}
