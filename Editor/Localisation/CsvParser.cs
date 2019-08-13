using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Original code: https://www.codeproject.com/Tips/823670/Csharp-Light-and-Fast-CSV-Parser
 */

namespace DUCK.Localisation.Editor
{
	public static class CsvParser
	{
		public static IEnumerable<IList<string>> Parse(string content, char delimiter = ',', char qualifier = '"')
		{
			var reader = new StringReader(content);
			var inQuote = false;
			var record = new List<string>();
			var sb = new StringBuilder();

			while (reader.Peek() != -1)
			{
				var readChar = (char) reader.Read();

				if (readChar == '\n' || (readChar == '\r' && (char) reader.Peek() == '\n'))
				{
					// If it's a \r\n combo consume the \n part and throw it away.
					if (readChar == '\r')
						reader.Read();

					if (inQuote)
					{
						if (readChar == '\r')
							sb.Append('\r');
						sb.Append('\n');
					}
					else
					{
						if (record.Count > 0 || sb.Length > 0)
						{
							record.Add(sb.ToString());
							sb.Clear();
						}

						if (record.Count > 0)
							yield return record;

						record = new List<string>(record.Count);
					}
				}
				else if (sb.Length == 0 && !inQuote)
				{
					if (readChar == qualifier)
						inQuote = true;
					else if (readChar == delimiter)
					{
						record.Add(sb.ToString());
						sb.Clear();
					}
					else if (char.IsWhiteSpace(readChar))
					{
						// Ignore leading whitespace
					}
					else
						sb.Append(readChar);
				}
				else if (readChar == delimiter)
				{
					if (inQuote)
						sb.Append(delimiter);
					else
					{
						record.Add(sb.ToString());
						sb.Clear();
					}
				}
				else if (readChar == qualifier)
				{
					if (inQuote)
					{
						if ((char) reader.Peek() == qualifier)
						{
							reader.Read();
							sb.Append(qualifier);
						}
						else
							inQuote = false;
					}
					else
						sb.Append(readChar);
				}
				else
					sb.Append(readChar);
			}

			if (record.Count > 0 || sb.Length > 0)
				record.Add(sb.ToString());

			if (record.Count > 0)
				yield return record;
		}
	}
}