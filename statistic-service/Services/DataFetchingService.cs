using statistic_service.DTOs;
using statistic_service.Models;
using System.Net.Http;

namespace statistic_service.Services
{
    public interface IDataFetchingService
    {
        Task<List<DeviceResponse>?> GetDevices();
        Task<List<EventResponse>?> GetEventsOfDevice(string serial, string jwtToken);
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

        public async Task<List<DeviceResponse>?> GetDevices()
        {
            try
            {
                var response = await _httpClientDevices.GetAsync("api/device");
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeviceResponse>>>();
                var devices = apiResponse.Data;

                return devices ?? new List<DeviceResponse>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EventResponse>?> GetEventsOfDevice(string serial, string jwtToken)
        {
            try
            {
                _httpClientEvents.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    _httpClientEvents.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
                }

                var response = await _httpClientEvents.GetAsync("api/event/device/" + serial);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventResponse>>>();
                var events = apiResponse.Data;

                return events ?? new List<EventResponse>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
