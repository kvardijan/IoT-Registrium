using statistic_service.DTOs;
using System.Text.Json;

namespace statistic_service.Services
{
    public class StatisticService
    {
        private readonly IDataFetchingService _dataFetchingService;

        public StatisticService(IDataFetchingService dataFetchingService)
        {
            _dataFetchingService = dataFetchingService;
        }

        public async Task<StatusStatisticResponse> GetDeviceStatusStatistic()
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

        public async Task<TemperatureDeviceStatisticResponse> GetTemperatureDeviceStatistic(string serial, string jwtToken)
        {
            float max = float.MinValue;
            float min = float.MaxValue;
            float sum = 0;
            int count = 0;

            var events = await _dataFetchingService.GetEventsOfDevice(serial, jwtToken);

            foreach (var e in events)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<EventDataWrapper>(e.Data,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (wrapper?.RecordedData?.Temperature != null)
                        {
                            var tempStr = wrapper.RecordedData.Temperature.Replace("C", "");
                            if (float.TryParse(tempStr, out float temperature))
                            {
                                max = Math.Max(max, temperature);
                                min = Math.Min(min, temperature);
                                sum += temperature;
                                count++;
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Failed to parse event data: {ex.Message}");
                    }
                }
            }

            var avg = count > 0 ? sum / count : 0;

            return new TemperatureDeviceStatisticResponse
            {
                Max = max,
                Min = min,
                Avg = avg
            };
        }
    }

    public class EventDataWrapper
    {
        public RecordedData RecordedData { get; set; }
    }

    public class RecordedData
    {
        public string? Temperature { get; set; }
    }
}
