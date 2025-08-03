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
    }
}
