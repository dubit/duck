using System;
using System.Collections.Generic;
using System.Linq;

namespace DUCK.Utils
{
	/// <summary>
	/// Helper functions designed to improve collections usage.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Returns a random element from collection
		/// Picks between 0 and size of collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static T Random<T>(this IEnumerable<T> collection)
		{
			var enumerable = collection as T[] ?? collection.ToArray();
			if (enumerable.Length == 0)
			{
				throw new IndexOutOfRangeException(collection + ": Cannot get a random element from an empty collection!");
			}
			return enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Length));
		}

		/// <summary>
		/// Shuffle calls ShuffleCollection and take a seed for the random number
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static T[] Shuffle<T>(this IEnumerable<T> collection, int seed)
		{
			var random = new Random(seed);
			var shuffledArray = ShuffleCollection(collection, random);
			return shuffledArray;
		}

		/// <summary>
		/// Shuffle calls ShuffleCollection and creates an instance of Random using it's default seed
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static T[] Shuffle<T>(this IEnumerable<T> collection)
		{
			var random = new Random();
			var shuffledArray = ShuffleCollection(collection, random);
			return shuffledArray;
		}

		/// <summary>
		/// Call from Shuffle and shuffles given collection using Fisher-Yates algorithm
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		private static T[] ShuffleCollection<T>(this IEnumerable<T> collection, Random random)
		{
			var newArray = collection.ToArray();

			for (var i = newArray.Length; i > 1; i--)
			{
				var j = random.Next(i);
				var tmp = newArray[j];
				newArray[j] = newArray[i - 1];
				newArray[i - 1] = tmp;
			}

			return newArray;
		}

		/// <summary>
		/// Loop over collection and call action on each object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumeration"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			foreach (var item in enumeration)
			{
				action.Invoke(item);
			}
		}

		/// <summary>
		/// Returns the list as an array, from a specified startIndex and count
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T[] ToArray<T>(this List<T> collection, int startIndex = 0, int count = int.MaxValue)
		{
			if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			if (count < 0) throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			if (startIndex + count > collection.Count) throw new ArgumentOutOfRangeException("count", "startIndex + count > this.Count");

			var array = new T[count];
			for (var i = 0; i < count; i++)
			{
				array[i] = collection[i + startIndex];
			}

			return array;
		}
	}
}
