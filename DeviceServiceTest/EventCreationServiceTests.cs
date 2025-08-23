using device_service.DTOs;
using device_service.Models;
using device_service.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class EventCreationServiceTests
{
    private EventCreationService CreateService(out Mock<HttpMessageHandler> handlerMock, HttpResponseMessage? response = null)
    {
        handlerMock = new Mock<HttpMessageHandler>();

        response ??= new HttpResponseMessage(HttpStatusCode.OK);

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost/") };

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("EventService")).Returns(httpClient);

        return new EventCreationService(httpClientFactory.Object);
    }

    private void VerifyEvent(HttpRequestMessage req, int expectedType, string expectedDevice, Action<JsonElement>? verifyData = null)
    {
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.EndsWith("api/event", req.RequestUri!.ToString());
        using var doc = JsonDocument.Parse(req.Content!.ReadAsStringAsync().Result);
        Assert.Equal(expectedType, doc.RootElement.GetProperty("type").GetInt32());
        Assert.Equal(expectedDevice, doc.RootElement.GetProperty("device").GetString());

        if (verifyData != null)
        {
            var dataString = doc.RootElement.GetProperty("data").GetString();
            using var dataDoc = JsonDocument.Parse(dataString!);
            verifyData(dataDoc.RootElement);
        }
    }

    [Fact]
    public async Task CreateDeviceAddedEventAsync_Should_PostCorrectPayload()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var device = new Device { Id = 1, SerialNumber = "SN123" };

        await service.CreateDeviceAddedEventAsync(device, "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 6, "SN123", data =>
        {
            Assert.Equal("SN123", data.GetProperty("SerialNumber").GetString());
        });
    }

    [Fact]
    public async Task CreateDeviceAddedEventAsync_ShouldThrow_OnFailure()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost/") };
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("EventService")).Returns(httpClient);

        var service = new EventCreationService(httpClientFactory.Object);

        var device = new Device { SerialNumber = "SN123" };

        await Assert.ThrowsAsync<Exception>(() => service.CreateDeviceAddedEventAsync(device, "jwt-token"));
    }


    [Fact]
    public async Task CreateDeviceInfoUpdatedEventAsync_Should_PostCorrectPayload()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var oldDevice = new Device { SerialNumber = "SN123", Model = "OldModel" };
        var newDevice = new Device { SerialNumber = "SN123", Model = "NewModel" };

        await service.CreateDeviceInfoUpdatedEventAsync(oldDevice, newDevice, "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 1, "SN123", data =>
        {
            Assert.Equal("OldModel", data.GetProperty("old").GetProperty("Model").GetString());
            Assert.Equal("NewModel", data.GetProperty("new").GetProperty("Model").GetString());
        });
    }

    [Fact]
    public async Task CreateDeviceStatusChangeEventAsync_Should_PostCorrectStatusChange()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await service.CreateDeviceStatusChangeEventAsync("SN999", "offline", "online", "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 2, "SN999", data =>
        {
            Assert.Equal("offline", data.GetProperty("oldStatus").GetString());
            Assert.Equal("online", data.GetProperty("newStatus").GetString());
        });
    }

    [Fact]
    public async Task CreateDeviceDataRecordingEventAsync_Should_PostCorrectData()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var dataDto = new DeviceDataDto { RecordedData = "temperature:25" };
        await service.CreateDeviceDataRecordingEventAsync("SN111", dataDto, "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 5, "SN111", data =>
        {
            Assert.Equal("temperature:25", data.GetProperty("RecordedData").GetString());
        });
    }

    [Fact]
    public async Task CreateDeviceSentCommandEventAsync_Should_PostCorrectCommand()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var cmd = new DeviceCommandDto { Command = "reboot" };
        await service.CreateDeviceSentCommandEventAsync("SN777", cmd, "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 3, "SN777", data =>
        {
            Assert.Equal("reboot", data.GetProperty("Command").GetString());
        });
    }

    [Fact]
    public async Task CreateDeviceFirmwareChangeEventAsync_Should_PostCorrectFirmwareChange()
    {
        var service = CreateService(out var handlerMock);
        HttpRequestMessage? sentRequest = null;

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => sentRequest = req)
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        await service.CreateDeviceFirmwareChangeEventAsync("SN555", "1.0", "2.0", "jwt-token");

        Assert.NotNull(sentRequest);
        VerifyEvent(sentRequest!, 4, "SN555", data =>
        {
            Assert.Equal("1.0", data.GetProperty("oldFirmwareVersion").GetString());
            Assert.Equal("2.0", data.GetProperty("newFirmwareVersion").GetString());
        });
    }
}
