using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public class MqQueueDescriptor
	{
		public String Name { get; set; }

		public Boolean Durable { get; set; }

		public Boolean Exclusive { get; set; }

		public Boolean AutoDelete { get; set; }

		public IDictionary<String, Object> Arguments { get; set; }
	}
}
