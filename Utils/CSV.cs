using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DUCK.Utils
{
	public static class CSV
	{
		public abstract class Type
		{
			internal abstract void ValidateType(CSVDocument.CSVRecord.CSVField field);
		}

		public abstract class CSVType<T> : Type
		{
			public Func<string, T> Parse { get; private set; }

			public CSVType(Func<string, T> parseFunction)
			{
				Parse = parseFunction;
			}

			internal override void ValidateType(CSVDocument.CSVRecord.CSVField field)
			{
				try
				{
					field.GetValue<T>();
				}
				catch
				{
					throw new ArgumentException(string.Format("Data '{0}' cannot be parsed to type {1}", field, typeof(T)));
				}
			}
		}

		public static class Types
		{
			public class String : CSVType<string>
			{
				public String() : base(s => s) { }

				public String(int maxLength) : base(s =>
				{
					if ((s != null ? s.Length : 0) > maxLength) throw new Exception("Max string length exceeded: " + maxLength);
					return s;
				}){}
			}

			public class Int : CSVType<int>
			{
				public Int() : base(int.Parse) { }

				public Int(int minValue = int.MinValue, int maxValue = int.MaxValue) : base(s => 
				{
					var value = int.Parse(s);
					if (value < minValue) throw new Exception("Min value not reached: " + maxValue);
					if (value > maxValue) throw new Exception("Max value exceeded: " + maxValue);
					return value;
				}){}
			}

			public class Bool : CSVType<bool>
			{
				public Bool() : base(bool.Parse) { }
			}

			public class Float : CSVType<float>
			{
				public Float() : base(float.Parse) { }

				public Float(float minValue = float.MinValue, float maxValue = float.MaxValue) : base(s => 
				{
					var value = float.Parse(s);
					if (value < minValue) throw new Exception("Min value not reached: " + maxValue);
					if (value > maxValue) throw new Exception("Max value exceeded: " + maxValue);
					return value;
				}){ }
			}

			public class Enum<T> : CSVType<T>
				where T : struct, IConvertible
			{
				public Enum() : base
					(
						str =>
						{
							if (!typeof(T).IsEnum)
							{
								throw new ArgumentException("Type must be Enum");
							}

							return (T)Enum.Parse(typeof(T), str);
						}
					){}
			}
		}

		public class CSVFormat
		{
			private Dictionary<string, Type> format;
			private Type defaultType = new Types.String();

			public CSVFormat(Dictionary<string, Type> format, Type defaultType = null)
			{
				this.format = format;

				if (defaultType != null)
				{
					this.defaultType = defaultType;
				}
			}

			internal string GetFieldName(int index)
			{
				return (format.Keys.ToArray()[index]);
			}

			internal Type GetType(string fieldName)
			{
				return format[fieldName];
			}

			internal void ValidateHeader(string[] header, bool allowUnexpectedFields = false)
			{
				if (format == null || format.Keys.Count == 0) return;

				List<string> expectedFieldNames = new List<string>(format.Keys);

				foreach (var textChunk in header)
				{
					var fieldName = textChunk.Trim();
					var didFind = expectedFieldNames.Remove(fieldName);

					if (!didFind)
					{
						// Field is known but wasn't remaining in the 'expected' - i.e. we've removed it before
						if (format.ContainsKey(fieldName))
						{
							throw new Exception("Duplicate field: " + fieldName);
						}
						// Field isn't known
						else
						{
							if (!allowUnexpectedFields)
							{
								throw new Exception("Unknown field: " + fieldName);
							}
							else
							{
								format.Add(fieldName.Trim(), defaultType);
							}
						}
					}
				}

				if (expectedFieldNames.Count > 0)
				{
					var stringBuilder = new StringBuilder(string.Format("Fields not present in header ({0}): ", expectedFieldNames.Count));

					var isFirst = true;
					foreach (var expectedField in expectedFieldNames)
					{
						if (!isFirst) stringBuilder.Append(", ");
						stringBuilder.Append(expectedField);
						isFirst = false;
					}

					throw new Exception(stringBuilder.ToString());
				}
			}
		}

		public class CSVDocument
		{
			public class CSVRecord
			{
				internal class CSVField
				{
					internal string data;
					private Type type;

					internal T GetValue<T>()
					{
						return (type as CSVType<T>).Parse(data);
					}

					internal CSVField(string data, Type type)
					{
						this.data = data;
						this.type = type;
					}

					public override string ToString()
					{
						return data;
					}
				}

				private Dictionary<string, CSVField> data;

				public T GetFieldValue<T>(string fieldName, bool useDefault = false)
				{
					try
					{
						return (data[fieldName].GetValue<T>());
					}
					catch (Exception e)
					{
						if (useDefault)
						{
							return default(T);
						}

						throw e;
					}
				}

				internal CSVRecord(string[] values, CSVFormat format)
				{
					data = new Dictionary<string, CSVField>();

					for (var i = 0; i < values.Length; i++)
					{
						try
						{
							var fieldName = format.GetFieldName(i);
							var fieldType = format.GetType(fieldName);
							var field = new CSVField(values[i].Trim(), fieldType);

							fieldType.ValidateType(field);

							data.Add(fieldName, field);
						}
						catch (Exception e)
						{
							var sb = new StringBuilder("");
							foreach (var s in values) sb.Append(s + ",");
							Debug.LogError("Failed to Validate field " + i + " of " + sb.ToString() + ": " + e.Message);
						}
					}
				}
			}

			public CSVRecord[] Records
			{
				get; private set;
			}

			public CSVDocument(string[] inputRecords, CSVFormat format)
			{
				var offset = (format != null ? 1 : 0);

				Records = new CSVRecord[inputRecords.Length - offset];

				for (var i = offset; i < inputRecords.Length; i++)
				{
					var inputRecord = inputRecords[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					Records[i - offset] = new CSVRecord(inputRecord, format);
				}
			}
		}

		public static CSVDocument Parse(string rawInputData, CSVFormat format, bool allowUnexpectedFields = false)
		{
			var inputRecords = rawInputData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (inputRecords.Length == 0) return null;

			if (format != null)
			{
				var header = inputRecords[0];
				if (format != null)
				{
					format.ValidateHeader(header.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), allowUnexpectedFields);
				}
			}

			return new CSVDocument(inputRecords, format);
		}
	}
}

