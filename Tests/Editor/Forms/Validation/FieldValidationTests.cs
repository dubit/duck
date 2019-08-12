﻿using DUCK.Forms.Validation;
using NUnit.Framework;

namespace DUCK.Forms.Editor.Tests.Validation
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
			Assert.False(TextLengthValidator.CheckStringLengthIsWithinRange("test", 1, 2));
			Assert.False(TextLengthValidator.CheckStringLengthIsWithinRange("test", 6, 8));
			Assert.True(TextLengthValidator.CheckStringLengthIsWithinRange("test", 2, 6));
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