using System.ComponentModel.DataAnnotations;

namespace device_service.DTOs
{
    public class DeviceCommandDto
    {
        [Required]
        public string Command { get; set; }
    }
}
