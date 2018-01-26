using UnityEngine;

namespace DUCK.Tween.Easings
{
	public static partial class Ease
	{
		public static float Parabola(float x) 
		{
			return 1f - Mathf.Pow((2f * x) - 1f, 2);
		}
	}
}