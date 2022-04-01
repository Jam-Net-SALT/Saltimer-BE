using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Saltimer.Api.Models;

namespace Saltimer.Api.Services
{
    public class AuthService : IAuthService
    {
        public readonly IHttpContextAccessor _httpContextAccessor;

        public readonly IConfiguration _configuration;

        public AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }
        public string CreateToken(User modelUser)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, modelUser.Username),
                new Claim(ClaimTypes.Uri, modelUser.ProfileImage),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
