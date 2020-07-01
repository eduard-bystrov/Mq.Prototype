using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqWebApp
{
	public class MqServer1 : IServer
	{
		private readonly FeatureCollection _features = new FeatureCollection();
		public IFeatureCollection Features => _features;

		public MqServer1()
		{
		}

		private class ServerApp<TContext>
		{
			private IConnection _connection;
			private IModel _model;
			private static readonly String _queueName = "TestQueue";
			private readonly IHttpApplication<TContext> _application;
			private readonly IFeatureCollection _features;

			public ServerApp(IHttpApplication<TContext> application, IFeatureCollection features)
			{
				_application = application;
				_features = features;
				var connectionFactory = new ConnectionFactory() { DispatchConsumersAsync = true };
				_connection = connectionFactory.CreateConnection();
				_model = _connection.CreateModel();
				var queueDeclareResult = _model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
				_model.BasicConsume(_queueName, autoAck: false, consumer: new Consumer(_model, this));
			}

			private class Consumer : AsyncDefaultBasicConsumer
			{
				private readonly ServerApp<TContext> _server;

				public Consumer(IModel model, ServerApp<TContext> server) : base(model)
				{
					_server = server;
				}

				public override async Task HandleBasicDeliver(String consumerTag, UInt64 deliveryTag, Boolean redelivered, String exchange, String routingKey, IBasicProperties properties, Byte[] body)
				{
					var app = _server._application;

					var path = String.Empty;
					if (properties.Headers.TryGetValue("path", out var pathObj))
					{
						path = Encoding.UTF8.GetString((Byte[])pathObj);
						if (!path.StartsWith("/")) path = "/" + path;
					}


					var requestFeature = new HttpRequestFeature
					{
						Body = new MemoryStream(body, writable: false),
						Method = "GET",
						Protocol = "http",
						Path = path,
						Scheme = "http",
					};

					var responseBodyStream = new MemoryStream();
					var responseFeature = new HttpResponseFeature
					{
						Body = responseBodyStream,
					};

					var features = new FeatureCollection(_server._features);
					features.Set<IHttpRequestFeature>(requestFeature);
					features.Set<IHttpResponseFeature>(responseFeature);

					var context = app.CreateContext(features);
					await app.ProcessRequestAsync(context);

					if (properties.IsReplyToPresent() && properties.ReplyToAddress != null)
					{
						var responseBody = responseBodyStream.ToArray();
						var props = Model.CreateBasicProperties();
						if (props.Headers == null)
						{
							props.Headers = new Dictionary<String, Object>();
						}
						foreach (var header in responseFeature.Headers)
						{
							props.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
						}
						Model.BasicPublish(properties.ReplyToAddress, props, responseBody);
					}

					Model.BasicAck(deliveryTag, multiple: false);

					app.DisposeContext(context, null);

				}
			}
		}


		//private class ResponseFeature : HttpResponseFeature
		//{
		//	override 

		//}


		public void Dispose()
		{
		}

		private Object _serverApp;
		public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
		{
			_serverApp = new ServerApp<TContext>(application, _features);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
