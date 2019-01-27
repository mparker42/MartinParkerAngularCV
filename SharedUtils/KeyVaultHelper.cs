using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.KeyVault;
using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MartinParkerAngularCV.SharedUtils
{
    public class KeyVaultHelper
    {
        private readonly IOptions<KeyVaultConfiguration> _config;
        private KeyVaultConfiguration Config { get { return _config.Value; } }

        // I have nominated to use the in memory cache here instead of a distributed cache for speed and to prevent the hastle of a circular reference.
        // The configuration information for the distributed cache would have to be stored in the key vault as this code is shared publicly.
        private readonly IMemoryCache _memoryCache;
        private readonly string _cachePrefix = "KeyVaultHelper_";

        public KeyVaultHelper(IOptions<KeyVaultConfiguration> config, IMemoryCache memoryCache)
        {
            _config = config;
            _memoryCache = memoryCache;
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            string paramsKey = $"{_cachePrefix}{authority};{resource};{scope}";

            if (_memoryCache.TryGetValue(paramsKey, out string cachedToken))
                return cachedToken;

            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Environment.GetEnvironmentVariable("KeyVaultClientID"),
                        Environment.GetEnvironmentVariable("KeyVaultClientSecret"));
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException($"Could not retrieve client credetials token for key vault");

            // Caching this will save us a lot of potential requests
            string accessToken = result.AccessToken;
            DateTimeOffset accessTokenExpiry = result.ExpiresOn.AddMinutes(-1);

            _memoryCache.Set(paramsKey, accessToken, accessTokenExpiry);

            return accessToken;
        }

        public async Task<string> GetSecret(string secretPath)
        {
            if (_memoryCache.TryGetValue(_cachePrefix + secretPath, out string cachedToken))
                return cachedToken;

            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(Config.BaseSecretURL + secretPath);

            // For safety we don't want to keep hold of results indefinately.
            // This will allow a one day lead on retiring old keys.
            _memoryCache.Set(secretPath, secret.Value, DateTimeOffset.Now.AddDays(1));

            return secret.Value;
        }
    }
}
