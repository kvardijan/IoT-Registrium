using event_service.DTOs;
using event_service.Models;
using event_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace event_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly SimulationService _simulationService;

        public SimulationController(SimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [Authorize]
        [HttpPost("start/{serialNumber}")]
        public IActionResult StartSimulation(string serialNumber, [FromBody] SimulationStartRequestDto dto)
        {
            var started = _simulationService.StartSimulation(serialNumber, dto.TypeId);
            return Ok(ApiResponse<bool>.Ok(started));
        }

        [Authorize]
        [HttpPost("stop/{serialNumber}")]
        public IActionResult StopSimulation(string serialNumber)
        {
            var stopped = _simulationService.StopSimulation(serialNumber);
            return Ok(ApiResponse<bool>.Ok(stopped));
        }

        [Authorize]
        [HttpPost("stop-all")]
        public IActionResult StopAllSimulations()
        {
            _simulationService.StopAllSimulations();
            return Ok(ApiResponse<bool>.Ok(true));
        }
    }

}
