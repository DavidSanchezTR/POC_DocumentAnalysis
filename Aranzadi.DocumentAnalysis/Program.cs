using Aranzadi.DocumentAnalysis;
using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Util;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

try
{
	var builder = WebApplication.CreateBuilder(args);

	DocumentAnalysisOptions documentAnalysisOptions = ApplicationSettings.InitConfiguration(builder, "appsettings.json");

	//Use Serilog
	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.WriteTo.AzureAnalytics(documentAnalysisOptions.LogAnalytics.WorkspaceId, documentAnalysisOptions.LogAnalytics.AuthenticationId, documentAnalysisOptions.LogAnalytics.LogName)
		.Enrich.FromLogContext()
		.CreateLogger();


	// Add services to the container.
	builder.Services.AddControllers();
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	ConfigurationServicesApplication.ConfigureServices(builder, documentAnalysisOptions);

	builder.Services.AddHostedService<QueuedHostedService>();
	builder.Services.AddHostedService<PoolingHostedService>();

	var app = builder.Build();

	var options = app.Services.GetRequiredService<IOptions<DocumentAnalysisOptions>>();

	#region CREATE DB IF NOT EXISTS

	using var scope = app.Services.CreateScope();
	using DocumentAnalysisDbContext dbContext = scope.ServiceProvider.GetRequiredService<DocumentAnalysisDbContext>();
	dbContext.Database.EnsureCreated();
	SeedDatabase.Seed(dbContext);

	#endregion CREATE DB IF NOT EXISTS

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

	app.MapHealthChecks("/healthcheck");

	app.Run();

}
catch (Exception ex)
{
	if (Log.Logger != null)
	{
		Log.Fatal(ex, "Host terminated unexpectedly");
	}
	return -1;
}
finally
{
	if (Log.Logger != null)
	{
		Log.CloseAndFlush();
	}
}
return 0;