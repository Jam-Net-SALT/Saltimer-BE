
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SaltimerDBContext _context;
        private readonly IAuthService _authService;
        public readonly IMapper _mapper;

        public AuthController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
        {
            _mapper = mapper;
            _authService = authService;
            _context = context;
        }

        [HttpGet("user"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        public async Task<ActionResult> GetMe()
        {
            var currentUser = await _context.User.Where(u => u.Username.Equals(User.Identity.Name)).SingleOrDefaultAsync();
            var response = _mapper.Map<UserResponseDto>(currentUser);
            return Ok(response);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            if (_context.User.Any(e => e.Username == request.Username))
                return BadRequest(new ErrorResponse()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "User already exists."
                });

            _authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = _mapper.Map<User>(request);
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;

            newUser = _context.User.Add(newUser).Entity;
            await _context.SaveChangesAsync();

            var response = _mapper.Map<UserResponseDto>(newUser);

            return Ok(response);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var targetUser = await _context.User.SingleOrDefaultAsync(c => c.Username == request.Username);

            if (targetUser == null || !_authService.VerifyPasswordHash(request.Password, targetUser.PasswordHash, targetUser.PasswordSalt))
            {
                return Unauthorized();
            }

            string token = _authService.CreateToken(targetUser);
            return Ok(new LoginResponseDto() { Token = token });
        }

    }
}