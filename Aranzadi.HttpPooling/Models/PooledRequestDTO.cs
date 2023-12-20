using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.HttpPooling.Models
{
    public class PooledRequestDTO
	{
		public HttpPoolingRequest Request { get; set; }
		public int NIntento { get; set; }
	}
}
