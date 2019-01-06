using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MartinParkerAngularCV.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartinParkerAngularCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
        private KeyVaultHelper KeyVaultHelper { get; }

        public TranslationsController(KeyVaultHelper keyVaultHelper)
        {
            KeyVaultHelper = keyVaultHelper;
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


        [HttpGet("Core/{locale}")]
        public async Task<IActionResult> GetCoreTranslations(string locale)
        {
            locale = TransformToSafeLocale(locale);

            string testKey = await KeyVaultHelper.GetSecret("martinparkercvstoreConnectionString/c9c196809fe04584a65159ed0fc14d8d");

            return Ok(new { resolvedLocale = locale, testKey });
        }
    }
}