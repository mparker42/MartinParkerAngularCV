using MartinParkerAngularCV.SharedUtils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private KeyVaultHelper KeyVaultHelper { get; }

        public ConfigurationController(KeyVaultHelper keyVaultHelper)
        {
            KeyVaultHelper = keyVaultHelper;
        }

        [HttpGet("{secretName}")]
        public async Task<IActionResult> GetConfigurationValue(string secretName)
        {
            return Ok(new
            {
                value = await KeyVaultHelper.GetSecret(secretName)
            });
        }
    }
}