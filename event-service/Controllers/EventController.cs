using event_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace event_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private readonly EventsDbContext _context;
        public EventController(EventsDbContext context)
        {
            _context = context;
        }
    }
}
