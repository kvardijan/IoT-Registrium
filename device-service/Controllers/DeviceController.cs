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
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationDto deviceRegistrationDto)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var device = await _deviceService.RegisterDevice(deviceRegistrationDto, jwtToken);

            if (device == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to register device.", 400));
            }

            return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, ApiResponse<DeviceResponse>.Ok(device));
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
            return Ok(ApiResponse<DeviceResponse>.Ok(device));
        }

        [HttpGet("serial/{serial}")]
        public IActionResult GetDeviceBySerialNumber(string serial)
        {
            var device = _deviceService.GetDeviceBySerialNumber(serial);
            if (device == null)
            {
                return NotFound(ApiResponse<object>.Fail("Device not found", 404));
            }
            return Ok(ApiResponse<DeviceResponse>.Ok(device));
        }

        [HttpGet]
        public IActionResult GetDevices()
        {
            var devices = _deviceService.GetDevices();
            if (devices == null)
            {
                return NotFound(ApiResponse<object>.Fail("No devices found", 404));
            }
            return Ok(ApiResponse<List<DeviceResponse>>.Ok(devices));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] DeviceUpdateDto deviceUpdateDto)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var updatedDevice = await _deviceService.UpdateDevice(id, deviceUpdateDto, jwtToken);

            if (updatedDevice == null)
            {
                return NotFound(ApiResponse<object>.Fail("Device not found", 404));
            }

            return Ok(ApiResponse<DeviceResponse>.Ok(updatedDevice));
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateDeviceStatus(int id, [FromBody] DeviceUpdateDto deviceUpdateDto)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var updatedDevice = await _deviceService.UpdateDeviceStatus(id, deviceUpdateDto, jwtToken);

            if (updatedDevice == null)
            {
                return NotFound(ApiResponse<object>.Fail("Device not found", 404));
            }

            return Ok(ApiResponse<DeviceResponse>.Ok(updatedDevice));
        }

        [Authorize]
        [HttpPatch("{id}/data")]
        public async Task<IActionResult> StoreDeviceData(int id, [FromBody] DeviceDataDto deviceDataDto)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString();
            var updatedDevice = await _deviceService.StoreDeviceData(id, deviceDataDto, jwtToken);

            if (updatedDevice == null)
            {
                return NotFound(ApiResponse<object>.Fail("Device not found", 404));
            }

            return Ok(ApiResponse<DeviceResponse>.Ok(updatedDevice));
        }
    }
}
