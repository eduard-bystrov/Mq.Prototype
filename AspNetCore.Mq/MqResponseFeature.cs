using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RabbitMQ.Client;

namespace AspNetCore.Mq
{
	public class MqResponseFeature : IMqResponseFeature
	{
		public String Exchange { get; set; }
		public String RoutingKey { get; set; }
		public Boolean Mandatory { get; set; }
		public IBasicProperties Properties { get; set; }
		public Byte[] Body { get; set; }
		public Stream BodyStream { get; set; }
		public UInt64? AckDeliveryTag { get; set; }
	}
}
