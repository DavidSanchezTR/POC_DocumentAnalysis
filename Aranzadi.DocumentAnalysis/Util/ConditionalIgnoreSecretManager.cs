using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Util
{
    internal class ConditionalIgnoreSecretManager : KeyVaultSecretManager
    {
        private IHostEnvironment hostingEnvironment;
        private string environmentPrefix;
        private List<string> secretsIncludedFromKeyVault;

        public ConditionalIgnoreSecretManager(IHostEnvironment hostingEnvironment, string environmentPrefix, List<string> secretsIncludedFromKeyVault)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.environmentPrefix = environmentPrefix;
            this.secretsIncludedFromKeyVault = secretsIncludedFromKeyVault;
        }

        public override string GetKey(KeyVaultSecret secret)
        {
            if (secretsIncludedFromKeyVault.Contains(secret.Name))
            {
                return secret.Name;
            }
            else
            {
                return UnPrefix(secret.Name);
            }
        }

        /// <summary>
        /// Removes the environment prefix from a secret.
        /// Example: uksouth-iflx-pre-customerportal-ConnectionStrings--StorageConnection becomes ConnectionStrings:StorageConnection
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        private string UnPrefix(string secretName)
        {
            return secretName.Replace("--", ":").Split("-").Last();
        }

        public override bool Load(SecretProperties secret)
        {
            if (secretsIncludedFromKeyVault.Contains(secret.Name))
                return true;

            if (!secret.Name.StartsWith(environmentPrefix))
                return false;
            // return true; // uncomment to run all settings (including connectionstrings) from keyvault
            //in debug (local machine), don't load the connectionstrings 
            return !(hostingEnvironment.IsEnvironment(EnvironmentNames.Debug) && UnPrefix(secret.Name).StartsWith("DefaultConnection"));
        }
    }
}
