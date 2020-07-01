using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IMqResponseFeature
	{
		String Exchange { get; set; }
		String RoutingKey { get; set; }
		Boolean Mandatory { get; set; }
		IBasicProperties Properties { get; set; }
		Byte[] Body { get; set; }
		Stream BodyStream { get; set; }

		UInt64? AckDeliveryTag { get; set; }
	}
}
