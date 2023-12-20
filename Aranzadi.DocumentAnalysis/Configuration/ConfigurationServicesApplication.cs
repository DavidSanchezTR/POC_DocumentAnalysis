using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.Filters;
using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Services.IServices;
using Aranzadi.HttpPooling;
using Aranzadi.HttpPooling.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Configuration
{
    public class ConfigurationServicesApplication
	{
		public static void ConfigureServices(WebApplicationBuilder builder, DocumentAnalysisOptions documentAnalysisOptions)
		{
			builder.Services.AddControllers(options =>
			{
				options.Filters.Add(typeof(ExceptionFilter));
			})
				.AddJsonOptions(jsonOptions =>
				{
					jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
				});

			string connString = documentAnalysisOptions.ConnectionStrings.DefaultConnection;

			builder.Services.AddDbContext<DocumentAnalysisDbContext>(dbOptions =>
			{
				dbOptions.UseCosmos(connString, documentAnalysisOptions.CosmosDatabaseName ?? "documentAnalisis", (cosmosOptions) =>
				{
					cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5));
				});
			});

			if (!builder.Environment.IsDevelopment()) { }

			builder.Services.AddTransient((e) =>
			{
				return new DocumentAnalysisEnvironment(builder.Environment.ContentRootPath, builder.Environment);
			});

			#region SERVICES
			builder.Services.AddSingleton(sp => { return documentAnalysisOptions; });
			//configure Pooling configuration
			PoolingConfiguration poolingConfiguration = documentAnalysisOptions.Pooling;
			poolingConfiguration.Messaging.Endpoint = documentAnalysisOptions.Messaging.Endpoint;
			builder.Services.AddSingleton(sp => { return poolingConfiguration; });
			builder.Services.AddSingleton<IHttpPoolingServices, AnacondaPoolingService>();

			builder.Services.AddHttpClient();
			builder.Services.AddTransient<IDocumentAnalysisService, DocumentAnalysisService>();
			builder.Services.AddTransient<IDocumentAnalysisRepository, DocumentAnalysisRepository>();
			builder.Services.AddTransient<IAnalysisProviderService, AnalysisProviderService>();
			builder.Services.AddTransient<ILogAnalysis, LogAnalysisService>();
			builder.Services.AddTransient<ICreditsConsumptionClient, CreditsConsumptionClient>();
			builder.Services.AddHealthChecks();

			#endregion
		}

	}
}
