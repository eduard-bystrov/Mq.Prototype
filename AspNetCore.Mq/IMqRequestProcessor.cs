using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Mq
{
	public interface IMqRequestProcessor
	{
		Task ProcessRequestAsync(IMqConnection connection, IMqRequestFeature requestFeature);
	}
}
