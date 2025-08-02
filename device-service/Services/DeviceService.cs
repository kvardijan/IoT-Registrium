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
            return _context.Devices.FirstOrDefault(d =>d.SerialNumber == serialNumber);
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
                LastSeen = DateTime.Now
            };
        }
    }
}
