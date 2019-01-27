using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.SharedUtils
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

        private CloudBlobContainer _cachedCoreContainer;
        private DateTimeOffset _cachedCoreContainerExpiry = DateTimeOffset.Now;

        private async Task<CloudBlobContainer> GetCoreContainer()
        {
            if(_cachedCoreContainer != null && DateTimeOffset.Now > _cachedCoreContainerExpiry)
                return _cachedCoreContainer;

            string storageConnectionString = await KeyVaultHelper.GetSecret(Config.StoreSecretURL);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            _cachedCoreContainer = blobClient.GetContainerReference("core");
            _cachedCoreContainerExpiry = DateTimeOffset.Now.AddHours(1);

            return _cachedCoreContainer;
        }

        public async Task<string> GetBlobAsString(string path)
        {
            CloudBlobContainer coreContainer = await GetCoreContainer();

            CloudBlockBlob blob = coreContainer.GetBlockBlobReference(path);

            return await blob.DownloadTextAsync();
        }

        public async Task UploadFolderToBlob(string path)
        {
            CloudBlobContainer coreContainer = await GetCoreContainer();

            IEnumerable<string> files = Directory.EnumerateFiles(path, "*", new EnumerationOptions() { RecurseSubdirectories = true });

            int substringStart = path.Length;

            foreach (string file in files)
                coreContainer
                    .GetBlockBlobReference(file.Substring(substringStart).TrimStart('\\').Replace('\\', '/'))
                    .UploadFromFile(file);
        }
    }
}
