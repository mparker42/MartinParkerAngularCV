using MartinParkerAngularCV.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.Utilities
{
    public class BlobStoreHelper
    {
        private readonly IOptions<BlobStoreConfiguration> _config;
        private BlobStoreConfiguration Config { get { return _config.Value; } }

        private KeyVaultHelper KeyVaultHelper { get; }

        public BlobStoreHelper(IOptions<BlobStoreConfiguration> config, KeyVaultHelper keyVaultHelper)
        {
            _config = config;
            KeyVaultHelper = keyVaultHelper;
        }

        public async Task<string> GetBlobAsString(string path)
        {
            string storageConnectionString = await KeyVaultHelper.GetSecret(Config.StoreSecretURL);

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                var cloudBlobContainer = cloudBlobClient.GetContainerReference("Core"); ;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(path);

                return await cloudBlockBlob.DownloadTextAsync();
            }
            else
                throw new Exception("Could not parse the connection string passed from the key vault as a storage account connection string.");
        }
    }
}
