using device_service.DTOs;
using device_service.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace device_service.Services
{
    public class DeviceService
    {
        private readonly DevicesDbContext _context;
        private readonly EventCreationService _eventCreationService;

        public DeviceService(DevicesDbContext context, EventCreationService eventCreationService)
        {
            _context = context;
            _eventCreationService = eventCreationService;
        }

        public async Task<DeviceResponse?> RegisterDevice(DeviceRegistrationDto deviceRegistrationDto, string jwtToken)
        {
            Device newDevice = MapDevice(deviceRegistrationDto);
            _context.Devices.Add(newDevice);
            _context.SaveChanges();

            var type = _context.Types.FirstOrDefault(t => t.Id == newDevice.Type);
            if (type == null)
            {
                throw new Exception("Invalid Type Id.");
            }

            var status = _context.Statuses.FirstOrDefault(s => s.Id == newDevice.Status);
            if (status == null)
            {
                throw new Exception("Invalid Status Id.");
            }

            await _eventCreationService.CreateDeviceAddedEventAsync(newDevice, jwtToken);

            return new DeviceResponse
            {
                Id = newDevice.Id,
                SerialNumber = newDevice.SerialNumber,
                Model = newDevice.Model,
                Manufacturer = newDevice.Manufacturer,
                TypeId = newDevice.Type,
                Type = type.Description,
                StatusId = newDevice.Status,
                Status = status.Description,
                FirmwareVersion = newDevice.FirmwareVersion,
                Location = newDevice.Location,
                LastSeen = newDevice.LastSeen
            };
        }

        public DeviceResponse? GetDeviceById(int id)
        {
            var device = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .FirstOrDefault(d => d.Id == id);

            if (device == null)
            {
                return null;
            }

            return MapDeviceResponse(device);
        }

        public DeviceResponse? GetDeviceBySerialNumber(string serialNumber)
        {
            var device = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .FirstOrDefault(d => d.SerialNumber == serialNumber);

            if (device == null)
            {
                return null;
            }

            return MapDeviceResponse(device);
        }

        public List<DeviceResponse> GetDevices()
        {
            return _context.Devices.Select(d => new DeviceResponse
            {
                Id = d.Id,
                SerialNumber = d.SerialNumber,
                Model = d.Model,
                Manufacturer = d.Manufacturer,
                TypeId = d.Type,
                Type = d.TypeNavigation.Description,
                StatusId = d.Status,
                Status = d.StatusNavigation.Description,
                FirmwareVersion = d.FirmwareVersion,
                Location = d.Location,
                LastSeen = d.LastSeen
            }).ToList();
        }

        public async Task<DeviceResponse?> UpdateDevice(int id, DeviceUpdateDto deviceUpdateDto, string jwtToken)
        {
            var existingDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .FirstOrDefault(d => d.Id == id);

            if (existingDevice == null)
            {
                return null;
            }

            var oldDevice = new Device
            {
                Id = existingDevice.Id,
                SerialNumber = existingDevice.SerialNumber,
                Model = existingDevice.Model,
                Manufacturer = existingDevice.Manufacturer,
                Type = existingDevice.Type,
                Status = existingDevice.Status,
                FirmwareVersion = existingDevice.FirmwareVersion,
                Location = existingDevice.Location,
                LastSeen = existingDevice.LastSeen,
                StatusNavigation = existingDevice.StatusNavigation,
                TypeNavigation = existingDevice.TypeNavigation
            };

            if (!string.IsNullOrEmpty(deviceUpdateDto.Model))
                existingDevice.Model = deviceUpdateDto.Model;

            if (!string.IsNullOrEmpty(deviceUpdateDto.Manufacturer))
                existingDevice.Manufacturer = deviceUpdateDto.Manufacturer;

            if (deviceUpdateDto.Type.HasValue)
                existingDevice.Type = deviceUpdateDto.Type.Value;

            if (deviceUpdateDto.Status.HasValue)
                existingDevice.Status = deviceUpdateDto.Status.Value;

            if (!string.IsNullOrEmpty(deviceUpdateDto.FirmwareVersion))
                existingDevice.FirmwareVersion = deviceUpdateDto.FirmwareVersion;

            if (deviceUpdateDto.Location.HasValue)
                existingDevice.Location = deviceUpdateDto.Location.Value;

            existingDevice.LastSeen = DateTime.UtcNow;

            _context.SaveChanges();

            var updatedDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .First(d => d.Id == id);

            await _eventCreationService.CreateDeviceInfoUpdatedEventAsync(oldDevice, updatedDevice, jwtToken);

            return MapDeviceResponse(updatedDevice);
        }

        public async Task<DeviceResponse?> UpdateDeviceStatus(int id, DeviceUpdateDto deviceUpdateDto, string jwtToken)
        {
            var existingDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .FirstOrDefault(d => d.Id == id);

            if (existingDevice == null)
            {
                return null;
            }

            string oldStatus = existingDevice.StatusNavigation.Description;

            if (deviceUpdateDto.Status.HasValue)
                existingDevice.Status = deviceUpdateDto.Status.Value;

            existingDevice.LastSeen = DateTime.UtcNow;

            _context.SaveChanges();

            var updatedDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .First(d => d.Id == id);

            await _eventCreationService.CreateDeviceStatusChangeEventAsync(
                updatedDevice.SerialNumber,
                oldStatus,
                updatedDevice.StatusNavigation.Description,
                jwtToken);

            return MapDeviceResponse(updatedDevice);
        }

        public async Task<DeviceResponse?> StoreDeviceData(int id, DeviceDataDto deviceDataDto, string jwtToken)
        {
            var existingDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .FirstOrDefault(d => d.Id == id);

            if (existingDevice == null)
            {
                return null;
            }

            existingDevice.LastSeen = DateTime.UtcNow;

            _context.SaveChanges();

            var updatedDevice = _context.Devices
                .Include(d => d.TypeNavigation)
                .Include(d => d.StatusNavigation)
                .First(d => d.Id == id);

            await _eventCreationService.CreateDeviceDataRecordingEventAsync(
                existingDevice.SerialNumber,
                deviceDataDto,
                jwtToken);

            return MapDeviceResponse(updatedDevice);
        }

        private Device MapDevice(DeviceRegistrationDto deviceRegistrationDto)
        {
            return new Device
            {
                SerialNumber = deviceRegistrationDto.SerialNumber,
                Model = deviceRegistrationDto.Model,
                Manufacturer = deviceRegistrationDto.Manufacturer,
                Type = deviceRegistrationDto.Type,
                Status = deviceRegistrationDto.Status,
                FirmwareVersion = deviceRegistrationDto.FirmwareVersion,
                Location = deviceRegistrationDto.Location,
                LastSeen = DateTime.UtcNow
            };
        }

        private DeviceResponse MapDeviceResponse(Device device)
        {
            return new DeviceResponse
            {
                Id = device.Id,
                SerialNumber = device.SerialNumber,
                Model = device.Model,
                Manufacturer = device.Manufacturer,
                TypeId = device.Type,
                Type = device.TypeNavigation.Description,
                StatusId = device.Status,
                Status = device.StatusNavigation.Description,
                FirmwareVersion = device.FirmwareVersion,
                Location = device.Location,
                LastSeen = device.LastSeen
            };
        }
    }
}
