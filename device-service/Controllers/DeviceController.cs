using device_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace device_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        private readonly DevicesDbContext _context;
        public DeviceController(DevicesDbContext context)
        {
            _context = context;
        }
    }
}
