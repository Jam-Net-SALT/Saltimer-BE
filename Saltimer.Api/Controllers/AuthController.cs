using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SaltimerDBContext _context;
        private static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService, SaltimerDBContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(SignupUserDto request)
        {
            if (_context.User.Any(e => e.Username == request.Username))
                return await Task.FromResult<ActionResult<User>>(BadRequest("User already exists."));

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.ProfileImage = request.Url;
            user.EmailAddress = request.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return await Task.FromResult<ActionResult<User>>(Ok(user));
        }

        [HttpPost("login")]
        public Task<ActionResult<string>> Login(LoginUserDto request)
        {
            var target_user = _context.User.FirstOrDefault(c => c.Username == request.Username);
            // if (user.Username != request.Username)
            // {
            //     return Task.FromResult<ActionResult<string>>(BadRequest("User not found."));
            // }
            if (target_user == null)
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("User not found."));
            }

            if (!VerifyPasswordHash(request.Password, target_user.PasswordHash, target_user.PasswordSalt))
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Wrong password."));
            }

            string token = CreateToken(user);
            return Task.FromResult<ActionResult<string>>(Ok(new { token = token }));
        }

        private string CreateToken(User modelUser)
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

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}