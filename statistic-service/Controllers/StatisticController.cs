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
        public async Task<IActionResult> getDeviceStatusStatistic()
        {
         return Ok(ApiResponse<StatusStatisticResponse>.Ok(null));
        }
    }
}
