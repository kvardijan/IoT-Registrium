using event_service.DTOs;
using event_service.Models;

namespace event_service.Services
{
    public class EventService
    {
        private readonly EventsDbContext _context;
        public EventService(EventsDbContext context)
        {
            _context = context;
        }

        public List<EventResponse> GetEvents()
        {
            return _context.Events.Select(e => new EventResponse
            {
                Id = e.Id,
                Device = e.Device,
                TypeId = e.Type,
                Type = e.TypeNavigation.Description,
                Data = e.Data,
                Timestamp = e.Timestamp
            }).ToList();
        }

        public List<EventResponse>? GetEventsOfDevice(string serialNumber)
        {
            return _context.Events.Where(e => e.Device == serialNumber).Select(e => new EventResponse
            {
                Id = e.Id,
                Device = e.Device,
                TypeId = e.Type,
                Type = e.TypeNavigation.Description,
                Data = e.Data,
                Timestamp = e.Timestamp
            }).ToList();
        }
    }
}
