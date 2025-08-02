using location_service.Models;

namespace location_service.Services
{
    public class LocationService
    {
        private readonly LocationsDbContext _context;
        public LocationService(LocationsDbContext context)
        {
            _context = context;
        }
    }
}
