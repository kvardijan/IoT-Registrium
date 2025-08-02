using location_service.Models;
using location_service.Services;
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
    }
}
