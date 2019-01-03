using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartinParkerAngularCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
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


        [HttpGet("Core")]
        public IActionResult GetCoreTranslations(string locale)
        {
            locale = TransformToSafeLocale(locale);

            return Ok(new { resolvedLocale = locale });
        }
    }
}