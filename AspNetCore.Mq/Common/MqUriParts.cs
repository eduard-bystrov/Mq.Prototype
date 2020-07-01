using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	[Flags]
	public enum MqUriParts
	{
		None = 0,
		VirtualHost = 0x1,
		Exchange = 0x2,
		RoutingKey = 0x4,

	}
}
