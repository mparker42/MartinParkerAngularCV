using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.KeyVault;
using MartinParkerAngularCV.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.KeyVault.Models;

namespace MartinParkerAngularCV.Utilities
{
    public class KeyVaultHelper
    {
        private readonly IOptions<KeyVaultConfiguration> _config;
        private KeyVaultConfiguration Config { get { return _config.Value; } }

        public KeyVaultHelper(IOptions<KeyVaultConfiguration> config)
        {
            _config = config;
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Environment.GetEnvironmentVariable("KeyVaultClientID"),
                        Environment.GetEnvironmentVariable("KeyVaultClientSecret"));
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException($"Could not retrieve client credetials token for key vault");

            return result.AccessToken;
        }

        public async Task<string> GetSecret(string secretPath)
        {
            KeyVaultClient kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            SecretBundle sec = await kv.GetSecretAsync(Config.BaseSecretURL + secretPath);

            return sec.Value;
        }
    }
}
