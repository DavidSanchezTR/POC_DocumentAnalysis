using Aranzadi.DocumentAnalysis;
using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Util;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

#region Configure keyvault
StoreLocation storeLocation = CertificateModes.Webapp.Equals(builder.Configuration[$"{nameof(DocumentAnalysisOptions.KeyVault)}:{nameof(KeyVaultSettings.CertificateMode)}"], StringComparison.OrdinalIgnoreCase)
	? StoreLocation.CurrentUser : StoreLocation.LocalMachine;
using var store = new X509Store(StoreName.My, storeLocation);
store.Open(OpenFlags.ReadOnly);
string thumbprint = builder.Configuration[$"{nameof(DocumentAnalysisOptions.KeyVault)}:{nameof(KeyVaultSettings.CertificateThumbprint)}"];
if (!string.IsNullOrEmpty(thumbprint))
{
	var certs = store.Certificates
		.Find(X509FindType.FindByThumbprint, thumbprint, false);
	if (certs.Count == 0)
		throw new Exception($"Could not find certificate by thumbprint {thumbprint}. Environment was resolved to: " + builder.Environment.EnvironmentName + ", searched in store" + storeLocation);
	string keyvaultUri = builder.Configuration[$"{nameof(DocumentAnalysisOptions.KeyVault)}:{nameof(KeyVaultSettings.Url)}"];
	string adTenantId = builder.Configuration[$"{nameof(DocumentAnalysisOptions.KeyVault)}:{nameof(KeyVaultSettings.ActiveDirectoryTenantId)}"];
	string clientId = builder.Configuration[$"{nameof(DocumentAnalysisOptions.KeyVault)}:{nameof(KeyVaultSettings.ClientAppId)}"];
	builder.Configuration.AddAzureKeyVault(new Uri(keyvaultUri),
		new ClientCertificateCredential(adTenantId, clientId, certs.OfType<X509Certificate2>().Single())
			, new ConditionalIgnoreSecretManager(builder.Environment, builder.Configuration[$"{nameof(DocumentAnalysisOptions.EnvironmentPrefix)}"]));
	store.Close();
}
#endregion Configure keyvault

//TEST - 
var configApiSecret = builder.Configuration.GetValue<string>("ApiSecret");



string connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DocumentAnalysisDbContext>(dbOptions =>
{
	dbOptions.UseCosmos(connString, DocumentAnalysisDbContext.DatabaseName, (cosmosOptions) =>
	{
		cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5));
	});
});




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigurationServicesApplication.ConfigureServices(builder);

var app = builder.Build();

var options = app.Services.GetRequiredService<IOptions<DocumentAnalysisOptions>>();
app.Services.GetRequiredService<DocumentAnalysisDbContext>().Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.EnvironmentName == "DEV")
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
