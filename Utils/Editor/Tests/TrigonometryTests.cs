using NUnit.Framework;
using UnityEngine;

namespace DUCK.Utils.Editor.Tests
{
	[TestFixture]
	public class TrigonometryTest
	{
		/* Test triangle: the 30/60/90 triangle
		 *
		 *         30°
		 *         |\
		 *         | \
		 *         |  \
		 *    O    |   \
		 *  (17.3) |    \  H (20)
		 *         |     \
		 *         |      \
		 *         |_______\  60°
		 *            A (10)
		 */
		private readonly float opposite = 17.3205081f;
		private readonly float adjacent = 10;
		private readonly float hypotenuse = 20;
		private readonly float angleAH = 60f;

		[Test(Description = "Expect the correct result from OppositeFromAngleAndAdjacent")]
		public void OppositeFromAngleAndAdjacent()
		{
			var result = Trigonometry.OppositeFromAngleAndAdjacent(angleAH, adjacent);
			Assert.IsTrue(Mathf.Approximately(opposite, result));
		}

		[Test(Description = "Expect the correct result from OppositeFromAngleAndHypotenuse")]
		public void OppositeFromAngleAndHypotenuse()
		{
			var result = Trigonometry.OppositeFromAngleAndHypotenuse(angleAH, hypotenuse);
			Assert.IsTrue(Mathf.Approximately(opposite, result));
		}

		[Test(Description = "Expect the correct result from OppositeFromAngleAndHypotenuse")]
		public void AdjacentFromAngleAndHypotenuse()
		{
			var result = Trigonometry.AdjacentFromAngleAndHypotenuse(angleAH, hypotenuse);
			Assert.IsTrue(Mathf.Approximately(adjacent, result));
		}

		[Test(Description = "Expect the correct result from AdjacentFromAngleAndOpposite")]
		public void AdjacentFromAngleAndOpposite()
		{
			var result = Trigonometry.AdjacentFromAngleAndOpposite(angleAH, opposite);
			Assert.IsTrue(Mathf.Approximately(adjacent, result));
		}

		[Test(Description = "Expect the correct result from HypotenuseFromAngleAndOpposite")]
		public void HypotenuseFromAngleAndOpposite()
		{
			var result = Trigonometry.HypotenuseFromAngleAndOpposite(angleAH, opposite);
			Assert.IsTrue(Mathf.Approximately(hypotenuse, result));
		}

		[Test(Description = "Expect the correct result from HypotenuseFromAngleAndAdjacent")]
		public void HypotenuseFromAngleAndAdjacent()
		{
			var result = Trigonometry.HypotenuseFromAngleAndAdjacent(angleAH, adjacent);
			Assert.IsTrue(Mathf.Approximately(hypotenuse, result));
		}
	}
}