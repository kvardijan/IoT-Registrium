using System.ComponentModel.DataAnnotations;

namespace device_service.DTOs
{
    public class DeviceRegistrationDto
    {
        [Required]
        public string SerialNumber { get; set; }
        [Required]
        public string Model { get; set; }
        public string? Manufacturer { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string FirmwareVersion { get; set; }
        public int? Location { get; set; }
    }
}
