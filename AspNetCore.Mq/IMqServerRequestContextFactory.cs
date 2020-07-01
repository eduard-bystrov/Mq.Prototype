using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public interface IMqServerRequestContextFactory
	{
		IMqServerRequestContext CreateContext(IMqConnection connection);
	}
}
