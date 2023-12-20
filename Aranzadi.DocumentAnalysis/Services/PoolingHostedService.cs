using Aranzadi.HttpPooling;
using Aranzadi.HttpPooling.Interfaces;
using Serilog;
using System.Configuration;

namespace Aranzadi.DocumentAnalysis.Services
{
    public class PoolingHostedService : BackgroundService
	{
		private readonly PoolingConfiguration configuration;
		private readonly IHostApplicationLifetime appLifetime;
		private readonly IHttpPoolingServices httpPoolingServices;

		public PoolingHostedService(
			  PoolingConfiguration configuration
			, IHostApplicationLifetime appLifetime
			, IHttpPoolingServices httpPoolingServices)
        {
			this.configuration = configuration;
			this.appLifetime = appLifetime;
			this.httpPoolingServices = httpPoolingServices;
			
		}
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				Log.Information($"{nameof(PoolingHostedService)} running at: {DateTimeOffset.Now}");
				
				await httpPoolingServices.Start();
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
			}
			
		}
	}
}
