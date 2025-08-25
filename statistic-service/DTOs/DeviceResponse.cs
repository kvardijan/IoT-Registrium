namespace statistic_service.DTOs
{
    public class DeviceResponse
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public int? Location { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
