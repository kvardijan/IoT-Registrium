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
    }
}
