using Aranzadi.DocumentAnalysis.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleClientApp
{
	public class LogAnalysis : ILogAnalysis
	{
		public void Debug(string message)
		{
		}

		public void Debug(Exception exception, string message)
		{
		}

		public void Error(string message)
		{
		}

		public void Error(Exception exception, string message)
		{
		}

		public void Fatal(string message)
		{
		}

		public void Fatal(Exception exception, string message)
		{
		}

		public void Information(string message)
		{
		}

		public void Information(Exception exception, string message)
		{
		}

		public void Warning(string message)
		{
		}

		public void Warning(Exception exception, string message)
		{
		}
	}
}