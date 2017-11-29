using System;
using DUCK.Tween.Easings;
using UnityEngine;

namespace DUCK.Tween
{
	/// <summary>
	/// A TimedAnimation is an abstraction use to serve as a base class for any animations that have a duration and are updated on a frame by frame basis.
	/// A TimedAnimation will subscribe to updates using the IAnimationDriver. On each frame the Refresh method is called passing the current progress.
	/// Progress is expressed as a float between 0.0f and 1.0f, where 0.0f is the start and 1.0f is the end. The subclass will use this opportunity to manipulate it's state.
	/// TimedAnimation also provides helpers that track the target object, and the ability to retreive it as a Transform/RecTransform as well as InterpolationHelpers that take care of easing
	/// </summary>
	public abstract class TimedAnimation : AbstractAnimation
	{
		/// <summary>
		/// The animation driver has the responsibility that updating all TimedAnimation.
		/// You can also assign a customised Animation Driver to replace the default one.
		/// </summary>
		public static IAnimationDriver AnimationDriver
		{
			get { return animationDriver ?? (animationDriver = DefaultAnimationDriver.Instance); }
			set { animationDriver = value; }
		}
		private static IAnimationDriver animationDriver;

		/// <summary>
		/// The target gameobject of the this animation
		/// </summary>
		public GameObject TargetObject { get; private set; }

		/// <summary>
		/// The RectTransform of the target game object, null if it doesn't have one
		/// </summary>
		public RectTransform TargetRectTransform { get; private set; }

		/// <summary>
		/// The duration of the timed animation
		/// Duration will always clamp to >= 0
		/// </summary>
		public float Duration
		{
			get { return duration; }
			set { duration = Mathf.Max(0f, value); }
		}
		private float duration;

		/// <summary>
		/// Current time of the animation
		/// </summary>
		public float CurrentTime { get; protected set; }

		/// <summary>
		/// Is the animation play reversed (backward)
		/// </summary>
		public bool IsReversed { get; private set; }

		public override bool IsValid { get { return TargetObject != null || TargetRectTransform != null; } }

		protected Func<float, float> easingFunction;

		private bool IsComplete { get { return IsReversed ? Progress <= 0f : Progress >= 1f; } }
		private float Progress { get { return Mathf.Clamp01(CurrentTime / Duration); } }

		protected TimedAnimation() {}

		protected TimedAnimation(GameObject targetGameObject, float duration, Func<float, float> easingFunction = null)
		{
			TargetObject = targetGameObject;
			Duration = duration;
			this.easingFunction = easingFunction ?? Ease.Linear.None;
		}

		protected TimedAnimation(RectTransform rectTransform, float duration, Func<float, float> easingFunction = null)
			: this(rectTransform.gameObject, duration, easingFunction)
		{
			TargetRectTransform = rectTransform;
		}

		/// <summary>
		/// Starts playback of the animation.
		/// Call it again (anytime) will simply replay the animation.
		/// </summary>
		/// <param name="onComplete">An optional callback invoked when the animation is complete</param>
		/// <param name="onAbort">An optional callback invoked if the animation is aborted</param>
		public override void Play(Action onComplete = null, Action onAbort = null)
		{
			if (!IsValid)
			{
				Debug.LogError("Cannot play TimedAnimation: Invalid target! (GameObject has probably been destroyed)");
				return;
			}

			if (Duration > 0f)
			{
				// A little guard for unnecessary adding to the update list
				if (!IsPlaying)
				{
					AnimationDriver.Add(Update);
				}

				base.Play(onComplete, onAbort);
				Refresh(CurrentTime = IsReversed ? Duration : 0f);
			}
			else
			{
				// If the duration <= 0 we immediately finish the animation
				base.Play(onComplete, onAbort);
				Refresh(1f);
				NotifyAnimationComplete();
			}

		}

		public override void Abort()
		{
			base.Abort();
			AnimationDriver.Remove(Update);
		}

		public override void FastForward()
		{
			// Set the animation state to its final form
			CurrentTime = Duration;
			Refresh(Progress);
			AnimationDriver.Remove(Update);

			base.FastForward();
		}

		/// <summary>
		/// Set the timed animation duration and also keep the current progress.
		/// i.e. ScaleTime(5.0f) will change a 1.0s/2.0s animation to a 2.5s/5.0s animation.
		/// This function having very similar effect of "change the animation playback speed".
		/// </summary>
		/// <param name="duration">The new duration of the animation.</param>
		public void ScaleTime(float duration)
		{
			var currentProgress = Progress;
			Duration = duration;
			CurrentTime = Duration * currentProgress;
		}

		/// <summary>
		/// Completely reverse the original animation.
		/// It can makes the animation play in between "forward" and "backward" at any point.
		/// </summary>
		public void Reverse()
		{
			IsReversed = !IsReversed;
		}

		/// <summary>
		/// Refresh is called on every frame and is used to update the object's state.
		/// </summary>
		/// <param name="progress"></param>
		protected abstract void Refresh(float progress);

		private void Update(float deltaTime)
		{
			if (!IsPlaying || IsPaused) return;

			if (IsValid)
			{
				CurrentTime += IsReversed ? -deltaTime : deltaTime;
				Refresh(Progress);
			}
			else
			{
				CurrentTime = Duration;
				Abort();
				return;
			}

			if (IsComplete)
			{
				if (!IsLooping)
				{
					AnimationDriver.Remove(Update);
				}
				NotifyAnimationComplete();
			}
		}

		//
		// Interpolation helpers
		//

		protected Vector3 Interpolate(Vector3 from, Vector3 to, float t)
		{
			return Vector3.LerpUnclamped(from, to, easingFunction(t));
		}

		protected Color Interpolate(Color from, Color to, float t)
		{
			return Color.LerpUnclamped(from, to, easingFunction(t));
		}

		protected Quaternion Interpolate(Quaternion from, Quaternion to, float t)
		{
			return Quaternion.LerpUnclamped(from, to, easingFunction(t));
		}

		protected float Interpolate(float from, float to, float t)
		{
			return Mathf.LerpUnclamped(from, to, easingFunction(t));
		}
	}
}