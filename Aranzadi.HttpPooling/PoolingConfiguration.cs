using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.HttpPooling
{
	public class PoolingConfiguration
	{
        public PoolingConfiguration()
        {
			Messaging = new ServiceBusClass()
			{
				Endpoint = "",
				Queue = ""
			};
		}
        public ServiceBusClass? Messaging { get; set; }
		public TimeSpan AvailableTime { get; set; }
		public int RetryTimeInSeconds { get; set; }

		public class ServiceBusClass
		{
			public string? Queue { get; set; }
			public string? Endpoint { get; set; }
		}
		
	}
}
