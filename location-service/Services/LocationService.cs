using location_service.DTOs;
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

        public Location? CreateLocation(LocationCreationDto locationCreationDto)
        {
            Location newLocation = MapLocation(locationCreationDto);
            _context.Locations.Add(newLocation);
            _context.SaveChanges();
            return newLocation;
        }

        public Location? UpdateLocation(int id, LocationUpdateDto locationUpdateDto)
        {
            var existingLocation = _context.Locations.FirstOrDefault(l => l.Id == id);
            if (existingLocation == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(locationUpdateDto.Latitude))
                existingLocation.Latitude = locationUpdateDto.Latitude;

            if (!string.IsNullOrEmpty(locationUpdateDto.Longitude))
                existingLocation.Longitude = locationUpdateDto.Longitude;

            if (!string.IsNullOrEmpty(locationUpdateDto.Address))
                existingLocation.Address = locationUpdateDto.Address;

            if (!string.IsNullOrEmpty(locationUpdateDto.Description))
                existingLocation.Description = locationUpdateDto.Description;

            _context.SaveChanges();
            return existingLocation;
        }

        private Location MapLocation(LocationCreationDto locationCreationDto)
        {
            return new Location
            {
                Latitude = locationCreationDto.Latitude,
                Longitude = locationCreationDto.Longitude,
                Address = locationCreationDto.Address,
                Description = locationCreationDto.Description
            };
        }
    }
}
