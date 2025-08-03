using event_service.Models;
using event_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace event_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private readonly EventService _eventService;
        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }
    }
}
