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
            var eventServiceMock = new Mock<IEventCreationService>();

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

        [Fact]
        public async Task UpdateDevice_ShouldUpdateFieldsAndCallEventService()
        {
            var context = GetDbContext("UpdateDeviceDb");
            var eventServiceMock = new Mock<IEventCreationService>();

            var device = new Device
            {
                SerialNumber = "SN1",
                Model = "Old",
                Manufacturer = "Acme",
                Type = 1,
                Status = 1,
                FirmwareVersion = "0.9",
                Location = 1
            };
            context.Devices.Add(device);
            context.SaveChanges();

            var service = new DeviceService(context, eventServiceMock.Object);

            var dto = new DeviceUpdateDto
            {
                Model = "NewModel",
                FirmwareVersion = "1.0"
            };

            var result = await service.UpdateDevice(device.Id, dto, "jwt");

            Assert.NotNull(result);
            Assert.Equal("NewModel", result.Model);
            Assert.Equal("1.0", result.FirmwareVersion);

            eventServiceMock.Verify(
                es => es.CreateDeviceInfoUpdatedEventAsync(It.IsAny<Device>(), It.IsAny<Device>(), "jwt"),
                Times.Once
            );
        }
    }
}