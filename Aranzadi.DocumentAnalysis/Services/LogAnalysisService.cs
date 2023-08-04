using Aranzadi.DocumentAnalysis.Messaging.Model;
using Serilog;
using System;

namespace Aranzadi.DocumentAnalysis.Services
{
    public class LogAnalysisService : ILogAnalysis
    {
		public void Debug(string message)
		{
			Log.Debug(message);
		}

		public void Debug(Exception exception, string message)
		{
			Log.Debug(exception, message);
		}

		public void Information(string message)
		{
			Log.Information(message);
		}

		public void Information(Exception exception, string message)
		{
			Log.Information(exception, message);
		}
		
		public void Warning(string message)
		{
			Log.Warning(message);
		}

		public void Warning(Exception exception, string message)
		{
			Log.Warning(exception, message);
		}

		public void Error(string message)
        {
            Log.Error(message);
        }

        public void Error(Exception exception, string message)
        {
            Log.Error(exception, message);
        }

		public void Fatal(string message)
		{
			Log.Fatal(message);
		}

		public void Fatal(Exception exception, string message)
		{
			Log.Fatal(exception, message);
		}

		

       
    }
}
