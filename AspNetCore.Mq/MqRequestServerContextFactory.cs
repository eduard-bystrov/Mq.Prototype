using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public class MqRequestServerContextFactory : IMqServerRequestContextFactory
	{
		public MqRequestServerContextFactory(IFeatureCollection serverFeatures)
		{
			ServerFeatures = serverFeatures ?? throw new ArgumentNullException(nameof(serverFeatures));
		}

		public IFeatureCollection ServerFeatures { get; }

		public IMqServerRequestContext CreateContext(IMqConnection connection)
		{
			var context = new MqServerRequestContext(connection, ServerFeatures);
			return context;
		}
	}
}
