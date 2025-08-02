using location_service.DTOs;
using location_service.Models;
using location_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace location_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;
        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _locationService.GetLocations();
            if (locations == null)
            {
                return NotFound(ApiResponse<object>.Fail("No location found.", 404));
            }
            return Ok(ApiResponse<List<Location>>.Ok(locations));
        }

        [HttpGet("{id}")]
        public IActionResult GetLocationById(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound(ApiResponse<object>.Fail("Location with the provided id was not found.", 404));
            }
            return Ok(ApiResponse<Location>.Ok(location));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateLocation([FromBody] LocationCreationDto locationCreationDto)
        {
            var location = _locationService.CreateLocation(locationCreationDto);
            if (location == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Failed to create location.", 400));
            }
            return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, ApiResponse<Location>.Ok(location));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult UpdateLocation(int id, [FromBody] LocationUpdateDto locationUpdateDto)
        {
            var updatedLocation = _locationService.UpdateLocation(id, locationUpdateDto);
            if(updatedLocation == null)
            {
                return NotFound(ApiResponse<object>.Fail("Location not found", 404));
            }
            return Ok(ApiResponse<Location>.Ok(updatedLocation));
        }
    }
}
