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

        public List<Event> GetEvents()
        {
            return _context.Events.ToList();
        }

        public List<Event>? GetEventsOfDevice(string serialNumber)
        {
            return _context.Events.Where(e => e.Device == serialNumber).ToList();
        }
    }
}
