using Aranzadi.DocumentAnalysis;
using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Util;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


var documentAnalysisOptions = ApplicationSettings.GetDocumentAnalysisOptions(builder.Configuration);

//if (false)
{

	#region Configure keyvault
	StoreLocation storeLocation = CertificateModes.Webapp.Equals(documentAnalysisOptions.KeyVault.CertificateMode, StringComparison.OrdinalIgnoreCase)
		? StoreLocation.CurrentUser : StoreLocation.LocalMachine;
	using var store = new X509Store(StoreName.My, storeLocation);
	store.Open(OpenFlags.ReadOnly);
	string thumbprint = documentAnalysisOptions.KeyVault.CertificateThumbprint;
	if (!string.IsNullOrEmpty(thumbprint))
	{
		var certs = store.Certificates
			.Find(X509FindType.FindByThumbprint, thumbprint, false);
		if (certs.Count == 0)
			throw new Exception($"Could not find certificate by thumbprint {thumbprint}. Environment was resolved to: " + builder.Environment.EnvironmentName + ", searched in store" + storeLocation);
		string keyvaultUri = documentAnalysisOptions.KeyVault.Url;
		string adTenantId = documentAnalysisOptions.KeyVault.ActiveDirectoryTenantId;
		string clientId = documentAnalysisOptions.KeyVault.ClientAppId;
		builder.Configuration.AddAzureKeyVault(new Uri(keyvaultUri),
			new ClientCertificateCredential(adTenantId, clientId, certs.OfType<X509Certificate2>().Single())
				, new ConditionalIgnoreSecretManager(builder.Environment, documentAnalysisOptions.EnvironmentPrefix, documentAnalysisOptions.SecretsIncludedFromKeyVault));
		store.Close();

	}
    documentAnalysisOptions = ApplicationSettings.GetDocumentAnalysisOptions(builder.Configuration);

    #endregion Configure keyvault
}



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry((x) => { x.ConnectionString = documentAnalysisOptions.ApplicationInsights.ConnectionString; });

ConfigurationServicesApplication.ConfigureServices(builder);

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

app.Run();
