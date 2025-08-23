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
    private HttpClient CreateMockHttpClient(HttpResponseMessage response, out Mock<HttpMessageHandler> handlerMock)
    {
        handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };
    }

    private EventCreationService CreateService(HttpResponseMessage response, out Mock<HttpMessageHandler> handlerMock)
    {
        var httpClient = CreateMockHttpClient(response, out handlerMock);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("EventService"))
                         .Returns(httpClient);

        return new EventCreationService(httpClientFactory.Object);
    }

    [Fact]
    public async Task CreateDeviceAddedEventAsync_Should_PostCorrectPayload_AndSucceed()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        var device = new Device { Id = 1, SerialNumber = "SN123" };

        await service.CreateDeviceAddedEventAsync(device, "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceAdded(req, "SN123")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceAdded(HttpRequestMessage req, string serial)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 6 &&
               doc.RootElement.GetProperty("device").GetString() == serial;
    }

    [Fact]
    public async Task CreateDeviceAddedEventAsync_ShouldThrow_OnFailure()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);
        var device = new Device { SerialNumber = "SN123" };

        await Assert.ThrowsAsync<Exception>(() => service.CreateDeviceAddedEventAsync(device, "jwt-token"));
    }

    [Fact]
    public async Task CreateDeviceInfoUpdatedEventAsync_Should_PostCorrectPayload()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        var oldDevice = new Device { SerialNumber = "SN123", Model = "OldModel" };
        var newDevice = new Device { SerialNumber = "SN123", Model = "NewModel" };

        await service.CreateDeviceInfoUpdatedEventAsync(oldDevice, newDevice, "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceInfoUpdated(req, "OldModel", "NewModel")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceInfoUpdated(HttpRequestMessage req, string oldModel, string newModel)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        var data = dataDoc.RootElement;

        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 1 &&
               data.GetProperty("old").GetProperty("Model").GetString() == oldModel &&
               data.GetProperty("new").GetProperty("Model").GetString() == newModel;
    }

    [Fact]
    public async Task CreateDeviceStatusChangeEventAsync_Should_PostCorrectStatusChange()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        await service.CreateDeviceStatusChangeEventAsync("SN999", "offline", "online", "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceStatusChange(req, "offline", "online")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceStatusChange(HttpRequestMessage req, string oldStatus, string newStatus)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        var data = dataDoc.RootElement;

        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 2 &&
               data.GetProperty("oldStatus").GetString() == oldStatus &&
               data.GetProperty("newStatus").GetString() == newStatus;
    }

    [Fact]
    public async Task CreateDeviceDataRecordingEventAsync_Should_PostCorrectData()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        var data = new DeviceDataDto { RecordedData = "temperature:25" };
        await service.CreateDeviceDataRecordingEventAsync("SN111", data, "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceData(req, "temperature:25")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceData(HttpRequestMessage req, string recordedData)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        var data = dataDoc.RootElement;

        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 5 &&
               data.GetProperty("RecordedData").GetString() == recordedData;
    }

    [Fact]
    public async Task CreateDeviceSentCommandEventAsync_Should_PostCorrectCommand()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        var command = new DeviceCommandDto { Command = "reboot" };
        await service.CreateDeviceSentCommandEventAsync("SN777", command, "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceCommand(req, "reboot")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceCommand(HttpRequestMessage req, string command)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        var data = dataDoc.RootElement;

        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 3 &&
               data.GetProperty("Command").GetString() == command;
    }

    [Fact]
    public async Task CreateDeviceFirmwareChangeEventAsync_Should_PostCorrectFirmwareChange()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), out var handlerMock);

        await service.CreateDeviceFirmwareChangeEventAsync("SN555", "1.0", "2.0", "jwt-token");

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => VerifyDeviceFirmware(req, "1.0", "2.0")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static bool VerifyDeviceFirmware(HttpRequestMessage req, string oldVersion, string newVersion)
    {
        var json = req.Content!.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(json);
        var dataString = doc.RootElement.GetProperty("data").GetString();
        using var dataDoc = JsonDocument.Parse(dataString!);
        var data = dataDoc.RootElement;

        return req.Method == HttpMethod.Post &&
               doc.RootElement.GetProperty("type").GetInt32() == 4 &&
               data.GetProperty("oldFirmwareVersion").GetString() == oldVersion &&
               data.GetProperty("newFirmwareVersion").GetString() == newVersion;
    }
}
