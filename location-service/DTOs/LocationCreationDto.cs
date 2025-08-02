using System.ComponentModel.DataAnnotations;

namespace location_service.DTOs
{
    public class LocationCreationDto
    {
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }
    }
}
