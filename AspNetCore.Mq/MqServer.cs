using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Mq
{
	public class MqServer : IServer
	{
		private readonly FeatureCollection _features = new FeatureCollection();
		private readonly ILoggerFactory _loggerFactory;

		public IFeatureCollection Features => _features;

		public MqServer(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			Features.Set<IHeaderConverter>(new DefaultHeaderConverter());
		}

		public void Dispose()
		{
		}

		private String _queueName = "TestQueue";

		public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				var connectionFactory = new ConnectionFactory()
				{
					DispatchConsumersAsync = true,
					AutomaticRecoveryEnabled = false,
					TopologyRecoveryEnabled = false,
					//VirtualHost = "testvhost",
				};
				var connection = connectionFactory.CreateConnection();
				var model = connection.CreateModel();
				var queueDeclareResult = model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

				var mqConnection = new MqConnection(connection, model, connectionFactory.VirtualHost);
				var serverContextFactory = new MqRequestServerContextFactory(Features);
				var requestProcessor = new MqRequestProcessor<TContext>(serverContextFactory, _loggerFactory.CreateLogger<MqRequestProcessor>(), application);
				var consumer = new MqConsumer(mqConnection, requestProcessor);

				model.BasicConsume(_queueName, autoAck: false, consumer: consumer);
			});
		}

		private class MqConnection : IMqConnection
		{
			public MqConnection(IConnection connection, IModel model, String virtualHost)
			{
				Connection = connection;
				Model = model;
				VirtualHost = virtualHost;
			}

			public IConnection Connection { get; }

			public IModel Model { get; }
			public String VirtualHost { get; }
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
