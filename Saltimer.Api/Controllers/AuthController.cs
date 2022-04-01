
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, SaltimerDBContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _authService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            if (_context.User.Any(e => e.Username == request.Username))
                return await Task.FromResult<ActionResult<User>>(BadRequest("User already exists."));

            _authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.ProfileImage = request.ProfileImageUrl;
            user.EmailAddress = request.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return await Task.FromResult<ActionResult<User>>(Ok(user));
        }

        [HttpPost("login")]
        public Task<ActionResult<string>> Login(LoginDto request)
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

            if (!_authService.VerifyPasswordHash(request.Password, target_user.PasswordHash, target_user.PasswordSalt))
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Wrong password."));
            }

            string token = _authService.CreateToken(user);
            return Task.FromResult<ActionResult<string>>(Ok(new { token = token }));
        }

    }
}