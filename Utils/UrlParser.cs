using System;
using System.Collections.Generic;
using System.Linq;

namespace DUCK.Utils
{
	/// <summary>
	/// A URL parser to retrieve parameters
	/// </summary>
	public static class UrlParser
	{
		public const char START = '?';
		static public readonly char[] TOKEN_SPLIT = { '&' };
		static public readonly char[] KEY_VALUE_SPLIT = { '=' };

		private const int KEY_INDEX = 0;
		private const int VALUE_INDEX = 1;
		private const int KEY_VALUE_PAIR_SIZE = 2;

		/// <summary>
		/// Parses the given URL to retrieve parameters
		/// </summary>
		/// <param name="url">URL to be parsed</param>
		/// <returns>Dictionary of string URL parameters</returns>
		public static Dictionary<string, string> ParseUrlString(string url)
		{
			if (!url.Contains(START))
			{
				return new Dictionary<string, string>();
			}

			return url.Substring(url.IndexOf(START) + 1)
				.Split(TOKEN_SPLIT, StringSplitOptions.RemoveEmptyEntries)
				.Select(token => token.Split(KEY_VALUE_SPLIT, StringSplitOptions.RemoveEmptyEntries))
				.Where(parts => parts.Length == KEY_VALUE_PAIR_SIZE)
				.ToDictionary(parts => parts[KEY_INDEX], parts => parts[VALUE_INDEX]);
		}
	}
}
