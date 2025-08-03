using event_service.DTOs;
using event_service.Models;
using event_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace event_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _eventService.GetEvents();
            if (events == null)
            {
                return NotFound(ApiResponse<object>.Fail("No events found.", 404));
            }
            return Ok(ApiResponse<List<EventResponse>>.Ok(events));
        }

        [Authorize]
        [HttpGet("{serialNumber}")]
        public IActionResult GetEventsOfDevice(string serialNumber)
        {
            var events = _eventService.GetEventsOfDevice(serialNumber);
            if (events == null)
            {
                return NotFound(ApiResponse<object>.Fail("No events found for device.", 404));
            }
            return Ok(ApiResponse<List<EventResponse>>.Ok(events));
        }
    }
}
