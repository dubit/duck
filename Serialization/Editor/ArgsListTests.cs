﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DUCK.Serialization.Editor
{
	[TestFixture]
	public class ArgsListTests
	{
		[Test]
		public void ExpectConstructorNotToThrow()
		{
			Assert.DoesNotThrow(() => { new ArgsList(); });
		}

		[Test]
		public void ExpectConstructorWithArgsNotToThrowWithValidTypes()
		{
			Assert.DoesNotThrow(() => { new ArgsList(typeof(string), typeof(int)); });
		}

		[Test]
		public void ExpectConstructorWithArgsToThrowWithInvalidTypes()
		{
			Assert.Throws<ArgumentException>(() => { new ArgsList(typeof(ArgsList)); });
		}
		
		[Test]
		public void ExpectSetTypesToThrowWithNull()
		{
			var argsList = new ArgsList();
			Assert.Throws<ArgumentNullException>(() => { argsList.SetTypes(null); });
		}

		[Test]
		public void ExpectSetTypesToThrowWithEmptyList()
		{
			var argsList = new ArgsList();
			Assert.Throws<ArgumentException>(() => { argsList.SetTypes(new List<Type>()); });
		}

		[Test]
		public void ExpectSetTypesToThrowWithInvalidTypes()
		{
			var argsList = new ArgsList();
			Assert.Throws<ArgumentException>(() => { argsList.SetTypes(new List<Type> {typeof(ArgsList)}); });
		}

		[Test]
		public void ExpectSetTypesNotToThrowWithValidTypes()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});
		}

		[Test]
		public void ExpectSetTypesToSetTheArgTypesProperty()
		{
			var argsList = new ArgsList();
			var argTypes = new List<Type> {typeof(string), typeof(string), typeof(int)};
			argsList.SetTypes(argTypes);

			Assert.AreEqual(argsList.ArgTypes.Count, argTypes.Count);
			for (var i = 0; i < argTypes.Count; i++)
			{
				Assert.AreEqual(argTypes[i], argsList.ArgTypes[i]);
			}
		}

		[Test]
		public void ExpectSetToThrowIfIndexIsOutOfBounds()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(int), typeof(int), typeof(int)});

			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList.Set(5, 10); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList.Set(-5, 10); });
		}

		[Test]
		public void ExpectSetToThrowIfIndexIsForWrongType()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			Assert.Throws<ArgumentException>(() => { argsList.Set(0, 10); });
		}

		[Test]
		public void ExpectGetToThrowIfIndexIsOutOfBounds()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList.Get<string>(5); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList.Get<string>(-5); });
		}

		[Test]
		public void ExpectGetToThrowIfIndexIsForWrongType()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});
			Assert.Throws<ArgumentException>(() => { argsList.Get<int>(0); });
		}

		[Test]
		public void ExpectGetToReturnValuePassedIntoSetAtSameIndex()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			var value = "foobar";
			argsList.Set(0, value);
			var result = argsList.Get<string>(0);
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectSetIndexerToThrowIfIndexIsOutOfBounds()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(int), typeof(int), typeof(int)});

			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList[5] = 10; });
			Assert.Throws<ArgumentOutOfRangeException>(() => { argsList[-5] = 10; });
		}

		[Test]
		public void ExpectSetIndexerToThrowIfIndexIsForWrongType()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			Assert.Throws<ArgumentException>(() => { argsList[0] = 10; });
		}

		[Test]
		public void ExpectSetIndexerToThrowWhenNullIsUsedAgainstValueTypes()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(int), typeof(int), typeof(int)});

			Assert.Throws<ArgumentException>(() => { argsList[1] = null; });
		}
		
		[Test]
		public void ExpectSetIndexerNotToThrowWhenNullIsUsedAgainstReferenceTypes()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> { typeof(string) });

			Assert.DoesNotThrow(() => { argsList[0] = null; });
		}

		[Test]
		public void ExpectGetIndexerToThrowIfIndexIsOutOfBounds()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			Assert.Throws<ArgumentOutOfRangeException>(() => { Debug.Log(argsList[5]); });
			Assert.Throws<ArgumentOutOfRangeException>(() => { Debug.Log(argsList[-5]); });
		}

		[Test]
		public void ExpectGetIndexerToReturnValuePassedIntoSetAtSameIndex()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			var value = "foobar";
			argsList[0] = value;
			var result = argsList[0];
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectGetToReturnDefaultValueIfSetWasNotCalled()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string), typeof(int)});

			var value = default(int);
			var result = argsList.Get<int>(2);
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectStringSerializationToBeSupported()
		{
			var argsList = new ArgsList();

			// test that it doesn't throw when specifying the type

			Assert.DoesNotThrow(() => argsList.SetTypes(new List<Type> {typeof(string)}));

			// Add some data, serialize and deserialize
			var value = "foobar";
			argsList.Set(0, value);
			var json = JsonUtility.ToJson(argsList);
			var resultArgsList = JsonUtility.FromJson<ArgsList>(json);
			var result = resultArgsList.Get<string>(0);

			// Now test the value is what it should be
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectIntSerializationToBeSupported()
		{
			var argsList = new ArgsList();

			// test that it doesn't throw when specifying the type

			Assert.DoesNotThrow(() => argsList.SetTypes(new List<Type> {typeof(int)}));

			// Add some data, serialize and deserialize
			var value = 42;
			argsList.Set(0, value);
			var json = JsonUtility.ToJson(argsList);
			var resultArgsList = JsonUtility.FromJson<ArgsList>(json);
			var result = resultArgsList.Get<int>(0);

			// Now test the value is what it should be
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectFloatSerializationToBeSupported()
		{
			var argsList = new ArgsList();

			// test that it doesn't throw when specifying the type

			Assert.DoesNotThrow(() => argsList.SetTypes(new List<Type> {typeof(float)}));

			// Add some data, serialize and deserialize
			var value = 9.75f;
			argsList.Set(0, value);
			var json = JsonUtility.ToJson(argsList);
			var resultArgsList = JsonUtility.FromJson<ArgsList>(json);
			var result = resultArgsList.Get<float>(0);

			// Now test the value is what it should be
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectBoolSerializationToBeSupported()
		{
			var argsList = new ArgsList();

			// test that it doesn't throw when specifying the type

			Assert.DoesNotThrow(() => argsList.SetTypes(new List<Type> {typeof(bool)}));

			// Add some data, serialize and deserialize
			var value = true;
			argsList.Set(0, value);
			var json = JsonUtility.ToJson(argsList);
			var resultArgsList = JsonUtility.FromJson<ArgsList>(json);
			var result = resultArgsList.Get<bool>(0);

			// Now test the value is what it should be
			Assert.AreEqual(value, result);
		}

		[Test]
		public void ExpectSerializationOfMultipleTypesToBeSupported()
		{
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type>
			{
				typeof(string),
				typeof(string),
				typeof(int),
				typeof(bool),
				typeof(float),
				typeof(int),
			});

			// Add some data, serialize and deserialize
			argsList.Set(0, "foo");
			argsList.Set(1, "bar");
			argsList.Set(2, 10);
			argsList.Set(3, true);
			argsList.Set(4, -4.2f);
			argsList.Set(5, 7);

			var json = JsonUtility.ToJson(argsList);
			var resultArgsList = JsonUtility.FromJson<ArgsList>(json);

			// Now test the value is what it should be
			Assert.AreEqual(argsList.Get<string>(0), resultArgsList.Get<string>(0));
			Assert.AreEqual(argsList.Get<string>(1), resultArgsList.Get<string>(1));
			Assert.AreEqual(argsList.Get<int>(2), resultArgsList.Get<int>(2));
			Assert.AreEqual(argsList.Get<bool>(3), resultArgsList.Get<bool>(3));
			Assert.AreEqual(argsList.Get<float>(4), resultArgsList.Get<float>(4));
			Assert.AreEqual(argsList.Get<int>(5), resultArgsList.Get<int>(5));
		}

		[Test(Description = "If SetTypes() is called to change the arg order/types/amount, old data should be invalidated (and replaced with default for the types)")]
		public void ExpectSetTypesToInvalidateIndices()
		{
			var value = 10;
			
			var argsList = new ArgsList();
			argsList.SetTypes(new List<Type> {typeof(int)});
			argsList.Set(0, value);
			argsList.SetTypes(new List<Type> {typeof(string)});
			
			Assert.AreNotEqual(value, argsList[0]);
			Assert.AreEqual(default(string), argsList[0]);
		}

		[Test(Description = "If SetTypes() is called to change the arg order/types/amount, for any indices where the type has not changed, the data should be maintained")]
		public void ExpectSetTypesToInvalidateIndicesButKeepDataIfTheTypesStillMatch()
		{
			var value = "foobar";
			
			var argsList = new ArgsList();
			
			// setup to take (int, string)
			argsList.SetTypes(new List<Type> {typeof(int), typeof(string)});
			argsList.Set(0, 42);
			argsList.Set(1, value);
			
			// now change to take (string, string)
			argsList.SetTypes(new List<Type> {typeof(string), typeof(string)});
			
			// the second string param should still be vlaid
			Assert.AreEqual(value, argsList[1]);
		}
	}
}