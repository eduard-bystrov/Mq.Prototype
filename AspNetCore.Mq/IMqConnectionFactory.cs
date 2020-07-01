using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IMqConnectionFactory
	{
		IMqConnection CreateConnection(IMqRequestProcessor requestProcessor);
	}
}
