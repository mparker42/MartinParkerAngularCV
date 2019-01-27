using MartinParkerAngularCV.SharedUtils;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.Utils
{
    public class TranslationHelper
    {
        private KeyVaultHelper KeyVaultHelper { get; }
        private BlobStoreHelper BlobStoreHelper { get; }
        private IDistributedCache DistributedCache { get; }
        private readonly string _cachePrefix = "Translations_",
            _blobPrefix = "Translations/";

        public TranslationHelper(KeyVaultHelper keyVaultHelper, IDistributedCache distributedCache, BlobStoreHelper blobStoreHelper)
        {
            KeyVaultHelper = keyVaultHelper;
            BlobStoreHelper = blobStoreHelper;
            DistributedCache = distributedCache;
        }

        internal string TransformToSafeLocale(string locale)
        {
            try
            {
                var culture = new CultureInfo(locale);

                return culture.TwoLetterISOLanguageName == "" ? "en" : culture.TwoLetterISOLanguageName;
            }
            catch (Exception)
            {
                return "en";
            }
        }

        internal async Task<Dictionary<string, string>> AddTranslationsPackageIntoCache(string packageName)
        {
            string translations = await BlobStoreHelper.GetBlobAsString(_blobPrefix + packageName);

            DistributedCache.Set(_cachePrefix + packageName, Encoding.UTF8.GetBytes(translations));

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(translations);
        }

        internal async Task<Dictionary<string, string>> GetTranslationPackage(string packageName)
        {
            byte[] cachedPackageBytes = await DistributedCache.GetAsync(_cachePrefix + packageName);

            if (cachedPackageBytes != null)
                return JsonConvert.DeserializeObject<Dictionary<string,string>>(Encoding.UTF8.GetString(cachedPackageBytes));

            return await AddTranslationsPackageIntoCache(packageName);
        }
    }
}
