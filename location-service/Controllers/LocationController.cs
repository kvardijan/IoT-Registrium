using location_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace location_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : Controller
    {
        private readonly LocationsDbContext _context;
        public LocationController(LocationsDbContext context)
        {
            _context = context;
        }
    }
}
