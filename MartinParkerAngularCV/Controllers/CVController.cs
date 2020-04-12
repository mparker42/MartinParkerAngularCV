using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartinParkerAngularCV.Controllers
{
    [Route("cv")]
    [ApiController]
    public class CVController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetConfigurationValue(string secretName)
        {
            return Redirect("https://onedrive.live.com/redir?resid=CAF46874A058133D!678&authkey=!ACJumV_R8-dF43M&ithint=file%2cdocx;");
        }
    }
}