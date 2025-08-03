using device_service.DTOs;
using device_service.Models;
using Microsoft.EntityFrameworkCore;

namespace device_service.Services
{
    public class DeviceService
    {
        private readonly DevicesDbContext _context;
        public DeviceService(DevicesDbContext context)
        {
            _context = context;
        }

        public Device? RegisterDevice(DeviceRegistrationDto deviceRegistrationDto)
        {
            Device newDevice = MapDevice(deviceRegistrationDto);
            _context.Devices.Add(newDevice);
            _context.SaveChanges();
            return newDevice;
        }

        public Device? GetDeviceById(int id)
        {
            return _context.Devices.FirstOrDefault(d => d.Id == id);
        }

        public Device? GetDeviceBySerialNumber(string serialNumber)
        {
            return _context.Devices.FirstOrDefault(d => d.SerialNumber == serialNumber);
        }

        public List<Device> GetDevices()
        {
            return _context.Devices.ToList();
        }

        public Device? UpdateDevice(int id, DeviceUpdateDto deviceUpdateDto)
        {
            var existingDevice = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (existingDevice == null)
            {
                return null;
            }

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
            return existingDevice;
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
    }
}
