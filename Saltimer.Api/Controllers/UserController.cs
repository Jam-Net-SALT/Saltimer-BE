#nullable disable
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SaltimerDBContext _context;
        public readonly IMapper _mapper;

        public UserController(IMapper mapper, SaltimerDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUser(string? filterTerm)
        {
            //return await _context.User.ToListAsync();
            return await _context.User
                .Where(u => filterTerm == null ? true :
                            u.Username.Contains(filterTerm) ||
                            u.EmailAddress.Contains(filterTerm))
                .Select(u => _mapper.Map<UserResponseDto>(u))
                .ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
