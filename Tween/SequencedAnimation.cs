using UnityEngine;

namespace DUCK.Tween
{
	/// <summary>
	/// SequencedAnimation is used to play a set of animations in sequence.
	/// It will not complete until all child animations in the queue have completed.
	/// </summary>
	public class SequencedAnimation : AnimationCollection
	{
		/// <summary>
		/// Get the current playing animation in the queue.
		/// </summary>
		public AbstractAnimation GetCurrentAnimation()
		{
			return Animations[Mathf.Clamp(NumberOfAnimationsCompleted, 0, Animations.Count -1)];
		}

		protected override void PlayQueuedAnimations()
		{
			if (Animations.Count > NumberOfAnimationsCompleted)
			{
				var nextAnimation = Animations[NumberOfAnimationsCompleted];
				if (nextAnimation.IsValid)
				{
					nextAnimation.Play(() =>
					{
						NumberOfAnimationsCompleted++;
						PlayQueuedAnimations();
					},
					() =>
					{
						Animations.Remove(nextAnimation);
						PlayQueuedAnimations();
					});
				}
				else
				{
					Animations.Remove(nextAnimation);
					PlayQueuedAnimations();
				}
			}
			else
			{
				NotifyAnimationComplete();
			}
		}

		public override void Reverse()
		{
			base.Reverse();
			NumberOfAnimationsCompleted = Animations.Count - 1 - NumberOfAnimationsCompleted;
		}
	}
}