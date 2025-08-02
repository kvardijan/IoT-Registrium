using device_service.DTOs;
using device_service.Models;
using device_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace device_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult RegisterDevice([FromBody] DeviceRegistrationDto deviceRegistrationDto)
        {
            var device = _deviceService.RegisterDevice(deviceRegistrationDto);

            if (device == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to register device.", 400));
            }

            return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, ApiResponse<Device>.Ok(device));
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetDeviceById(int id)
        {
            var device = _deviceService.GetDeviceById(id);
            if (device == null)
            {
                return NotFound(ApiResponse<object>.Fail("Device not found", 404));
            }
            return Ok(ApiResponse<Device>.Ok(device));
        }
    }
}
