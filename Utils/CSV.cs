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
			private List<string> orderedKeys;
			private IDictionary<string, Type> format;

			public string Header
			{
				get
				{
					var sb = new StringBuilder();
					Output(sb);
					return sb.ToString();
				}
			}

			public Format(IDictionary<string, Type> format)
			{
				this.format = format;
			}

			public string[] GetFieldNames()
			{
				var fieldCount = format.Keys.Count;
				var fieldNames = new string[fieldCount];
				for (var i = 0; i < fieldCount; i++)
				{
					fieldNames[i] = GetFieldName(i);
				}
				return fieldNames;
			}

			internal string GetFieldName(int index)
			{
				if (orderedKeys != null)
				{
					return (orderedKeys[index]);
				}
				else
				{
					return (format.Keys.ToArray())[index];
				}
			}

			internal Type GetType(string fieldName)
			{
				return format[fieldName];
			}

			internal void ValidateHeader(string[] headerFields, Type defaultType = null)
			{
				if (format == null || format.Keys.Count == 0) return;

				var expectedFieldNames = new List<string>(format.Keys);

				orderedKeys = new List<string>();

				for (var i = 0; i < headerFields.Length; i++)
				{
					var fieldName = headerFields[i].Trim();
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
							if (defaultType == null)
							{
								throw new Exception("Unknown field, no default type: " + fieldName);
							}
							else
							{
								format.Add(fieldName.Trim(), defaultType);
							}
						}
					}

					orderedKeys.Add(fieldName);
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

			internal void Output(StringBuilder sb)
			{
				var fieldNames = GetFieldNames();
				for (var i = 0; i < fieldNames.Length; i++)
				{
					sb.Append(fieldNames[i]);
					if (i < fieldNames.Length - 1)
					{
						sb.Append(",");
					}
				}

				sb.AppendLine();
			}
		}

		public class Document
		{
			public static readonly Type BasicDefaultType = new Types.String(allowEmptyValues: true);

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

				public T GetFieldValue<T>(object fieldName, bool useFallback = false)
				{
					try
					{
						return (data[fieldName.ToString()].GetValue<T>());
					}
					catch (Exception e)
					{
						if (useFallback)
						{
							return default(T);
						}
						else throw e;
					}
				}

				internal Record(string[] values, Format format, Type defaultType = null)
				{
					data = new Dictionary<string, Field>();

					for (var i = 0; i < values.Length; i++)
					{
						var fieldName = (format != null)
							? format.GetFieldName(i)
							: i.ToString();

						try
						{
							var fieldType = (format != null)
								? format.GetType(fieldName)
								: defaultType;

							if (fieldType == null) throw new Exception(string.Format("Field '{0}' has no format or default type provided."));

							var field = new Field(values[i].Trim(), fieldType);

							fieldType.ValidateField(field);

							data.Add(fieldName, field);
						}
						catch (Exception e)
						{
							var sb = new StringBuilder("");
							foreach (var s in values) sb.Append(s + ",");
							throw new Exception(string.Format("Failed to validate field {0} ({1}) of {2} : {3}",
								i,
								fieldName,
								sb.ToString(),
								e.Message));
						}
					}
				}

				internal void Output(StringBuilder sb)
				{
					var fields = data.Values.ToArray();

					for (var i = 0; i < fields.Length; i++)
					{
						sb.Append(fields[i].ToString());
						if (i < fields.Length - 1)
						{
							sb.Append(",");
						}
					}

					sb.AppendLine();
				}
			}

			public IList<Record> Records
			{
				get
				{
					return records;
				}
			}

			protected List<Record> records { get; private set; }
			protected Format format { get; private set; }

			public Document(string[] inputRecords, Format format, Type defaultType = null)
			{
				this.format = format;

				records = new List<Record>();

				if (format != null && inputRecords.Length > 0)
				{
					var header = inputRecords[0];
					format.ValidateHeader(header.Split(new[] { ',' }), defaultType);
				}

				var offset = (format != null ? 1 : 0);

				if (inputRecords.Length <= offset) return;

				for (var i = offset; i < inputRecords.Length; i++)
				{
					var inputRecord = inputRecords[i].Split(new[] { ',' });
					Records.Add(new Record(inputRecord, format, defaultType));
				}
			}

			public string Output()
			{
				StringBuilder sb = new StringBuilder();

				if (format != null)
				{
					format.Output(sb);
				}

				foreach (var record in Records)
				{
					record.Output(sb);
				}

				return sb.ToString();
			}
		}

		public class WriteableDocument : Document
		{
			private Type defaultType;

			public WriteableDocument(CSV.Format format, Type defaultType = null) : base(new[] { format.Header }, format, defaultType)
			{
				this.defaultType = defaultType;
			}

			public void AddRecord(string[] values)
			{
				records.Add(new Record(values, format, defaultType));
			}
		}

		public static Document Parse(string rawInputData, Format format, Type defaultType = null, bool allowUnexpectedFields = false)
		{
			var inputRecords = rawInputData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (inputRecords.Length == 0) return null;

			return new Document(inputRecords, format);
		}
	}
}

