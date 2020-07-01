using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RabbitMQ.Client;

namespace AspNetCore.Mq
{
	//public partial class MqServerRequestContext : IMqResponseFeature
	//{
	//	public String ResponseExchange { get; set; }
	//	public String ResponseRoutingKey { get; set; }
	//	public Boolean ResponseMandatory { get; set; }
	//	public IBasicProperties ResponseProperties { get; set; }
	//	public Byte[] ResponseBody { get; set; }
	//	public Stream ResponseBodyStream { get; set; }

	//	String IMqResponseFeature.Exchange
	//	{
	//		get => ResponseExchange;
	//		set => ResponseExchange;
	//	}
	//	String IMqResponseFeature.RoutingKey
	//	{
	//		get => throw new NotImplementedException();
	//		set => throw new NotImplementedException();
	//	}
	//	Boolean IMqResponseFeature.Mandatory
	//	{
	//		get => throw new NotImplementedException();
	//		set => throw new NotImplementedException();
	//	}
	//	IBasicProperties IMqResponseFeature.Properties
	//	{
	//		get => throw new NotImplementedException();
	//		set => throw new NotImplementedException();
	//	}
	//	Byte[] IMqResponseFeature.Body
	//	{
	//		get => throw new NotImplementedException();
	//		set => throw new NotImplementedException();
	//	}
	//	Stream IMqResponseFeature.BodyStream
	//	{
	//		get => throw new NotImplementedException();
	//		set => throw new NotImplementedException();
	//	}
	//}
}
