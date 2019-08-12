using UnityEngine;

namespace DUCK.Utils
{
	public static class MathUtils
	{
		public const float PI = Mathf.PI;
		public const float PI_2 = Mathf.PI * 2.0f;
		public const float PI_HALF = Mathf.PI * 0.5f;

		/// <summary>
		/// Check if the target integer is an odd number
		/// </summary>
		/// <param name="value">The target integer</param>
		/// <returns>True for odd number</returns>
		public static bool IsOdd(this int value)
		{
			return value % 2 != 0;
		}

		/// <summary>
		/// Check if the target integer is an even number
		/// </summary>
		/// <param name="value">The target integer</param>
		/// <returns>True for even number</returns>
		public static bool IsEven(this int value)
		{
			return value % 2 == 0;
		}

		/// <summary>
		/// Check if the target number is in between two values
		/// </summary>
		/// <param name="value">The target int number</param>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="inclusive">Check if the values are inclusive (or exclusive)</param>
		/// <returns>True if the number is between two values</returns>
		public static bool IsBetween(this int value, int a, int b, bool inclusive = false)
		{
			return ((float)value).IsBetween(a, b, inclusive);
		}

		/// <summary>
		/// Check if the target number is in between two values
		/// </summary>
		/// <param name="value">The target float number</param>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="inclusive">Check if the values are inclusive (or exclusive)</param>
		/// <returns>True if the number is between two values</returns>
		public static bool IsBetween(this float value, float a, float b, bool inclusive = false)
		{
			if (inclusive)
			{
				return a < b ? value >= a && value <= b : value >= b && value <= a;
			}
			// else
			return a < b ? value > a && value < b : value > b && value < a;
		}

		/// <summary>
		///   <para>Calculates the linear parameter t that produces the interpolant value within the range [a, b].</para>
		/// </summary>
		/// <param name="a">From value</param>
		/// <param name="b">To value</param>
		/// <param name="value">Current value</param>
		public static float InverseLerpUnclamped(float a, float b, float value)
		{
			return a == b ? 0.0f : (value - a) / (b - a);
		}

		/// <summary>
		/// Returns a normalized angle that will always be between 0 and 360.
		/// </summary>
		/// <param name="angleInDegrees">Angle in degrees</param>
		/// <returns>The normalized angle</returns>
		public static float NormalizeAngle360(float angleInDegrees)
		{
			return angleInDegrees - 360f * Mathf.Floor(angleInDegrees / 360f);
		}

		/// <summary>
		/// Returns a normalized angle that will always be between 0 and 360.
		/// </summary>
		/// <param name="angles">The Eular angles</param>
		/// <returns>The normalized angles</returns>
		public static Vector3 NormalizeAngle360(Vector3 angles)
		{
			return new Vector3(NormalizeAngle360(angles.x), NormalizeAngle360(angles.y), NormalizeAngle360(angles.z));
		}

		/// <summary>
		/// Returns a normalized angle that will always be between -180 and 180.
		/// </summary>
		/// <param name="angleInDegrees">Angle in degrees</param>
		/// <returns>The normalized angle</returns>
		public static float NormalizeAngle180(float angleInDegrees)
		{
			var normalized = NormalizeAngle360(angleInDegrees);
			return normalized > 180f ? normalized - 360f : normalized;
		}

		/// <summary>
		/// Returns normalized angles that will always be between -180 and 180.
		/// </summary>
		/// /// <param name="angles">The Eular angles</param>
		/// <returns>The normalized angles</returns>
		public static Vector3 NormalizeAngle180(Vector3 angles)
		{
			return new Vector3(NormalizeAngle180(angles.x), NormalizeAngle180(angles.y), NormalizeAngle180(angles.z));
		}

		/// <summary>
		/// Converts Gyroscope Attitude into Unity Rotation
		/// </summary>
		/// <param name="attitude">The Gyroscope Attitude</param>
		/// <returns>Quaternion in Unity Rotation</returns>
		public static Quaternion GyroToUnityRotation(Quaternion attitude)
		{
			return new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);
		}
	}
}
