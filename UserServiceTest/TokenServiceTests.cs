using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using user_service.Models;
using user_service.Services;
using Xunit;

namespace user_service.Tests
{
    public class TokenServiceTests
    {
        private IConfiguration GetConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "jakotajnikljuc123TEST_evorandomslova:94yf57893yn78g5ng7eydy"},
                {"Jwt:Issuer", "iot-registry-api"},
                {"Jwt:Audience", "iot-registry-frontend"},
                {"Jwt:ExpiresInMinutes", "60"}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private TokenValidationParameters GetValidationParameters(IConfiguration config)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = config["Jwt:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        [Fact]
        public void CreateToken_ShouldReturnTokenString()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 1, Username = "testuser" };
            var tokenString = tokenService.CreateToken(user);

            Assert.False(string.IsNullOrEmpty(tokenString));
        }

        [Fact]
        public void CreateToken_ShouldContainCorrectClaims()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 42, Username = "alice" };
            var tokenString = tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            var subClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);

            Assert.NotNull(subClaim);
            Assert.NotNull(nameClaim);
            Assert.Equal("42", subClaim!.Value);
            Assert.Equal("alice", nameClaim!.Value);
        }

        [Fact]
        public void CreateToken_ShouldHaveCorrectIssuerAndAudience()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 1, Username = "testuser" };
            var tokenString = tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal("iot-registry-api", token.Issuer);
            Assert.Equal("iot-registry-frontend", token.Audiences.First());
        }

        [Fact]
        public void CreateToken_ShouldHaveFutureExpiration()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 1, Username = "testuser" };
            var tokenString = tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.True(token.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_ShouldValidateSignature()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 1, Username = "testuser" };
            var tokenString = tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(config);

            handler.ValidateToken(tokenString, validationParameters, out var validatedToken);

            Assert.NotNull(validatedToken);
            Assert.IsType<JwtSecurityToken>(validatedToken);
        }

        [Fact]
        public void CreateToken_ShouldContainCorrectClaimsAndValidateSignature()
        {
            var config = GetConfiguration();
            var tokenService = new TokenService(config);

            var user = new User { Id = 42, Username = "alice" };
            var tokenString = tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(config);

            var principal = handler.ValidateToken(tokenString, validationParameters, out var validatedToken);

            Assert.NotNull(validatedToken);
            Assert.IsType<JwtSecurityToken>(validatedToken);

            // mapping from pure jwt clim to .net claim puts .sub into .nameidentifier !!!
            var subClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var nameClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);

            Assert.NotNull(subClaim);
            Assert.Equal("42", subClaim!);

            Assert.NotNull(nameClaim);
            Assert.Equal("alice", nameClaim!.Value);
        }
    }
}
