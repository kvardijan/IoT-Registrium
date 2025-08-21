namespace DeviceServiceTest
{
    using device_service.Services;
    using device_service.Models;
    using device_service.DTOs;
    using Xunit;
    using Moq;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    public class DeviceServiceTests
    {
        private DevicesDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DevicesDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new DevicesDbContext(options);

            context.Types.Add(new device_service.Models.Type { Id = 1, Description = "Sensor" });
            context.Statuses.Add(new Status { Id = 1, Description = "Active" });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task RegisterDevice_ShouldSaveAndCallEventService()
        {
            var context = GetDbContext("RegisterDeviceDb");
            var eventServiceMock = new Mock<EventCreationService>();

            var service = new DeviceService(context, eventServiceMock.Object);

            var dto = new DeviceRegistrationDto
            {
                SerialNumber = "SN123",
                Model = "X100",
                Manufacturer = "Acme",
                Type = 1,
                Status = 1,
                FirmwareVersion = "1.0.0",
                Location = 5
            };

            var result = await service.RegisterDevice(dto, "fake-jwt");

            Assert.NotNull(result);
            Assert.Equal("SN123", result.SerialNumber);
            Assert.Equal("Sensor", result.Type);
            Assert.Equal("Active", result.Status);

            Assert.Single(context.Devices);

            eventServiceMock.Verify(
                es => es.CreateDeviceAddedEventAsync(It.IsAny<Device>(), "fake-jwt"),
                Times.Once
            );
        }
    }
}