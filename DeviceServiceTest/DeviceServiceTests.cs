using device_service.DTOs;
using device_service.Models;
using device_service.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DeviceServiceTests
{
    private DevicesDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DevicesDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new DevicesDbContext(options);
    }

    [Fact]
    public async Task RegisterDevice_ShouldSaveAndCallEventService()
    {
        var context = GetDbContext(nameof(RegisterDevice_ShouldSaveAndCallEventService));
        context.Types.Add(new device_service.Models.Type { Id = 1, Description = "Sensor" });
        context.Statuses.Add(new Status { Id = 1, Description = "Active" });
        context.SaveChanges();

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

        var devices = context.Devices.ToList();
        Assert.Equal(1, devices.Count);
        Assert.Equal("SN123", devices[0].SerialNumber);

        Assert.NotNull(result);
        Assert.Equal("SN123", result.SerialNumber);

        eventServiceMock.Verify(
            es => es.CreateDeviceAddedEventAsync(It.IsAny<Device>(), "fake-jwt"),
            Times.Once
        );
    }

    [Fact]
    public void GetDeviceById_ShouldReturnDevice_WhenExists()
    {
        var context = GetDbContext(nameof(GetDeviceById_ShouldReturnDevice_WhenExists));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN001",
            Model = "M1",
            Manufacturer = "TestCo",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 1,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var service = new DeviceService(context, Mock.Of<IEventCreationService>());
        var result = service.GetDeviceById(device.Id);

        Assert.NotNull(result);
        Assert.Equal("SN001", result.SerialNumber);
    }

    [Fact]
    public void GetDeviceBySerialNumber_ShouldReturnDevice_WhenExists()
    {
        var context = GetDbContext(nameof(GetDeviceBySerialNumber_ShouldReturnDevice_WhenExists));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN002",
            Model = "M2",
            Manufacturer = "TestCo",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 2,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var service = new DeviceService(context, Mock.Of<IEventCreationService>());
        var result = service.GetDeviceBySerialNumber("SN002");

        Assert.NotNull(result);
        Assert.Equal("SN002", result.SerialNumber);
    }

    [Fact]
    public async Task UpdateDevice_ShouldModifyAndCallEventService()
    {
        var context = GetDbContext(nameof(UpdateDevice_ShouldModifyAndCallEventService));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN003",
            Model = "Old",
            Manufacturer = "OldCo",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 3,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var eventServiceMock = new Mock<IEventCreationService>();
        var service = new DeviceService(context, eventServiceMock.Object);

        var dto = new DeviceUpdateDto { Model = "NewModel" };

        var result = await service.UpdateDevice(device.Id, dto, "jwt");

        Assert.NotNull(result);
        Assert.Equal("NewModel", result.Model);

        eventServiceMock.Verify(
            es => es.CreateDeviceInfoUpdatedEventAsync(It.IsAny<Device>(), It.IsAny<Device>(), "jwt"),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateDeviceStatus_ShouldChangeStatusAndCallEventService()
    {
        var context = GetDbContext(nameof(UpdateDeviceStatus_ShouldChangeStatusAndCallEventService));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status1 = new Status { Id = 1, Description = "Active" };
        var status2 = new Status { Id = 2, Description = "Inactive" };
        context.Types.Add(type);
        context.Statuses.AddRange(status1, status2);

        var device = new Device
        {
            SerialNumber = "SN004",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status1.Id,
            FirmwareVersion = "v1",
            Location = 4,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status1
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var eventServiceMock = new Mock<IEventCreationService>();
        var service = new DeviceService(context, eventServiceMock.Object);

        var dto = new DeviceUpdateDto { Status = 2 };

        var result = await service.UpdateDeviceStatus(device.Id, dto, "jwt");

        Assert.NotNull(result);
        Assert.Equal("Inactive", result.Status);

        eventServiceMock.Verify(
            es => es.CreateDeviceStatusChangeEventAsync("SN004", "Active", "Inactive", "jwt"),
            Times.Once
        );
    }

    [Fact]
    public async Task StoreDeviceData_ShouldCallEventService()
    {
        var context = GetDbContext(nameof(StoreDeviceData_ShouldCallEventService));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN005",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 5,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var eventServiceMock = new Mock<IEventCreationService>();
        var service = new DeviceService(context, eventServiceMock.Object);

        var dto = new DeviceDataDto { RecordedData = "some data" };

        var result = await service.StoreDeviceData(device.Id, dto, "jwt");

        Assert.NotNull(result);

        eventServiceMock.Verify(
            es => es.CreateDeviceDataRecordingEventAsync("SN005", dto, "jwt"),
            Times.Once
        );
    }

    [Fact]
    public async Task SendCommandToDevice_ShouldCallEventService()
    {
        var context = GetDbContext(nameof(SendCommandToDevice_ShouldCallEventService));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN006",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 6,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var eventServiceMock = new Mock<IEventCreationService>();
        var service = new DeviceService(context, eventServiceMock.Object);

        var dto = new DeviceCommandDto { Command = "reboot" };

        var result = await service.SendCommandToDevice(device.Id, dto, "jwt");

        Assert.NotNull(result);

        eventServiceMock.Verify(
            es => es.CreateDeviceSentCommandEventAsync("SN006", dto, "jwt"),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateDeviceFirmware_ShouldChangeFirmwareAndCallEventService()
    {
        var context = GetDbContext(nameof(UpdateDeviceFirmware_ShouldChangeFirmwareAndCallEventService));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN007",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 7,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var eventServiceMock = new Mock<IEventCreationService>();
        var service = new DeviceService(context, eventServiceMock.Object);

        var dto = new DeviceUpdateDto { FirmwareVersion = "v2" };

        var result = await service.UpdateDeviceFirmware(device.Id, dto, "jwt");

        Assert.NotNull(result);
        Assert.Equal("v2", result.FirmwareVersion);

        eventServiceMock.Verify(
            es => es.CreateDeviceFirmwareChangeEventAsync("SN007", "v1", "v2", "jwt"),
            Times.Once
        );
    }

    [Fact]
    public void GetDevices_ShouldReturnList()
    {
        var context = GetDbContext(nameof(GetDevices_ShouldReturnList));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN007",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 7,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();

        var service = new DeviceService(context, Mock.Of<IEventCreationService>());

        var result = service.GetDevices();

        Assert.Single(result);
    }

    [Fact]
    public void GetStatuses_ShouldReturnSeeded()
    {
        var context = GetDbContext(nameof(GetStatuses_ShouldReturnSeeded));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN007",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 7,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();
        var service = new DeviceService(context, Mock.Of<IEventCreationService>());

        var statuses = service.GetStatuses();

        Assert.Single(statuses);
        Assert.Equal("Active", statuses[0].Description);
    }

    [Fact]
    public void GetTypes_ShouldReturnSeeded()
    {
        var context = GetDbContext(nameof(GetTypes_ShouldReturnSeeded));
        var type = new device_service.Models.Type { Id = 1, Description = "Sensor" };
        var status = new Status { Id = 1, Description = "Active" };
        context.Types.Add(type);
        context.Statuses.Add(status);

        var device = new Device
        {
            SerialNumber = "SN007",
            Model = "M",
            Manufacturer = "Co",
            Type = type.Id,
            Status = status.Id,
            FirmwareVersion = "v1",
            Location = 7,
            LastSeen = DateTime.UtcNow,
            TypeNavigation = type,
            StatusNavigation = status
        };
        context.Devices.Add(device);
        context.SaveChanges();
        var service = new DeviceService(context, Mock.Of<IEventCreationService>());

        var types = service.GetTypes();

        Assert.Single(types);
        Assert.Equal("Sensor", types[0].Description);
    }
}
