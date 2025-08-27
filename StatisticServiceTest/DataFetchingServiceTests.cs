using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using statistic_service.DTOs;
using statistic_service.Services;
using statistic_service.Models;

namespace DataFetchingServiceTests
{
    public class DataFetchingServiceTests
    {
        private DataFetchingService CreateService(
            out Mock<HttpMessageHandler> devicesHandler,
            out Mock<HttpMessageHandler> locationsHandler,
            out Mock<HttpMessageHandler> eventsHandler,
            HttpResponseMessage? devicesResponse = null,
            HttpResponseMessage? eventsResponse = null
        )
        {
            devicesHandler = new Mock<HttpMessageHandler>();
            locationsHandler = new Mock<HttpMessageHandler>();
            eventsHandler = new Mock<HttpMessageHandler>();

            var devicesClient = new HttpClient(devicesHandler.Object) { BaseAddress = new Uri("http://localhost/") };
            var locationsClient = new HttpClient(locationsHandler.Object) { BaseAddress = new Uri("http://localhost/") };
            var eventsClient = new HttpClient(eventsHandler.Object) { BaseAddress = new Uri("http://localhost/") };

            var factoryDevices = new Mock<IHttpClientFactory>();
            var factoryLocations = new Mock<IHttpClientFactory>();
            var factoryEvents = new Mock<IHttpClientFactory>();

            factoryDevices.Setup(f => f.CreateClient("DeviceService")).Returns(devicesClient);
            factoryLocations.Setup(f => f.CreateClient("LocationService")).Returns(locationsClient);
            factoryEvents.Setup(f => f.CreateClient("EventService")).Returns(eventsClient);

            return new DataFetchingService(factoryDevices.Object, factoryLocations.Object, factoryEvents.Object);
        }

        [Fact]
        public async Task GetDevices_ShouldReturnDevices_WhenApiResponseIsValid()
        {
            var service = CreateService(out var devicesHandler, out _, out _);

            var devices = new List<DeviceResponse>
        {
            new DeviceResponse { Id = 1, StatusId = 1 },
            new DeviceResponse { Id = 2, StatusId = 2 }
        };

            var apiResponse = new ApiResponse<List<DeviceResponse>> { Data = devices };

            devicesHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(apiResponse)
                });

            var result = await service.GetDevices();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetDevices_ShouldReturnNull_WhenApiCallFails()
        {
            var service = CreateService(out var devicesHandler, out _, out _);

            devicesHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var result = await service.GetDevices();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEventsOfDevice_ShouldReturnEvents_WhenApiResponseIsValid()
        {
            var service = CreateService(out _, out _, out var eventsHandler);

            var events = new List<EventResponse>
        {
            new EventResponse { Id = 1, Device = "SN1" },
            new EventResponse { Id = 2, Device = "SN1" }
        };

            var apiResponse = new ApiResponse<List<EventResponse>> { Data = events };

            eventsHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("api/event/device/SN1")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(apiResponse)
                });

            var result = await service.GetEventsOfDevice("SN1", "jwt-token");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEventsOfDevice_ShouldReturnNull_WhenApiCallFails()
        {
            var service = CreateService(out _, out _, out var eventsHandler);

            eventsHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Timeout"));

            var result = await service.GetEventsOfDevice("SN1", "jwt-token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetEventsOfDevice_ShouldAttachAuthorizationHeader_WhenJwtProvided()
        {
            HttpRequestMessage? capturedRequest = null;

            var service = CreateService(out _, out _, out var eventsHandler);

            var events = new List<EventResponse> { new EventResponse { Id = 1, Device = "SN1" } };
            var apiResponse = new ApiResponse<List<EventResponse>> { Data = events };

            eventsHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(apiResponse)
                });

            await service.GetEventsOfDevice("SN1", "jwt-token");

            Assert.NotNull(capturedRequest);
            Assert.NotNull(capturedRequest.Headers.Authorization);
            Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
            Assert.Equal("jwt-token", capturedRequest.Headers.Authorization.Parameter);
        }
    }
}
