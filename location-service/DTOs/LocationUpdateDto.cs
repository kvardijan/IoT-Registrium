using System.ComponentModel.DataAnnotations;

namespace location_service.DTOs
{
    public class LocationUpdateDto
    {
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}
