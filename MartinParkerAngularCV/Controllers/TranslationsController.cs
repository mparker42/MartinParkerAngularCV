using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MartinParkerAngularCV.SharedUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MartinParkerAngularCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
        private KeyVaultHelper KeyVaultHelper { get; }
        private BlobStoreHelper BlobStoreHelper { get; }
        private IDistributedCache DistributedCache { get; }
        private readonly string _cachePrefix = "Translations_",
            _blobPrefix = "Translations/";

        public TranslationsController(KeyVaultHelper keyVaultHelper, IDistributedCache distributedCache, BlobStoreHelper blobStoreHelper)
        {
            KeyVaultHelper = keyVaultHelper;
            BlobStoreHelper = blobStoreHelper;
            DistributedCache = distributedCache;
        }

        private string TransformToSafeLocale(string locale)
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

        private async Task<object> AddTranslationsPackageIntoCache(string packageName)
        {
            string translations = await BlobStoreHelper.GetBlobAsString(_blobPrefix + packageName);

            DistributedCache.Set(_cachePrefix + packageName, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(translations)));

            return translations;
        }

        private async Task<object> GetTranslationPackage(string packageName)
        {
            byte[] cachedPackageBytes = await DistributedCache.GetAsync(_cachePrefix + packageName);

            if (cachedPackageBytes != null)
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(cachedPackageBytes));

            return await AddTranslationsPackageIntoCache(packageName);
        }

        [HttpGet("Core/{locale}")]
        public async Task<IActionResult> GetCoreTranslations(string locale)
        {
            locale = TransformToSafeLocale(locale);

            return Ok(new { resolvedLocale = locale, translations = await GetTranslationPackage($"Core/{locale}.json") });
        }

        [HttpGet("Reset/{packageName}/{locale}")]
        public async Task<IActionResult> ResetTransaltionPackage(string packageName, string locale)
        {
            await AddTranslationsPackageIntoCache($"{packageName}/{locale}.json");

            return NoContent();
        }
    }
}