using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using statistic_service.Models;
using statistic_service.Services;
using statistic_service.DTOs;

namespace statistic_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly StatisticService _statisticService;

        public StatisticController(StatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDeviceStatusStatistic()
        {
            var statusStatistic = await _statisticService.GetDeviceStatusStatistic();

            if (statusStatistic == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to get device status statistic.", 400));
            }

            return Ok(ApiResponse<StatusStatisticResponse>.Ok(statusStatistic));
        }

        [Authorize]
        [HttpGet("temperature/{serial}")]
        public async Task<IActionResult> GetTemperatureDeviceStatistic(string serial)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var temperatureDeviceStatisticResponse = await _statisticService.GetTemperatureDeviceStatistic(serial, jwtToken);

            if (temperatureDeviceStatisticResponse == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to get temperature device statistic.", 400));
            }

            return Ok(ApiResponse<TemperatureDeviceStatisticResponse>.Ok(temperatureDeviceStatisticResponse));
        }

        [Authorize]
        [HttpGet("humidity/{serial}")]
        public async Task<IActionResult> GetHumidityDeviceStatistic(string serial)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var humidityDeviceStatisticResponse = await _statisticService.GetHumidityDeviceStatistic(serial, jwtToken);

            if (humidityDeviceStatisticResponse == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to get humidity device statistic.", 400));
            }

            return Ok(ApiResponse<HumidityDeviceStatisticResponse>.Ok(humidityDeviceStatisticResponse));
        }

        [Authorize]
        [HttpGet("smartbin/{serial}")]
        public async Task<IActionResult> GetSmartBinDeviceStatistic(string serial)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var smartBinDeviceStatisticResponse = await _statisticService.GetSmartBinDeviceStatistic(serial, jwtToken);

            if (smartBinDeviceStatisticResponse == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to get smart bin device statistic.", 400));
            }

            return Ok(ApiResponse<SmartBinDeviceStatisticResponse>.Ok(smartBinDeviceStatisticResponse));
        }
    }
}
