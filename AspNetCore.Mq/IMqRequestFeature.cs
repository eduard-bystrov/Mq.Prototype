using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IMqRequestFeature
	{
		String ConsumerTag { get; set; }
		UInt64 DeliveryTag { get; set; }
		Boolean Redelivered { get; set; }
		String Exchange { get; set; }
		String RoutingKey { get; set; }
		IBasicProperties Properties { get; set; }
		Byte[] Body { get; set; }
	}
}
