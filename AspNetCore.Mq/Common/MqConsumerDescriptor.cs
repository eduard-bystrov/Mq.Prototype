using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public class MqConsumerDescriptor
	{
		public String Queue { get; set; }

		public Boolean AutoAck { get; set; }

		public String ConsumerTag { get; set; }

		/// <summary>
		/// If the no-local field is set the server will not send messages to the connection that published them.
		/// </summary>
		public Boolean NoLocal { get; set; }

		public Boolean Exclusive { get; set; }

		public IDictionary<String, Object> Arguments { get; set; }
	}
}
