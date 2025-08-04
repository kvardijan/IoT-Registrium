using device_service.DTOs;
using device_service.Models;
using System.Net.Http;
using System.Text.Json;

namespace device_service.Services
{
    public class EventCreationService
    {

        private readonly HttpClient _httpClient;

        public EventCreationService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EventService");
        }
        public async Task CreateDeviceAddedEventAsync(Device newDevice, string jwtToken)
        {
            var eventPayload = new
            {
                Device = newDevice.SerialNumber,
                Type = 6, // device added
                Data = JsonSerializer.Serialize(MapDeviceEventDto(newDevice))
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        public async Task CreateDeviceInfoUpdatedEventAsync(Device oldDevice, Device newDevice, string jwtToken)
        {
            var eventPayload = new
            {
                Device = newDevice.SerialNumber,
                Type = 1, // device info updated
                Data = JsonSerializer.Serialize(new
                {
                    old = MapDeviceEventDto(oldDevice),
                    @new = MapDeviceEventDto(newDevice)
                })
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        public async Task CreateDeviceStatusChangeEventAsync(string device, string oldStatus, string newStatus, string jwtToken)
        {
            var eventPayload = new
            {
                Device = device,
                Type = 2, // device status change
                Data = JsonSerializer.Serialize(new
                {
                    oldStatus = oldStatus,
                    newStatus = newStatus
                })
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        public async Task CreateDeviceDataRecordingEventAsync(string device, DeviceDataDto data, string jwtToken)
        {
            var eventPayload = new
            {
                Device = device,
                Type = 5, // device received data
                Data = JsonSerializer.Serialize(new
                {
                    RecordedData = data.RecordedData
                })
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        public async Task CreateDeviceSentCommandEventAsync(string device, DeviceCommandDto command, string jwtToken)
        {
            var eventPayload = new
            {
                Device = device,
                Type = 3, // device sent command
                Data = JsonSerializer.Serialize(new
                {
                    Command = command.Command
                })
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        public async Task CreateDeviceFirmwareChangeEventAsync(string device, string oldFirmwareVersion, string newFirmwareVersion, string jwtToken)
        {
            var eventPayload = new
            {
                Device = device,
                Type = 4, // device firmware update
                Data = JsonSerializer.Serialize(new
                {
                    oldFirmwareVersion = oldFirmwareVersion,
                    newFirmwareVersion = newFirmwareVersion
                })
            };

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/event", eventPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create event: {response.StatusCode}");
            }
        }

        private DeviceEventDto MapDeviceEventDto(Device device)
        {
            return new DeviceEventDto
            {
                Id = device.Id,
                SerialNumber = device.SerialNumber,
                Model = device.Model,
                Manufacturer = device.Manufacturer,
                Type = device.Type,
                Status = device.Status,
                FirmwareVersion = device.FirmwareVersion,
                Location = device.Location,
                LastSeen = device.LastSeen
            };
        }
    }
}
