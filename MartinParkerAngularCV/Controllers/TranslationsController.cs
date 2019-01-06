using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MartinParkerAngularCV.Utilities;
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
        private readonly string _cachePrefix = "Translations_";
        private readonly string _blobPrefix = "Translations/";

        public TranslationsController(KeyVaultHelper keyVaultHelper, IDistributedCache distributedCache, BlobStoreHelper blobStoreHelper)
        {
            KeyVaultHelper = keyVaultHelper;
            BlobStoreHelper = blobStoreHelper;
            DistributedCache = distributedCache;
        }

        private string TransformToSafeLocale(string locale)
        {
            locale = locale.ToLowerInvariant();

            // Concat five letter locales e.g. en-GB to their two letter counterparts there is no need for this site to support regional dialects.
            if (Regex.Match(locale, "[A-z]{2}-[A-z]{2}").Success)
                locale = locale.Substring(0, 2);
            // I will only support locales in two formats five letter (e.g. en-GB) and two letter locales (e.g. fr)
            else if (!Regex.Match(locale, "[A-z]{2}").Success)
                locale = "en";

            return locale;
        }

        private async Task<object> AddTranslationsPackageIntoCache(string packageName)
        {
            var translations = BlobStoreHelper.GetBlobAsString(_blobPrefix + packageName);

            DistributedCache.Set(_cachePrefix + packageName, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(translations)));

            return translations;
        }

        private async Task<object> GetTranslationPackage(string packageName)
        {
            var cachedPackageBytes = await DistributedCache.GetAsync(_cachePrefix + packageName);

            if (cachedPackageBytes != null)
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(cachedPackageBytes));

            return await AddTranslationsPackageIntoCache(packageName);
        }

        [HttpGet("Core/{locale}")]
        public async Task<IActionResult> GetCoreTranslations(string locale)
        {
            locale = TransformToSafeLocale(locale);

            return Ok(new { resolvedLocale = locale, translations = GetTranslationPackage($"Core/{locale}.json") });
        }

        [HttpGet("Reset/{packageName}/{locale}")]
        public async Task<IActionResult> ResetTransaltionPackage(string packageName, string locale)
        {
            await AddTranslationsPackageIntoCache($"{packageName}/{locale}.json");

            return NoContent();
        }
    }
}