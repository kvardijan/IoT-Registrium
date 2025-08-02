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

        public List<Location> GetLocations()
        {
            return _context.Locations.ToList();
        }

        public Location? GetLocationById(int id)
        {
            return _context.Locations.FirstOrDefault(l => l.Id == id);
        }
    }
}
