using UnityEngine;

namespace DUCK.Utils
{
	public static class Trigonometry
	{
		/// <summary>
		/// Calculates and returns the length of the opposite side of a right angled triangle given the angle and adjacent side.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="adjacent">Length of the side adjacent to the angle</param>
		/// <returns>The length of the opposite side from the angle</returns>
		public static float OppositeFromAngleAndAdjacent(float angleInDegrees, float adjacent)
		{
			return Mathf.Tan(angleInDegrees * Mathf.Deg2Rad) * adjacent;
		}

		/// <summary>
		/// Calculates and returns the length of the opposite side of a right angled triangle given the angle and hypotenuse side.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="hypotenuse">Length of the hypotenuse</param>
		/// <returns>The length of the opposite side from the angle</returns>
		public static float OppositeFromAngleAndHypotenuse(float angleInDegrees, float hypotenuse)
		{
			return Mathf.Sin(angleInDegrees * Mathf.Deg2Rad) * hypotenuse;
		}

		/// <summary>
		/// Calculates and returns the length of the adjacent side of a right angled triangle given the angle and hypotenuse side.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="hypotenuse">Length of the hypotenuse</param>
		/// <returns>The length of the adjacent side side to the angle</returns>
		public static float AdjacentFromAngleAndHypotenuse(float angleInDegrees, float hypotenuse)
		{
			return Mathf.Cos(angleInDegrees * Mathf.Deg2Rad) * hypotenuse;
		}

		/// <summary>
		/// Calculates and returns the length of the adjacent side of a right angled triangle given the angle and opposite side.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="opposite">Length of the side opposite from the angle</param>
		/// <returns>The length of the adjacent side to the angle</returns>
		public static float AdjacentFromAngleAndOpposite(float angleInDegrees, float opposite)
		{
			return opposite / Mathf.Tan(angleInDegrees * Mathf.Deg2Rad);
		}

		/// <summary>
		/// Calculates and returns the length of the hypotenuse side of a right angled triangle given the angle and opposite sides.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="opposite">Length of the side opposite to the angle</param>
		/// <returns>The length of the hypotenuse</returns>
		public static float HypotenuseFromAngleAndOpposite(float angleInDegrees, float opposite)
		{
			return opposite / Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
		}
		
		/// <summary>
		/// Calculates and returns the length of the hypotenuse side of a right angled triangle given the angle and adjacent sides.
		/// </summary>
		/// <param name="angleInDegrees">Angle of the side AH (Adjacent/Hypotenuse) in degrees</param>
		/// <param name="adjacent">Length of the side adjacent to the angle</param>
		/// <returns>The length of the hypotenuse</returns>
		public static float HypotenuseFromAngleAndAdjacent(float angleInDegrees, float adjacent)
		{
			return adjacent / Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
		}
	}
}