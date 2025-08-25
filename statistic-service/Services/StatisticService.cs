using statistic_service.DTOs;

namespace statistic_service.Services
{
    public class StatisticService
    {
        public async Task<StatusStatisticResponse> getDeviceStatusStatistic()
        {
            return new StatusStatisticResponse();
        }
    }
}
