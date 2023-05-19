namespace Aranzadi.DocumentAnalysis;

public static class ApplicationSettings
{
    public static DocumentAnalysisOptions GetDocumentAnalysisOptions(IConfiguration configuration)
    {
        DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();

        documentAnalysisOptions.SecretsIncludedFromKeyVault = new List<string>()
        {
            //Comentar o descomentar para que escuche en la cola blue o green
            //"uksouth-iflx-dev-blue-orch-Messaging--Endpoint", //ServiceBus:ConnectionString en entorno dev blue
			"uksouth-iflx-dev-green-orch-Messaging--Endpoint" //ServiceBus:ConnectionString en entorno dev green
         };

        configuration.Bind(documentAnalysisOptions);


        var keyNames = configuration.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
        // bind other secrets included from Keyvault
        if (documentAnalysisOptions.SecretsIncludedFromKeyVault != null)
        {
            foreach (var secretName in documentAnalysisOptions.SecretsIncludedFromKeyVault)
            {
                switch (secretName)
                {
                    case "uksouth-iflx-dev-blue-orch-Messaging--Endpoint":
                        documentAnalysisOptions.ServiceBus.ConnectionString = keyNames.ContainsKey(secretName)
                                                                              ? configuration.GetValue<string>(secretName)
                                                                              : documentAnalysisOptions.ServiceBus.ConnectionString;
                        break;
					case "uksouth-iflx-dev-green-orch-Messaging--Endpoint":
						documentAnalysisOptions.ServiceBus.ConnectionString = keyNames.ContainsKey(secretName)
																			  ? configuration.GetValue<string>(secretName)
																			  : documentAnalysisOptions.ServiceBus.ConnectionString;
						break;
					default:
                        break;
                }
            }
        }

        return documentAnalysisOptions;
    }

    public static KeyVaultSettings GetKeyVaultSettings(IConfiguration configuration)
    {
        KeyVaultSettings keyVaultSettings = new KeyVaultSettings();

        configuration.Bind("KeyVault", keyVaultSettings);

        return keyVaultSettings;
    }


}

public class DocumentAnalysisOptions
{

    public List<string> SecretsIncludedFromKeyVault { get; set; }

    public string EnvironmentPrefix { get; set; }

    public string CosmosDatabaseName { get; set; }

    public KeyVaultSettings KeyVault { get; set; }

    public ConnectionStringsClass ConnectionStrings { get; set; }

    public class ConnectionStringsClass
    {
        public string DefaultConnection { get; set; }
    }

    public ServiceBusClass ServiceBus { get; set; }
    public class ServiceBusClass
    {
        public string ConnectionString { get; set; }
        public string Queue { get; set; }
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

    public ApplicationInsightsClass ApplicationInsights { get; set; }
    public class ApplicationInsightsClass
    {
        public string ConnectionString { get; set; }
    }

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
    public string Url { get; set; }
    public string CertificateThumbprint { get; set; }
    public string ClientAppId { get; set; }
    public string ActiveDirectoryTenantId { get; set; }
    public string CertificateMode { get; set; }
}



