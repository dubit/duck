using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace DUCK.Localisation.Tests
{
	public class LocalisationTests
	{
		private const string TEST_DATA_PATH = "Packages/com.dubit.duck/Tests/Editor/Localisation/Data";
		private const string TEST_TABLE_ENGLISH = TEST_DATA_PATH + "/TestEnglish";
		private const string TEST_TABLE_FRENCH = TEST_DATA_PATH + "/TestFrench";

		private const string RESOURCES_PATH = "Resources/";

		private bool didResourcesExist;
		private string[] copiedLocalisationTableGuids;

		private string ResourcesDataPath
		{
			get { return Application.dataPath + "/" + RESOURCES_PATH; }
		}

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Assert.IsTrue(AssetDatabase.IsValidFolder(TEST_DATA_PATH));

			didResourcesExist = Directory.Exists(ResourcesDataPath);
			if (!didResourcesExist)
			{
				Directory.CreateDirectory(ResourcesDataPath);
			}

			var localisationTableGuids = AssetDatabase.FindAssets("t: LocalisationTable", new []{ TEST_DATA_PATH });

			copiedLocalisationTableGuids = new string[localisationTableGuids.Length];

			for (var i = 0; i < localisationTableGuids.Length; i++)
			{
				var sourcePath = AssetDatabase.GUIDToAssetPath(localisationTableGuids[i]);
				var destinationPath = "Assets/" + RESOURCES_PATH + Path.GetFileName(AssetDatabase.GUIDToAssetPath(localisationTableGuids[i]));

				if (!AssetDatabase.CopyAsset(sourcePath, destinationPath))
				{
					throw new IOException("Could not create test asset: " + destinationPath);
				}

				copiedLocalisationTableGuids[i] = AssetDatabase.AssetPathToGUID(destinationPath);
			}

			AssetDatabase.Refresh();
		}

		[Test]
		public void ExpectLocaliserToInitialise()
		{
			Assert.DoesNotThrow(() =>
			{
				Localiser.Initialise("", "en-GB");
			});

			Assert.IsTrue(Localiser.Initialised);
		}

		[Test]
		public void ExpectInitialisingWithEmptyFolderToFail()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				Localiser.Initialise("InvalidFolderName_Does_Not_Exist", "en-GB");
			});

			Assert.IsFalse(Localiser.Initialised);
		}

		[Test]
		public void ExpectRetrievingLocalisedStringToWork()
		{
			ExpectLocaliserToInitialise();

			Assert.AreEqual("Hello", TestLoc.Get(TestKeys.Test.Hello));
			Assert.AreEqual("Thanks", TestLoc.Get(TestKeys.Test.ThankYou));
			Assert.AreEqual("Goodbye", TestLoc.Get(TestKeys.Test.Goodbye));
		}

		[Test]
		public void ExpectSwitchingToValidLanguageToWork()
		{
			ExpectLocaliserToInitialise();

			Assert.IsTrue(Localiser.SwitchCulture("fr"));

			Assert.AreEqual("Bonjour", TestLoc.Get(TestKeys.Test.Hello));
			Assert.AreEqual("Merci", TestLoc.Get(TestKeys.Test.ThankYou));
			Assert.AreEqual("Au revoir", TestLoc.Get(TestKeys.Test.Goodbye));
		}

		[Test]
		public void ExpectSwitchingToInvalidLanguageToFail()
		{
			ExpectLocaliserToInitialise();

			Assert.IsFalse(Localiser.SwitchCulture("noSuchLocale"));

			Assert.AreEqual("en-GB", Localiser.CurrentLocale.Name);
		}

		[Test]
		public void ExpectSettingDefaultLanguageToSucceed()
		{
			ExpectLocaliserToInitialise();

			Assert.IsTrue(Localiser.OverrideDefaultCulture("fr"));

			Assert.IsTrue(Localiser.RevertToDefaultCulture());

			Assert.AreEqual("fr", Localiser.CurrentLocale.Name);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			foreach (var guid in copiedLocalisationTableGuids)
			{
				AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
			}

			if (!didResourcesExist)
			{
				Directory.Delete(ResourcesDataPath, recursive: true);
				File.Delete(Application.dataPath + "Resources.meta");
			}

			AssetDatabase.Refresh();
		}
	}
}

