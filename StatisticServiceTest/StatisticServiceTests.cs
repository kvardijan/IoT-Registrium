using Moq;
using statistic_service.DTOs;
using statistic_service.Services;
using Xunit;
using System.Text.Json;

namespace StatisticServiceTests
{
    public class StatisticServiceTests
    {
        private readonly Mock<IDataFetchingService> _mockDataFetchingService;
        private readonly StatisticService _service;

        public StatisticServiceTests()
        {
            _mockDataFetchingService = new Mock<IDataFetchingService>();
            _service = new StatisticService(_mockDataFetchingService.Object);
        }

        [Fact]
        public async Task GetDeviceStatusStatistic_ShouldReturnCounts_WhenDevicesExist()
        {
            _mockDataFetchingService.Setup(s => s.GetDevices()).ReturnsAsync(new List<DeviceResponse>
            {
                new DeviceResponse { StatusId = 1 },
                new DeviceResponse { StatusId = 1 },
                new DeviceResponse { StatusId = 2 },
                new DeviceResponse { StatusId = 3 },
                new DeviceResponse { StatusId = 4 }
            });

            var result = await _service.GetDeviceStatusStatistic();

            Assert.NotNull(result);
            Assert.Equal(2, result.Active);
            Assert.Equal(1, result.Idle);
            Assert.Equal(1, result.Deactivated);
            Assert.Equal(1, result.Error);
        }

