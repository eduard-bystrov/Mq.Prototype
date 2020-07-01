using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IMqConnection
	{
		IConnection Connection { get; }

		IModel Model { get; }

		String VirtualHost { get; }
	}
}
