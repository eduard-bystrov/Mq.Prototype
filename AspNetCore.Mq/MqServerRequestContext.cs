using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace AspNetCore.Mq
{
	public partial class MqServerRequestContext : IMqServerRequestContext
	{
		private readonly IMqConnection _mqConnection;
		private readonly IFeatureCollection _serverFeatures;

		public MqServerRequestContext(IMqConnection mqConnection, IFeatureCollection serverFeatures)
		{
			_mqConnection = mqConnection ?? throw new ArgumentNullException(nameof(mqConnection));
			_serverFeatures = serverFeatures ?? throw new ArgumentNullException(nameof(serverFeatures));

			_pathBaseBase = "/";
			if (_mqConnection.VirtualHost != null)
			{
				_pathBaseBase += Uri.EscapeDataString(_mqConnection.VirtualHost);
			}
			else
			{
				_pathBaseBase += Uri.EscapeDataString("/");
			}
		}

		private readonly String _pathBaseBase;

		public IFeatureCollection Features { get; private set; }

		public void InitializeRequest(IMqRequestFeature mqRequest)
		{
			var headerConverter = _serverFeatures.Get<IHeaderConverter>();

			var mqResponse = CreateMqResponse(mqRequest);
			var httpRequest = CreateHttpRequest(mqRequest, headerConverter);
			var httpResponse = CreateHttpResponse(mqResponse);

			Features = new FeatureCollection(_serverFeatures);
			Features.Set<IMqConnection>(_mqConnection);
			Features.Set<IMqRequestFeature>(mqRequest);
			Features.Set<IMqResponseFeature>(mqResponse);
			Features.Set<IHttpRequestFeature>(httpRequest);
			Features.Set<IHttpResponseFeature>(httpResponse);
		}

		private IHttpRequestFeature CreateHttpRequest(IMqRequestFeature mqRequest, IHeaderConverter headerConverter)
		{
			var props = mqRequest.Properties;
			var headers = props?.Headers;

			var httpRequest = new HttpRequestFeature
			{
				Scheme = "amqp",
				Protocol = _mqConnection.Connection.Protocol.ApiName,
			};

			httpRequest.PathBase = _pathBaseBase
				+ "/" + (mqRequest.Exchange ?? String.Empty) 
				+ "/" + (mqRequest.RoutingKey ?? String.Empty);

			if (mqRequest.Body != null)
			{
				httpRequest.Body = new MemoryStream(mqRequest.Body, writable: false);
				httpRequest.Headers[HeaderNames.ContentLength] = mqRequest.Body.LongLength.ToString();
			}

			if (props != null)
			{
				// TODO: fill headers from props.
			}

			if (headers != null)
			{
				if (headers.TryGetValue(HeaderNames.Method, out var rawMethod))
				{
					httpRequest.Method = headerConverter.ReadString(rawMethod);
				}
				if (headers.TryGetValue(HeaderNames.Path, out var rawPath))
				{
					httpRequest.Path = headerConverter.ReadString(rawPath);
				}
				if (headers.TryGetValue(HeaderNames.QueryString, out var rawQueryString))
				{
					httpRequest.QueryString = headerConverter.ReadString(rawQueryString);
				}

				foreach (var item in headers)
				{
					httpRequest.Headers[item.Key] = headerConverter.Read(item.Value);
				}
			}

			return httpRequest;
		}

		private IMqResponseFeature CreateMqResponse(IMqRequestFeature mqRequest)
		{
			var mqResponse = new MqResponseFeature
			{
				BodyStream = new MemoryStream(),
				Mandatory = false,
				Properties = _mqConnection.Model.CreateBasicProperties(),
				AckDeliveryTag = mqRequest.DeliveryTag,
			};

			mqResponse.Properties.Headers = new Dictionary<String, Object>();

			var requestProps = mqRequest.Properties;
			if (requestProps.IsReplyToPresent() && requestProps.ReplyToAddress != null)
			{
				var addr = requestProps.ReplyToAddress;
				mqResponse.Exchange = addr.ExchangeName;
				mqResponse.RoutingKey = addr.RoutingKey;
			}

			return mqResponse;
		}

		private IHttpResponseFeature CreateHttpResponse(IMqResponseFeature mqResponse)
		{
			var httpResponse = new HttpResponseFeature
			{
				Body = mqResponse.BodyStream,
			};

			return httpResponse;
		}


		public void SetError(Int32 statusCode, Exception exception)
		{
			var responseFeature = Features.Get<IHttpResponseFeature>();
			SetError(responseFeature, statusCode, exception);
		}

		private void SetError(IHttpResponseFeature responseFeature, Int32 statusCode, Exception exception)
		{
			if (responseFeature == null || responseFeature.HasStarted)
			{
				return;
			}
			responseFeature.StatusCode = statusCode;
		}



		public Task SendResponseAsync()
		{
			return SendResponseAsync(
				Features.Get<IMqResponseFeature>(),
				Features.Get<IHttpResponseFeature>(),
				Features.Get<IHeaderConverter>()
				);
		}

		#region send response

		private static Byte[] GetBody(IMqResponseFeature mqResponse)
		{
			if (mqResponse.Body != null) return mqResponse.Body;
			if (mqResponse.BodyStream != null && mqResponse.BodyStream is MemoryStream memoryStream)
			{
				return memoryStream.ToArray();
			}

			return null;
		}

		private static void MergeResponseHeaders(IMqResponseFeature mqResponse, IHttpResponseFeature httpResponse, IHeaderConverter headerConverter)
		{
			var headers = mqResponse.Properties.Headers;
			foreach (var item in httpResponse.Headers)
			{
				headers[item.Key] = headerConverter.Write(item.Value);
			}

		}

		private Task SendResponseAsync(
			IMqResponseFeature mqResponse,
			IHttpResponseFeature httpResponse,
			IHeaderConverter headerConverter
			)
		{
			var needPublish = false;
			var needAck = mqResponse.AckDeliveryTag.HasValue;

			if (mqResponse.Exchange != null)
			{
				MergeResponseHeaders(mqResponse, httpResponse, headerConverter);
				needPublish = true;
			}

			return Task.Run(() =>
			{
				if (needPublish)
				{
					_mqConnection.Model.BasicPublish(mqResponse.Exchange, mqResponse.RoutingKey, mqResponse.Mandatory, mqResponse.Properties, GetBody(mqResponse));
				}
				if (needAck)
				{
					_mqConnection.Model.BasicAck(mqResponse.AckDeliveryTag.Value, multiple: false);
				}
			});
		}

		#endregion
	}
}
