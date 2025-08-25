namespace statistic_service.Services
{
    public interface IDataFetchingService
    {

    }

    public class DataFetchingService : IDataFetchingService
    {
        private readonly HttpClient _httpClientDevices;
        private readonly HttpClient _httpClientLocations;
        private readonly HttpClient _httpClientEvents;

        public DataFetchingService(IHttpClientFactory httpClientFactoryDevices, IHttpClientFactory httpClientFactoryLocations, IHttpClientFactory httpClientFactoryEvents)
        {
            _httpClientDevices = httpClientFactoryDevices.CreateClient("DeviceService");
            _httpClientLocations = httpClientFactoryLocations.CreateClient("LocationService");
            _httpClientEvents = httpClientFactoryEvents.CreateClient("EventService");
        }
    }
}
