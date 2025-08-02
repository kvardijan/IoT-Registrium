using user_service.Models;

namespace user_service.Services
{
    public class UserService
    {
        private readonly UsersDbContext _context;
        public UserService(UsersDbContext context)
        {
            _context = context;
        }

        public User? Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null) return null;

            //workaround for admin user
            if (user.Username.Equals("admin"))
            {
                if (user.Password.Equals(password))
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            //workaround for admin user

            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!verified) return null;

            return user;
        }
    }
}
