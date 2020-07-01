using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Mq
{
	public class MqConsumer : AsyncDefaultBasicConsumer
	{
		private readonly IMqConnection _connection;
		private readonly IMqRequestProcessor _requestProcessor;

		public MqConsumer(IMqConnection connection, IMqRequestProcessor requestProcessor) : base(connection?.Model)
		{
			_connection = connection;
			_requestProcessor = requestProcessor;
		}

		public override Task HandleBasicDeliver(String consumerTag, UInt64 deliveryTag, Boolean redelivered, String exchange, String routingKey, IBasicProperties properties, Byte[] body)
		{
			var feature = new MqRequestFeature
			{
				ConsumerTag = consumerTag,
				DeliveryTag = deliveryTag,
				Redelivered = redelivered,
				Exchange = exchange,
				RoutingKey = routingKey,
				Properties = properties,
				Body = body,				
			};
			return _requestProcessor.ProcessRequestAsync(_connection, feature);
		}
	}
}
