using statistic_service.DTOs;

namespace statistic_service.Services
{
    public class StatisticService
    {
        private readonly IDataFetchingService _dataFetchingService;

        public StatisticService(IDataFetchingService dataFetchingService)
        {
            _dataFetchingService = dataFetchingService;
        }

        public async Task<StatusStatisticResponse> getDeviceStatusStatistic()
        {
            int active = 0;
            int idle = 0;
            int deactivated = 0;
            int error = 0;
            var devices = await _dataFetchingService.GetDevices();

            foreach (var device in devices)
            {
                switch (device.StatusId)
                {
                    case 1:
                        active++; break;
                    case 2:
                        idle++; break;
                    case 3:
                        deactivated++; break;
                    case 4:
                        error++; break;
                    default:
                        break;
                }
            }

            return new StatusStatisticResponse
            {
                Active = active,
                Idle = idle,
                Deactivated = deactivated,
                Error = error
            };
        }
    }
}
