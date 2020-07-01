using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IHeaderConverter
	{
		String ReadString(Object rawValue);
		Object WriteString(String value);

		StringValues Read(Object rawValue);
		Object Write(StringValues value);
	}
}
