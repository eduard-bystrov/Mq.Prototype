using AspNetCore.Mq;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Hosting
{
	public static class MqServerHostBuilderExtensions
	{
		public static IWebHostBuilder UseMqServer(this IWebHostBuilder hostBuilder)
		{
			return hostBuilder.ConfigureServices(services =>
			{
				services.AddSingleton<IServer, MqServer>();
			});
		}

	}
}
