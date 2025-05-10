using Microsoft.AspNetCore.Mvc;
using SmartHomeBackend.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHomeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TuyaController : ControllerBase
    {
        private readonly TuyaService _tuyaService;

        public TuyaController(TuyaService tuyaService)
        {
            _tuyaService = tuyaService;
        }

        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            var result = await _tuyaService.GetTokenAsync();
            return Ok(JsonDocument.Parse(result));
        }

        [HttpGet("device-status/{deviceId}")]
        public async Task<IActionResult> GetDeviceStatus(string deviceId, [FromQuery] string token)
        {
            string signUrl = $"/v1.0/devices/{deviceId}/status";
            var result = await _tuyaService.SendTuyaRequestAsync(signUrl, "GET", null, token);
            return Ok(JsonDocument.Parse(result));
        }
    }

}
