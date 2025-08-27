namespace statistic_service.DTOs
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string? Device { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
