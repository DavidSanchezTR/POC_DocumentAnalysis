using Aranzadi.DocumentAnalysis.Services.IServices;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Aranzadi.DocumentAnalysis.Services
{
    public class KeyVaultService : IKeyvaultService
    {
        static SecretClient client;
        //private IConfiguration _configuration;

        public KeyVaultService(IConfiguration configuration, SecretClient secretClient)
        {            
            if (secretClient == null)
            {

                string keyVaultUrl = configuration.GetSection("KeyVault").GetValue<string>("Url");
                client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());

            }
            else
            {
                client = secretClient;
            }
        }

        public KeyVaultService(IConfiguration configuration)
        {
            if (client == null)
            {
                string keyVaultUrl = configuration.GetSection("KeyVault").GetValue<string>("Url");
                client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
            }
        }

        public string GetValueFromKV(string key)
        {
            //string keySecret = Environment.GetEnvironmentVariable(key);
            return client.GetSecret(key).Value.Value;
        }
    }
}
