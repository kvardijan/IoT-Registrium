namespace statistic_service.DTOs
{
    public class StatusStatisticResponse
    {
        public int Active { get; set; }
        public int Idle { get; set; }
        public int Deactivated { get; set; }
        public int Error { get; set; }
    }
}
