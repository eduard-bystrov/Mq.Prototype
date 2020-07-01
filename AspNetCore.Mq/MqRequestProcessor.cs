using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AspNetCore.Mq
{

	public abstract class MqRequestProcessor : IMqRequestProcessor
	{
		private readonly IMqServerRequestContextFactory _serverContextFactory;
		private readonly ILogger _logger;

		protected MqRequestProcessor(
			IMqServerRequestContextFactory serverContextFactory,
			ILogger<MqRequestProcessor> logger
			)
		{
			_serverContextFactory = serverContextFactory ?? throw new ArgumentNullException(nameof(serverContextFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}


		public abstract Task ProcessRequestAsync(IMqConnection connection, IMqRequestFeature requestFeature);

		public async Task ProcessRequestAsync<TContext>(IMqConnection connection, IHttpApplication<TContext> application, IMqRequestFeature requestFeature)
		{
			try
			{
				var serverContext = _serverContextFactory.CreateContext(connection);
				serverContext.InitializeRequest(requestFeature);
				var features = serverContext.Features;

				var context = application.CreateContext(features);
				Exception contextException = null;
				try
				{
					Exception requestException = null;
					try
					{
						await application.ProcessRequestAsync(context);
					}
					catch (Exception ex)
					{
						requestException = ex;
						_logger.LogError(ex, "Request processing error");
					}

					if (requestException != null)
					{
						serverContext.SetError(StatusCodes.Status500InternalServerError, requestException);
					}

					await serverContext.SendResponseAsync();

				}
				catch (Exception ex)
				{
					contextException = ex;
					throw;
				}
				finally
				{
					application.DisposeContext(context, contextException);
				}

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Processing error");
			}
		}


	}

	public class MqRequestProcessor<TContext> : MqRequestProcessor
	{
		public MqRequestProcessor(
			IMqServerRequestContextFactory serverContextFactory,
			ILogger<MqRequestProcessor> logger,
			IHttpApplication<TContext> application
			) 
			: base(serverContextFactory, logger)
		{
			Application = application ?? throw new ArgumentNullException(nameof(application));
		}


		public IHttpApplication<TContext> Application { get; }

		public override Task ProcessRequestAsync(IMqConnection connection, IMqRequestFeature requestFeature)
		{
			return ProcessRequestAsync(connection, Application, requestFeature);
		}
	}
}
