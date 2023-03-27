namespace Aranzadi.DocumentAnalysis
{
    public class DocumentAnalysisOptions
    {
        public string CosmosDatabaseName { get; set; }

        public KeyVaultSettings KeyVault { get; set; }

        public class KeyVaultSettings
        {
            public string Url { get; set; }
            public string CertificateThumbprint { get; set; }
            public string ClientAppId { get; set; }
            public string ActiveDirectoryTenantId { get; set; }

            public string CertificateMode { get; set; }
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
    }
}
