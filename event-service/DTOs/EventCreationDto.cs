using System.ComponentModel.DataAnnotations;

namespace event_service.DTOs
{
    public class EventCreationDto
    {
        public string? Device { get; set; }
        [Required]
        public int Type { get; set; }
        public string? Data { get; set; }
    }
}
