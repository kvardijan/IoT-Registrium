using Microsoft.AspNetCore.Mvc;
using user_service.Models;

namespace user_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UsersDbContext _context;
        public UserController(UsersDbContext context)
        {
            _context = context;
        }
    }
}
