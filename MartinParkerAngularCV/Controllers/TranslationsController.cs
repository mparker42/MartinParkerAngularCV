using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MartinParkerAngularCV.SharedUtilities;
using MartinParkerAngularCV.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MartinParkerAngularCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
        private TranslationHelper TranslationHelper { get; }

        public TranslationsController(TranslationHelper translationHelper)
        {
            TranslationHelper = translationHelper;
        }

        private async Task<IActionResult> GetTranslations(string package)
        {
            CultureInfo culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture;

            string safeLocale = TranslationHelper.TransformToSafeLocale(culture.TwoLetterISOLanguageName);
            bool isRTL = culture.TextInfo.IsRightToLeft;

            Dictionary<string, string> translations = await TranslationHelper.GetTranslationPackage($"{package}/{safeLocale}.json");

            return Ok(new { resolvedLocale = safeLocale, translations, isRTL });
        }

        // We need to have a function per translation package to prevent people navigating around the blob store
        [HttpGet("Core")]
        public async Task<IActionResult> GetCoreTranslations()
        {
            return await GetTranslations("core");
        }

        [HttpGet("Navigation")]
        public async Task<IActionResult> GetNavigationTranslations()
        {
            return await GetTranslations("navigation");
        }

        [HttpGet("Reset/{packageName}/{locale}")]
        public async Task<IActionResult> ResetTransaltionPackage(string packageName, string locale)
        {
            await TranslationHelper.AddTranslationsPackageIntoCache($"{packageName}/{locale}.json");

            return NoContent();
        }
    }
}