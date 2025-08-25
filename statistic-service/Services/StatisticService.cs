using statistic_service.DTOs;

namespace statistic_service.Services
{
    public class StatisticService
    {
        private readonly DataFetchingService _dataFetchingService;

        public StatisticService(DataFetchingService dataFetchingService)
        {
            _dataFetchingService = dataFetchingService;
        }

        public async Task<StatusStatisticResponse> getDeviceStatusStatistic()
        {
            return new StatusStatisticResponse();
        }
    }
}
