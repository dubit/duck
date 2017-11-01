using System;
using UnityEngine;

namespace DUCK.Tween
{
	/// <summary>
	/// Custom Animation will simply supplies the interpolated value to the user via an Action function.
	/// </summary>
	public class CustomAnimation : TimedAnimation
	{
		public float CurrentValue { get; private set; }
		public float From { get; private set; }
		public float To { get; private set; }

		/// <summary>
		/// Custom Animation is always true because customUpdate cannot be null
		/// </summary>
		public override bool IsValid { get { return true; } }

		private readonly Action<float> customUpdate;

		/// <summary>
		/// Create a new Custom Animation.
		/// The interpolated value will be passed into the custom update function.
		/// </summary>
		/// <param name="customUpdate">A Custom Update function</param>
		/// <param name="from">From value</param>
		/// <param name="to">To value</param>
		/// <param name="duration">Duration of the animation</param>
		/// <param name="easingFunction">The easing function for the interpolation</param>
		public CustomAnimation(Action<int> customUpdate, int from, int to, float duration = 1.0f, Func<float, float> easingFunction = null)
			: this((float value) => customUpdate((int)value), from, to, duration, easingFunction) {}

		/// <summary>
		/// Create a new Custom Animation.
		/// The interpolated value will be passed into the custom update function.
		/// </summary>
		/// <param name="customUpdate">A Custom Update function</param>
		/// <param name="from">From value</param>
		/// <param name="to">To value</param>
		/// <param name="duration">Duration of the animation</param>
		/// <param name="easingFunction">The easing function for the interpolation</param>
		public CustomAnimation(Action<float> customUpdate, float from = 0, float to = 1.0f, float duration = 1.0f, Func<float, float> easingFunction = null)
			: base((GameObject)null, duration, easingFunction)
		{
			if (customUpdate == null)
			{
				throw new ArgumentNullException("customUpdate");
			}
			this.customUpdate = customUpdate;
			From = from;
			To = to;
		}

		protected override void Refresh(float progress)
		{
			customUpdate(CurrentValue = Interpolate(From, To, progress));
		}
	}
}