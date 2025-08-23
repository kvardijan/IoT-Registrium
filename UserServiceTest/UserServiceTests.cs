using user_service.Models;
using user_service.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace user_service.Tests
{
    public class UserServiceTests
    {
        private UsersDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new UsersDbContext(options);

            if (!context.Users.Any())
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
                context.Users.Add(new User
                {
                    Id = 1,
                    Username = "username",
                    Password = hashedPassword
                });

                context.Users.Add(new User
                {
                    Id = 2,
                    Username = "admin",
                    Password = "admin"
                });

                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public void Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
        {
            var context = GetDbContext("AuthValidDb");
            var service = new UserService(context);

            var user = service.Authenticate("username", "password123");

            Assert.NotNull(user);
            Assert.Equal("username", user.Username);
        }

        [Fact]
        public void Authenticate_ShouldReturnAdmin_WhenAdminCredentialsAreCorrect()
        {
            var context = GetDbContext("AuthAdminDb");
            var service = new UserService(context);

            var user = service.Authenticate("admin", "admin");

            Assert.NotNull(user);
            Assert.Equal("admin", user.Username);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            var context = GetDbContext("AuthWrongPasswordDb");
            var service = new UserService(context);

            var user = service.Authenticate("username", "kriva lozinka");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenAdminPasswordIsIncorrect()
        {
            var context = GetDbContext("AuthWrongAdminPasswordDb");
            var service = new UserService(context);

            var user = service.Authenticate("admin", "kriva loznika");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var context = GetDbContext("AuthNonExistentDb");
            var service = new UserService(context);

            var user = service.Authenticate("ne postojim", "sdahkfgsdhjkfsghjkd");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUsernameIsEmpty()
        {
            var context = GetDbContext("AuthEmptyUsernameDb");
            var service = new UserService(context);

            var user = service.Authenticate("", "password123");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenPasswordIsEmpty()
        {
            var context = GetDbContext("AuthEmptyPasswordDb");
            var service = new UserService(context);

            var user = service.Authenticate("username", "");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUsernameAndPasswordAreEmpty()
        {
            var context = GetDbContext("AuthEmptyBothDb");
            var service = new UserService(context);

            var user = service.Authenticate("", "");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUsernameIsNull()
        {
            var context = GetDbContext("AuthNullUsernameDb");
            var service = new UserService(context);

            var user = service.Authenticate(null!, "password123");

            Assert.Null(user);
        }
    }
}
