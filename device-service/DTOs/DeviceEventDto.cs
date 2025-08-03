namespace device_service.DTOs
{
    public class DeviceEventDto
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string FirmwareVersion { get; set; }
        public int? Location { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
