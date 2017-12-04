using DUCK.Form.Validation;
using NUnit.Framework;

namespace DUCK.Form.Editor.Tests.Validation
{
	public class FieldValidationTests
	{
		private readonly System.Collections.Generic.List<string> blackList = new System.Collections.Generic.List<string>
		{
			"test",
			"test1"
		};

		[Test]
		public void ExpectTextLengthReturnTheCorrectResults()
		{
			Assert.False(TextLengthValidator.CheckStringLengthIsWithRange("test", 1, 2));
			Assert.False(TextLengthValidator.CheckStringLengthIsWithRange("test", 6, 8));
			Assert.True(TextLengthValidator.CheckStringLengthIsWithRange("test", 2, 6));
		}

		[Test]
		public void ExpectCheckIsBlackListedToReturnCorrectResults()
		{
			Assert.True(BlackListValidator.CheckIsBlackListed(blackList, "test"));
			Assert.True(BlackListValidator.CheckIsBlackListed(blackList, "Test", true));
			Assert.False(BlackListValidator.CheckIsBlackListed(blackList, "test0"));
			Assert.False(BlackListValidator.CheckIsBlackListed(blackList, "test0", true));
		}
	}
}