using System;
using UnityEngine;

namespace DUCK.Tween
{
	/// <summary>
	/// Encapsulates an AbstractAnimation, which is only created at the time it is requested to play.
	/// Useful for AbstractAnimations which depend on an object being in a certain state other than the state in which the abstractAnimations are originally created
	/// </summary>
	public class DelegateAnimation<T> : AbstractAnimation where T : AbstractAnimation
	{
		public T Animation { get; private set; }

		/// <summary>
		/// Delegate animation is always valid before you created the child animation;
		/// Then the validation will be relied on the created animation.
		/// </summary>
		public override bool IsValid { get { return Animation == null || Animation.IsValid; } }

		private readonly Func<T> animationCreationFunction;

		/// <summary>
		/// Creates a new DelegateAnimation
		/// </summary>
		/// <param name="animationCreationFunction">A function that will create the animation when called</param>
		public DelegateAnimation(Func<T> animationCreationFunction)
		{
			if (animationCreationFunction == null)
			{
				throw new ArgumentNullException("animationCreationFunction");
			}
			this.animationCreationFunction = animationCreationFunction;
		}

		public override void Play(Action onComplete)
		{
			base.Play(onComplete);

			try
			{
				Animation = animationCreationFunction();
				if (Animation.IsValid)
				{
					Animation.Play(NotifyAnimationComplete);
				}
				else
				{
					base.FastForward();
				}
			}
			catch (MissingReferenceException e)
			{
				Debug.LogWarning("DelegateAnimation: Your GameObject has probably been destroyed!");
				Debug.LogWarning(e.Message);
				base.FastForward();
			}
			catch (Exception)
			{
				Debug.LogError("DelegateAnimation: Cannot create your animation!");
				throw;
			}
		}

		public override void Abort()
		{
			if (Animation != null)
			{
				Animation.Abort();
			}
			base.Abort();
		}

		public override void FastForward()
		{
			if (Animation != null)
			{
				Animation.FastForward();
			}
			else
			{
				Debug.LogError("DelegateAnimation: You cannot fast forward because you haven't started your delegate animation yet!");
			}
			base.FastForward();
		}

		public override void Pause()
		{
			base.Pause();
			if (Animation != null)
			{
				Animation.Pause();
			}
		}

		public override void Resume()
		{
			base.Resume();
			if (Animation != null)
			{
				Animation.Resume();
			}
			else
			{
				Debug.LogError("DelegateAnimation: You cannot resume this animation because you haven't started your delegate animation yet!");
			}
		}
	}
}