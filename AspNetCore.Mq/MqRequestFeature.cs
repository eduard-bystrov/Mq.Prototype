using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Mq
{
	public class MqRequestFeature : IMqRequestFeature
	{
		public String ConsumerTag { get; set; }
		public UInt64 DeliveryTag { get; set; }
		public Boolean Redelivered { get; set; }
		public String Exchange { get; set; }
		public String RoutingKey { get; set; }
		public IBasicProperties Properties { get; set; }
		public Byte[] Body { get; set; }
	}
}
