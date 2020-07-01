using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public class MqServerOptions
	{
		public static MqQueueDescriptor CreateDefaultQueueDescriptor()
		{
			return new MqQueueDescriptor
			{
				Durable = true,
				Exclusive = false,
				AutoDelete = false,
			};
		}

		public static MqConsumerDescriptor CreateDefaultConsumerDescriptor()
		{
			return new MqConsumerDescriptor
			{
				AutoAck = false,
				Exclusive = false,
				NoLocal = false,
				
			};
		}

		public MqServerOptions()
		{
			QueueDescriptor = CreateDefaultQueueDescriptor();
			ConsumerDescriptor = CreateDefaultConsumerDescriptor();
		}

		public MqQueueDescriptor QueueDescriptor { get; set; }

		public MqConsumerDescriptor ConsumerDescriptor { get; set; }

		public IConnectionFactory ConnectionFactory { get; set; }

		public MqUriParts PathBaseParts { get; set; }
			= MqUriParts.VirtualHost | MqUriParts.Exchange | MqUriParts.RoutingKey;



	}
}
