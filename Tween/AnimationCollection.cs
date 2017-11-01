using System;
using System.Collections.Generic;

namespace DUCK.Tween
{
	/// <summary>
	/// AnimationCollection is a type of animation that acts as a container for a set of other animations.
	/// When played, each animation will play in sequence/parallel until all have been completed. An Animation in
	/// an AnimationCollection can also be itself an AnimationCollection
	/// </summary>
	public abstract class AnimationCollection : AbstractAnimation
	{
		public override bool IsValid { get { return Animations.Count > 0; } }

		protected List<AbstractAnimation> Animations { get; private set; }
		protected int NumberOfAnimationsCompleted { get; set; }

		protected AnimationCollection()
		{
			Animations = new List<AbstractAnimation>();
			NumberOfAnimationsCompleted = 0;
		}

		/// <summary>
		/// Adds one or more animations to this AnimationCollection
		/// </summary>
		/// <param name="animations">One or more AbstractAnimations to add to this AnimationCollection</param>
		/// <returns>Returns this AnimationCollection to comply with fluent interface</returns>
		public AnimationCollection Add(params AbstractAnimation[] animations)
		{
			Animations.AddRange(animations);
			return this;
		}

		/// <summary>
		/// Starts playback of the AnimationCollection starting at the first animation
		/// </summary>
		/// <param name="onComplete">An optional callback invoked when the AnimationCollection is complete</param>
		public override void Play(Action onComplete)
		{
			base.Play(onComplete);
			NumberOfAnimationsCompleted = 0;
			PlayQueuedAnimations();
		}

		/// <summary>
		/// Aborts the AnimationCollection
		/// </summary>
		public override void Abort()
		{
			foreach (var animation in Animations)
			{
				animation.Abort();
			}
			base.Abort();
		}

		/// <summary>
		/// Fastforwards the AnimationCollection to the end and stops it.
		/// This will result in each animation in the collection having .FastForward() called on it.
		/// meaning each animation will fast forward, and end up in the state it would be in after the final frame of animation
		/// </summary>
		public override void FastForward()
		{
			foreach (var animation in Animations)
			{
				animation.FastForward();
			}
			base.FastForward();
		}

		/// <summary>
		/// Pause all the animations which is currently playing
		/// </summary>
		public override void Pause()
		{
			base.Pause();
			foreach (var animation in Animations)
			{
				if (animation.IsPlaying)
				{
					animation.Pause();
				}
			}
		}

		/// <summary>
		/// Resume all the paused animations.
		/// </summary>
		public override void Resume()
		{
			base.Resume();
			foreach (var animation in Animations)
			{
				if (animation.IsPlaying)
				{
					animation.Resume();
				}
			}
		}

		/// <summary>
		/// This function decides how we handle the animations in the queue (collection).
		/// Override this function for different playback strategies.
		/// </summary>
		protected abstract void PlayQueuedAnimations();
	}
}
