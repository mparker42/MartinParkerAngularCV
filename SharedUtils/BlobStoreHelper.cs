﻿using MartinParkerAngularCV.SharedUtils.Enums;
using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Storage;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

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

        private string GetContainerSecretURL(BlobContainerType containerType)
        {
            switch (containerType)
            {
                case BlobContainerType.Public:
                    return Config.PublicStoreSecretURL;
                case BlobContainerType.Internal:
                    return Config.InternalStoreSecretURL;
                default:
                    throw new ArgumentOutOfRangeException("No known secret url to get the container type connection string");
            }
        }

        private async Task<CloudBlobContainer> GetContainer(BlobContainerType containerType)
        {
            if(_cachedCoreContainer != null && DateTimeOffset.Now > _cachedCoreContainerExpiry)
                return _cachedCoreContainer;

            string storageConnectionString = await KeyVaultHelper.GetSecret(GetContainerSecretURL(containerType));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            _cachedCoreContainer = blobClient.GetContainerReference("core");
            _cachedCoreContainerExpiry = DateTimeOffset.Now.AddHours(1);

            return _cachedCoreContainer;
        }

        public async Task<string> GetBlobAsString(string path, BlobContainerType containerType)
        {
            CloudBlobContainer coreContainer = await GetContainer(containerType);

            CloudBlockBlob blob = coreContainer.GetBlockBlobReference(path);

            return await blob.DownloadTextAsync();
        }

        public async Task UploadFolderToBlob(string path, BlobContainerType containerType)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            CloudBlobContainer coreContainer = await GetContainer(containerType);

            IEnumerable<string> files = Directory.EnumerateFiles(path, "*", new EnumerationOptions() { RecurseSubdirectories = true });

            int substringStart = path.Length;

            foreach (string file in files)
            {
                CloudBlockBlob blob = coreContainer
                    .GetBlockBlobReference(file.Substring(substringStart).TrimStart('\\').Replace('\\', '/'));

                if (!provider.TryGetContentType(file, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                blob.Properties.ContentType = contentType;

                blob.UploadFromFile(file);
            }
        }
    }
}
