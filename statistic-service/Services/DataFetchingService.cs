using statistic_service.DTOs;
using statistic_service.Models;
using System.Net.Http;

namespace statistic_service.Services
{
    public interface IDataFetchingService
    {
        Task<List<DeviceResponse>> GetDevices();
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

        public async Task<List<DeviceResponse>> GetDevices()
        {
            var response = await _httpClientDevices.GetAsync("api/device");

            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeviceResponse>>>();
            var devices = apiResponse.Data;

            return devices ?? new List<DeviceResponse>();
        }
    }
}
