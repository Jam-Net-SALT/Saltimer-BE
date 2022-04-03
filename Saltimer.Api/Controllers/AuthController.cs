using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
            : base(mapper, authService, context) { }

        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        public ActionResult<UserResponseDto> GetMe()
        {
            var currentUser = _authService.GetCurrentUser();
            var response = _mapper.Map<UserResponseDto>(currentUser);
            return Ok(response);
        }

        [HttpPut("user")]
        public async Task<IActionResult> PutUser(UpdateUserDto request)
        {
            var currentUser = _authService.GetCurrentUser();
            var targetUser = await _context.User.Where(u => u.Id == currentUser.Id).SingleOrDefaultAsync();

            if (_context.User.Any(e => e.Id != targetUser.Id && e.Username == request.Username))
                return BadRequest(new ErrorResponse()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Username is already taken."
                });

            if (_context.User.Any(e => e.Id != targetUser.Id && e.EmailAddress == request.EmailAddress))
                return BadRequest(new ErrorResponse()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email address is already taken."
                });

            _mapper.Map(request, targetUser);
            _context.Entry(targetUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("register"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            if (_context.User.Any(e => e.Username == request.Username))
                return BadRequest(new ErrorResponse()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Username is already taken."
                });

            if (_context.User.Any(e => e.EmailAddress == request.EmailAddress))
                return BadRequest(new ErrorResponse()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email address is already taken."
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

        [HttpPost("login"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var targetUser = await _context.User.SingleOrDefaultAsync(c => c.Username == request.Username);

            if (targetUser == null || !_authService.VerifyPasswordHash(request.Password, targetUser.PasswordHash, targetUser.PasswordSalt))
            {
                return Unauthorized();
            }

            string token = _authService.CreateToken(targetUser);
            return Ok(new LoginResponse() { Token = token });
        }

    }
}