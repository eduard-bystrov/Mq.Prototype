using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.Mq
{
	public class DefaultHeaderConverter : IHeaderConverter
	{
		public static Encoding DefaultEncoding => Encoding.UTF8;
		public static String DefaultSeparator => ",";

		public DefaultHeaderConverter()
			: this(null, null)
		{ }

		public DefaultHeaderConverter(Encoding encoding, String separator)
		{
			Encoding = encoding ?? DefaultEncoding;
			Separator = separator ?? DefaultSeparator;
			_seps = new[] { Separator };
		}

		public Encoding Encoding { get; }
		public String Separator { get; }

		private String[] _seps;

		public String ReadString(Object rawValue)
		{
			return Encoding.GetString((Byte[])rawValue);
		}

		public Object WriteString(String value)
		{
			return Encoding.GetBytes(value);
		}

		public StringValues Read(Object rawValue)
		{
			var str = ReadString(rawValue);
			if (str.IndexOf(Separator) == -1)
			{
				return str.Trim();
			}
			else
			{
				var splitted = str.Split(_seps, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < splitted.Length; i++)
				{
					splitted[i] = splitted[i].Trim();
				}
				return new StringValues(splitted);
			}
		}

		public Object Write(StringValues value)
		{
			if (value.Count == 0)
			{
				return null;
			}
			else if (value.Count == 1)
			{
				return Encoding.GetBytes(value[0]);
			}
			else
			{
				return WriteString(String.Join(Separator, value.AsEnumerable()));
			}
		}

	}
}
