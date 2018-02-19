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
			internal abstract void ValidateField(Document.Record.Field field);
		}

		public class Type<T> : Type
		{
			public Func<string, T> Parse { get; private set; }

			public Type(Func<string, T> parseFunction, bool allowEmptyValues = false)
			{
				Parse = (s) =>
				{
					if (string.IsNullOrEmpty(s))
					{
						if (allowEmptyValues)
						{
							return default(T);
						}

						throw new ArgumentNullException();
					}

					return parseFunction(s);
				};
			}

			internal override void ValidateField(Document.Record.Field field)
			{
				try
				{
					field.GetValue<T>();
				}
				catch (ArgumentNullException)
				{
					throw new ArgumentException(string.Format("Empty/null values are not accepted by this {0}", this.GetType()));
				}
				catch (FormatException)
				{
					throw new ArgumentException(string.Format("Input '{0}' cannot be parsed to type {1}", field, typeof(T)));
				}
				catch (ArgumentException)
				{
					throw new ArgumentException(string.Format("Input '{0}' is invalid for the constraints on this {1}", field, this.GetType()));
				}
			}
		}

		public static class Types
		{
			public class String : Type<string>
			{
				public String(bool allowEmptyValues = false) : base(s => s, allowEmptyValues) { }
			}

			public class Int : Type<int>
			{
				public Int() : base(int.Parse) { }

				public Int(bool allowEmptyValues = false, int minValue = int.MinValue, int maxValue = int.MaxValue) : base(s => 
				{
					var value = int.Parse(s);
					if (value < minValue) throw new ArgumentException("Min value not reached: " + maxValue);
					if (value > maxValue) throw new ArgumentException("Max value exceeded: " + maxValue);
					return value;
				}, allowEmptyValues){}
			}

			public class Bool : Type<bool>
			{
				public Bool(bool allowEmptyValues = false) : base(bool.Parse, allowEmptyValues) { }
			}

			public class Float : Type<float>
			{
				public Float() : base(float.Parse) { }

				public Float(bool allowEmptyValues = false, float minValue = float.MinValue, float maxValue = float.MaxValue) : base(s =>
				{
					var value = float.Parse(s);
					if (value < minValue) throw new ArgumentException("Min value not reached: " + maxValue);
					if (value > maxValue) throw new ArgumentException("Max value exceeded: " + maxValue);
					return value;
				}, allowEmptyValues){ }
			}

			public class Enum<T> : Type<T> where T : struct, IConvertible
			{
				public Enum(bool ignoreCase = false, bool allowEmptyValues = false, T fallbackValue = default(T)) : base(s =>
				{
					if (!typeof(T).IsEnum) throw new ArgumentException("Type must be an Enum type.");

					if (string.IsNullOrEmpty(s))
					{
						if (allowEmptyValues) return fallbackValue;

						throw new ArgumentNullException();
					}

					return (T)Enum.Parse(typeof(T), s, ignoreCase);
				}, allowEmptyValues){ }
			}
		}

		public class Format
		{
			private Dictionary<string, Type> format;
			private Type defaultType = new Types.String();

			public Format(Dictionary<string, Type> format, Type defaultType = null)
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

		public class Document
		{
			public class Record
			{
				internal class Field
				{
					internal string data;
					private Type type;

					internal T GetValue<T>()
					{
						return (type as Type<T>).Parse(data);
					}

					internal Field(string data, Type type)
					{
						this.data = data;
						this.type = type;
					}

					public override string ToString()
					{
						return data;
					}
				}

				private Dictionary<string, Field> data;

				public T GetFieldValue<T>(string fieldName, bool useFallback = false)
				{
					try
					{
						return (data[fieldName].GetValue<T>());
					}
					catch (Exception e)
					{
						if (useFallback)
						{
							return default(T);
						}

						throw e;
					}
				}

				internal Record(string[] values, Format format)
				{
					data = new Dictionary<string, Field>();

					for (var i = 0; i < values.Length; i++)
					{
						var fieldName = format.GetFieldName(i);

						try
						{
							var fieldType = format.GetType(fieldName);
							var field = new Field(values[i].Trim(), fieldType);

							fieldType.ValidateField(field);

							data.Add(fieldName, field);
						}
						catch (Exception e)
						{
							var sb = new StringBuilder("");
							foreach (var s in values) sb.Append(s + ",");
							Debug.LogError(string.Format("Failed to validate field {0} ({1}) of {2} : {3}",
								i,
								fieldName,
								sb.ToString(),
								e.Message));
						}
					}
				}
			}

			public Record[] Records
			{
				get; private set;
			}

			public Document(string[] inputRecords, Format format)
			{
				var offset = (format != null ? 1 : 0);

				Records = new Record[inputRecords.Length - offset];

				for (var i = offset; i < inputRecords.Length; i++)
				{
					var inputRecord = inputRecords[i].Split(new[] { ',' });
					Records[i - offset] = new Record(inputRecord, format);
				}
			}
		}

		public static Document Parse(string rawInputData, Format format, bool allowUnexpectedFields = false)
		{
			var inputRecords = rawInputData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (inputRecords.Length == 0) return null;

			if (format != null)
			{
				var header = inputRecords[0];
				if (format != null)
				{
					format.ValidateHeader(header.Split(new[] { ',' }), allowUnexpectedFields);
				}
			}

			return new Document(inputRecords, format);
		}
	}
}

