using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Mq
{
	public interface IMqServerRequestContext
	{
		void InitializeRequest(IMqRequestFeature requestFeature);

		IFeatureCollection Features { get; }

		void SetError(Int32 statusCode, Exception exception);

		Task SendResponseAsync();
	}
}
