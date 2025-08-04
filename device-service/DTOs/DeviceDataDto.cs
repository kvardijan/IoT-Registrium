using System.ComponentModel.DataAnnotations;

namespace device_service.DTOs
{
    public class DeviceDataDto
    {
        [Required]
        public object RecordedData { get; set; }
    }
}
