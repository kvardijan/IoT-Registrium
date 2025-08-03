using event_service.DTOs;
using event_service.Models;
using Microsoft.EntityFrameworkCore;

namespace event_service.Services
{
    public class EventService
    {
        private readonly EventsDbContext _context;
        public EventService(EventsDbContext context)
        {
            _context = context;
        }

        public EventResponse? GetEventById(int id)
        {
            var evnt = _context.Events.Include(e => e.TypeNavigation).FirstOrDefault(e => e.Id == id);
            if (evnt == null)
            {
                return null;
            }
            return new EventResponse
            {
                Id = evnt.Id,
                Device = evnt.Device,
                TypeId = evnt.Type,
                Type = evnt.TypeNavigation.Description,
                Data = evnt.Data,
                Timestamp = evnt.Timestamp
            };
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
            return _context.Events
                .Where(e => e.Device == serialNumber)
                .Select(e => new EventResponse
            {
                Id = e.Id,
                Device = e.Device,
                TypeId = e.Type,
                Type = e.TypeNavigation.Description,
                Data = e.Data,
                Timestamp = e.Timestamp
            }).ToList();
        }

        public EventResponse CreateEvent(EventCreationDto eventCreationDto)
        {
            var newEvent = MapEvent(eventCreationDto);
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            var type = _context.Types.FirstOrDefault(t => t.Id == newEvent.Type);
            if (type == null)
            {
                throw new Exception("Invalid Type Id.");
            }

            return new EventResponse
            {
                Id = newEvent.Id,
                Device = newEvent.Device,
                TypeId = newEvent.Type,
                Type = type.Description,
                Data = newEvent.Data,
                Timestamp = newEvent.Timestamp
            };
        }

        private Event MapEvent(EventCreationDto eventCreationDto)
        {
            return new Event
            {
                Device = eventCreationDto.Device,
                Type = eventCreationDto.Type,
                Data = eventCreationDto.Data,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
