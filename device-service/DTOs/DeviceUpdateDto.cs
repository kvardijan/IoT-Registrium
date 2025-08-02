namespace device_service.DTOs
{
    public class DeviceUpdateDto
    {
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public string? FirmwareVersion { get; set; }
        public int? Location { get; set; }
    }
}