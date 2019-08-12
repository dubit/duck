using NUnit.Framework;
using UnityEngine;

namespace DUCK.Utils.Editor.Tests
{
	[TestFixture]
	public class MathUtilsTests
	{
		[Test]
		public void ExpectNormalizeAngle360ToReturnTheCorrectResult()
		{
			// should be between 0 and 360
			var result = MathUtils.NormalizeAngle360(77f);
			Assert.IsTrue(result >= 0f && result <= 360f);

			// should return the correct angle as entered (within range)
			var result2 = MathUtils.NormalizeAngle360(77f);
			Assert.IsTrue(Mathf.Approximately(result2, 77f));

			// should return the normalized angle

			// test various values (within the orginal range, and outside both +/-, and several times higher)
			var result3 = MathUtils.NormalizeAngle360(365f);
			Assert.IsTrue(Mathf.Approximately(result3, 5f));

			var result4 = MathUtils.NormalizeAngle360(762.3334262f);
			Assert.IsTrue(Mathf.Approximately(result4, 42.3334262f));

			var result5 = MathUtils.NormalizeAngle360(-20f);
			Assert.IsTrue(Mathf.Approximately(result5, 340f));
		}

		[Test]
		public void ExpectNormalizeAngle180ToReturnTheCorrectResult()
		{
			// should be between -180 and 180
			var result = MathUtils.NormalizeAngle180(77f);
			Assert.IsTrue(result >= -180f && result <= 180f);

			// should return the normalized angle

			// test various values (within the orginal range, and outside both +/-, and several times higher)
			var result2 = MathUtils.NormalizeAngle180(-77f);
			Assert.IsTrue(Mathf.Approximately(result2, -77f));

			var result3 = MathUtils.NormalizeAngle180(300f);
			Assert.IsTrue(Mathf.Approximately(result3, -60f));

			var result4 = MathUtils.NormalizeAngle180(762.3334262f);
			Assert.IsTrue(Mathf.Approximately(result4, 42.3334262f));
			
			var result5 = MathUtils.NormalizeAngle180(-193.4f);
			Assert.IsTrue(Mathf.Approximately(result5, 166.6f));
		}
	}
}