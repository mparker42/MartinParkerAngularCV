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

        [HttpGet("Core/{locale}")]
        public async Task<IActionResult> GetCoreTranslations(string locale)
        {
            CultureInfo culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture;

            string safeLocale = TranslationHelper.TransformToSafeLocale(culture.TwoLetterISOLanguageName);
            bool isRTL = culture.TextInfo.IsRightToLeft;

            Dictionary<string,string> translations = await TranslationHelper.GetTranslationPackage($"core/{safeLocale}.json");

            return Ok(new { resolvedLocale = locale, translations, isRTL });
        }

        [HttpGet("Reset/{packageName}/{locale}")]
        public async Task<IActionResult> ResetTransaltionPackage(string packageName, string locale)
        {
            await TranslationHelper.AddTranslationsPackageIntoCache($"{packageName}/{locale}.json");

            return NoContent();
        }
    }
}