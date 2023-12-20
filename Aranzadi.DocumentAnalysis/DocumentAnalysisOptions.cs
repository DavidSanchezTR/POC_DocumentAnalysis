using Aranzadi.DocumentAnalysis.Util;
using Aranzadi.HttpPooling;
using Azure.Identity;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Aranzadi.DocumentAnalysis;

public static class ApplicationSettings
{
	public static DocumentAnalysisOptions GetDocumentAnalysisOptions(IConfiguration configuration)
	{
		DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
		configuration.Bind(documentAnalysisOptions);
		return documentAnalysisOptions;
	}

	public static KeyVaultSettings GetKeyVaultSettings(IConfiguration configuration)
	{
		KeyVaultSettings keyVaultSettings = new KeyVaultSettings();
		configuration.Bind("KeyVault", keyVaultSettings);
		return keyVaultSettings;
	}

	public static DocumentAnalysisOptions InitConfiguration(WebApplicationBuilder builder, string appSettingsFile)
	{
		builder.Configuration.AddJsonFile(appSettingsFile, optional: true, reloadOnChange: true);
		builder.Configuration.AddEnvironmentVariables();

		var documentAnalysisOptions = GetDocumentAnalysisOptions(builder.Configuration);

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
					, new ConditionalIgnoreSecretManager(builder.Environment, documentAnalysisOptions.EnvironmentPrefix));
			store.Close();

		}
		documentAnalysisOptions = GetDocumentAnalysisOptions(builder.Configuration);
		documentAnalysisOptions.Environment = builder.Environment.EnvironmentName;
        #endregion Configure keyvault

        return documentAnalysisOptions;
	}

}

public class DocumentAnalysisOptions
{
    public DocumentAnalysisOptions()
    {
		AnalysisProvider = new AnalysisProviderClass()
		{
			UrlApiJobs = string.Empty,
			UrlApiDocuments = string.Empty,
			ApiKey = string.Empty
		};

	}
    public string? Environment { get; set; }

    public string? EnvironmentPrefix { get; set; }

	public string? CosmosDatabaseName { get; set; }

	public KeyVaultSettings? KeyVault { get; set; }

	public ConnectionStringsClass? ConnectionStrings { get; set; }

	public class ConnectionStringsClass
	{
		public string? DefaultConnection { get; set; }
	}

	public ServiceBusClass? Messaging { get; set; }
	public class ServiceBusClass
	{
		public string? Queue { get; set; }
		public string? Endpoint { get; set; }
	}

	public class DocumentAnalysisEnvironment
	{
		public DocumentAnalysisEnvironment(string contentRootPath, IHostEnvironment environment)
		{
			ContentRootPath = contentRootPath;
			Environment = environment;
		}

		public string ContentRootPath { get; set; }
		public IHostEnvironment Environment { get; set; }
	}

	public ApplicationInsightsClass? ApplicationInsights { get; set; }
	public class ApplicationInsightsClass
	{
		public string? ConnectionString { get; set; }
	}

	public LogAnalyticsClass? LogAnalytics { get; set; }
	public class LogAnalyticsClass
	{
		public string? WorkspaceId { get; set; }
		public string? AuthenticationId { get; set; }
		public string? LogName { get; set; }
	}
	public bool CheckIfActiveCreditsConsumption { get; set; }
	public CreditsConsumptionClass? CreditsConsumption { get; set; }
	public class CreditsConsumptionClass
    {
        public string? CreditsConsumptionService { get; set; }
        public string? AzureADCreditsConsumptionScope { get; set; }
		public string? AzureADCreditsConsumptionTenant { get; set; }
	}

    

	public AnalysisProviderClass? AnalysisProvider { get; set; }
	public class AnalysisProviderClass
	{
		public string? UrlApiJobs { get; set; }
		public string? UrlApiDocuments { get; set; }
		public string? ApiKey { get; set; }
	}

	public PoolingConfiguration Pooling { get; set; }

}

public class EnvironmentNames
{
	public const string Debug = "Debug";
	public const string Dev = "Dev";
	public const string Pre = "Pre";
	public const string QA = "QA";
	public const string RC = "RC";
	public const string Pro = "Pro";
}

public static class CertificateModes
{
	public const string Webapp = "webapp";
	public const string VirtualMachine = "virtualmachine";
}


public class KeyVaultSettings
{
	public string? Url { get; set; }
	public string? CertificateThumbprint { get; set; }
	public string? ClientAppId { get; set; }
	public string? ActiveDirectoryTenantId { get; set; }
	public string? CertificateMode { get; set; }
}
