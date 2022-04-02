#nullable disable
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
    public class MobTimerSessionController : ControllerBase
    {
        private readonly SaltimerDBContext _context;
        public readonly IMapper _mapper;

        public MobTimerSessionController(IMapper mapper, SaltimerDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/MobTimerSession
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<MobTimerResponseDto>>> GetMobTimerSession()
        {
            var currentUser = await getCurrentUser();

            return await _context.MobTimerSession
                .Where(m => m.Owner.Username.Equals(currentUser.Username) ||
                            m.Members.Any(s => s.User.Username.Equals(currentUser.Username)))
                .Include(e => e.Members)
                .Select(m => _mapper.Map<MobTimerResponseDto>(m))
                .ToListAsync();
        }

        // GET: api/MobTimerSession/5
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<MobTimerResponseDto>> GetMobTimerSession(int id)
        {
            var currentUser = await getCurrentUser();

            var mobTimerSession = await _context.MobTimerSession.Where(m => m.Owner.Username.Equals(currentUser.Username) ||
                            m.Members.Any(s => s.User.Username.Equals(currentUser.Username)))
                .Include(e => e.Members)
                .Where(m => m.Id == id)
                .Select(m => _mapper.Map<MobTimerResponseDto>(m)).SingleOrDefaultAsync();

            if (mobTimerSession == null)
            {
                return NotFound();
            }

            return mobTimerSession;
        }

        // PUT: api/MobTimerSession/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutMobTimerSession(int id, MobTimerSession mobTimerSession)
        {
            if (id != mobTimerSession.Id)
            {
                return BadRequest();
            }

            _context.Entry(mobTimerSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!mobTimerSessionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MobTimerResponseDto>> PostMobTimerSession(CreateMobTimerDto request)
        {
            var currentUser = await getCurrentUser();
            var newMobTimer = _mapper.Map<MobTimerSession>(request);
            newMobTimer.Owner = currentUser;

            newMobTimer = _context.MobTimerSession.Add(newMobTimer).Entity;
            _context.SessionMember.Add(new SessionMember()
            {
                User = currentUser,
                Session = newMobTimer
            });

            await _context.SaveChangesAsync();

            var response = _mapper.Map<MobTimerResponseDto>(newMobTimer);

            return CreatedAtAction("GetMobTimerSession", new { id = response.Id }, response);
        }

        // DELETE: api/MobTimerSession/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteMobTimerSession(int id)
        {
            var mobTimerSession = await _context.MobTimerSession.FindAsync(id);
            if (mobTimerSession == null)
            {
                return NotFound();
            }

            _context.MobTimerSession.Remove(mobTimerSession);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<User> getCurrentUser()
        {
            return await _context.User.Where(u => u.Username.Equals(User.Identity.Name)).SingleOrDefaultAsync();
        }

        private bool mobTimerSessionExists(int id)
        {
            return _context.MobTimerSession.Any(e => e.Id == id);
        }
    }
}
