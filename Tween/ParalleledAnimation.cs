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

			if (Animations.Count > 0)
			{
				Animations.ForEach(animation => animation.Play(HandleEachAnimationCompleted, () =>
				{
					if (!animation.IsValid)
					{
						Animations.Remove(animation);
					}
				}));
			}
			// In case of all animations have been removed
			else
			{
				Abort();
			}
		}

		private void HandleEachAnimationCompleted()
		{
			// Bump up the number and check if all the animations are finished or not
			if (++NumberOfAnimationsCompleted == Animations.Count)
			{
				NotifyAnimationComplete();
			}
		}
	}
}