        [Fact]
        public async Task GetDeviceStatusStatistic_ShouldReturnNull_WhenDevicesIsNull()
        {
            _mockDataFetchingService.Setup(s => s.GetDevices()).ReturnsAsync((List<DeviceResponse>?)null);

            var result = await _service.GetDeviceStatusStatistic();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTemperatureDeviceStatistic_ShouldReturnCorrectStats()
        {
            var events = new List<EventResponse>
            {
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Temperature = "10C" } }), Timestamp = DateTime.UtcNow },
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Temperature = "20C" } }), Timestamp = DateTime.UtcNow },
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Temperature = "30C" } }), Timestamp = DateTime.UtcNow }
            };

            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync(events);

            var result = await _service.GetTemperatureDeviceStatistic("SN1", "token");

            Assert.NotNull(result);
            Assert.Equal(30, result.Max);
            Assert.Equal(10, result.Min);
            Assert.Equal(20, result.Avg);
        }

        [Fact]
        public async Task GetTemperatureDeviceStatistic_ShouldReturnNull_WhenEventsNull()
        {
            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN1", "token"))
                .ReturnsAsync((List<EventResponse>?)null);

            var result = await _service.GetTemperatureDeviceStatistic("SN1", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetHumidityDeviceStatistic_ShouldReturnCorrectStats()
        {
            var events = new List<EventResponse>
            {
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Humidity = "30%" } }), Timestamp = DateTime.UtcNow },
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Humidity = "40%" } }), Timestamp = DateTime.UtcNow },
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Humidity = "50%" } }), Timestamp = DateTime.UtcNow }
            };

            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN2", "token")).ReturnsAsync(events);

            var result = await _service.GetHumidityDeviceStatistic("SN2", "token");

            Assert.NotNull(result);
            Assert.Equal(50, result.Max);
            Assert.Equal(30, result.Min);
            Assert.Equal(40, result.Avg);
        }

        [Fact]
        public async Task GetHumidityDeviceStatistic_ShouldReturnNull_WhenEventsNull()
        {
            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN2", "token"))
                .ReturnsAsync((List<EventResponse>?)null);

            var result = await _service.GetHumidityDeviceStatistic("SN2", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSmartBinDeviceStatistic_ShouldReturnLatestCapacity()
        {
            var events = new List<EventResponse>
            {
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Capacity = "40%" } }), Timestamp = DateTime.UtcNow.AddMinutes(-10) },
                new EventResponse { Data = JsonSerializer.Serialize(new EventDataWrapper { RecordedData = new RecordedData { Capacity = "70%" } }), Timestamp = DateTime.UtcNow }
            };

            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN3", "token")).ReturnsAsync(events);

            var result = await _service.GetSmartBinDeviceStatistic("SN3", "token");

            Assert.NotNull(result);
            Assert.Equal(70, result.PercentageFull);
        }

        [Fact]
        public async Task GetSmartBinDeviceStatistic_ShouldReturnNull_WhenEventsNull()
        {
            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN3", "token"))
                .ReturnsAsync((List<EventResponse>?)null);

            var result = await _service.GetSmartBinDeviceStatistic("SN3", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSmartBinDeviceStatistic_ShouldIgnoreInvalidJson()
        {
            var events = new List<EventResponse>
            {
                new EventResponse { Data = "invalid-json", Timestamp = DateTime.UtcNow }
            };

            _mockDataFetchingService.Setup(s => s.GetEventsOfDevice("SN3", "token")).ReturnsAsync(events);

            var result = await _service.GetSmartBinDeviceStatistic("SN3", "token");

            Assert.NotNull(result);
            Assert.Equal(0, result.PercentageFull);
        }

        [Fact]
        public async Task GetDeviceStatusStatistic_ShouldIgnoreUnknownStatusIds()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetDevices()).ReturnsAsync(new List<DeviceResponse>
            {
                new DeviceResponse { StatusId = 99 }
            });

            var service = new StatisticService(mock.Object);

            var result = await service.GetDeviceStatusStatistic();

            Assert.Equal(0, result.Active);
            Assert.Equal(0, result.Idle);
            Assert.Equal(0, result.Deactivated);
            Assert.Equal(0, result.Error);
        }

        [Fact]
        public async Task GetTemperatureDeviceStatistic_ShouldReturnNull_WhenEventsIsNull()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync((List<EventResponse>?)null);

            var service = new StatisticService(mock.Object);

            var result = await service.GetTemperatureDeviceStatistic("SN1", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTemperatureDeviceStatistic_ShouldHandleInvalidJson()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync(new List<EventResponse>
            {
                new EventResponse { Data = "{not-valid-json" }
            });

            var service = new StatisticService(mock.Object);

            var result = await service.GetTemperatureDeviceStatistic("SN1", "token");

            Assert.Equal(float.MinValue, result.Max);
            Assert.Equal(float.MaxValue, result.Min);
            Assert.Equal(0, result.Avg);
        }

        [Fact]
        public async Task GetHumidityDeviceStatistic_ShouldReturnNull_WhenEventsIsNull()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync((List<EventResponse>?)null);

            var service = new StatisticService(mock.Object);

            var result = await service.GetHumidityDeviceStatistic("SN1", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetHumidityDeviceStatistic_ShouldHandleInvalidJson()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync(new List<EventResponse>
            {
                new EventResponse { Data = "{invalid}" }
            });

            var service = new StatisticService(mock.Object);

            var result = await service.GetHumidityDeviceStatistic("SN1", "token");

            Assert.Equal(float.MinValue, result.Max);
            Assert.Equal(float.MaxValue, result.Min);
            Assert.Equal(0, result.Avg);
        }

        [Fact]
        public async Task GetSmartBinDeviceStatistic_ShouldReturnNull_WhenEventsIsNull()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync((List<EventResponse>?)null);

            var service = new StatisticService(mock.Object);

            var result = await service.GetSmartBinDeviceStatistic("SN1", "token");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSmartBinDeviceStatistic_ShouldHandleInvalidJson()
        {
            var mock = new Mock<IDataFetchingService>();
            mock.Setup(s => s.GetEventsOfDevice("SN1", "token")).ReturnsAsync(new List<EventResponse>
            {
                new EventResponse { Data = "{not-json}" }
            });

            var service = new StatisticService(mock.Object);

            var result = await service.GetSmartBinDeviceStatistic("SN1", "token");

            Assert.Equal(0, result.PercentageFull);
        }

    }
}
