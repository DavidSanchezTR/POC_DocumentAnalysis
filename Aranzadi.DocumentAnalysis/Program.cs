using Aranzadi.DocumentAnalysis;
using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
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

var builder = WebApplication.CreateBuilder(args);

DocumentAnalysisOptions documentAnalysisOptions = ApplicationSettings.InitConfiguration(builder, "appsettings.json");

//Use Serilog
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.ApplicationInsights(new TelemetryConfiguration() { ConnectionString = documentAnalysisOptions.ApplicationInsights.ConnectionString }
		, TelemetryConverter.Traces)
	.WriteTo.AzureAnalytics(documentAnalysisOptions.LogAnalytics.WorkspaceId, documentAnalysisOptions.LogAnalytics.AuthenticationId, documentAnalysisOptions.LogAnalytics.LogName)
	.CreateLogger();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigurationServicesApplication.ConfigureServices(builder, documentAnalysisOptions);

builder.Services.AddHostedService<QueuedHostedService>();

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
