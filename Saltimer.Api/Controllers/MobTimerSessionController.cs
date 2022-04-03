#nullable disable
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    public class MobTimerSessionController : BaseController
    {
        public MobTimerSessionController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
           : base(mapper, authService, context) { }

        // GET: api/MobTimerSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobTimerResponseDto>>> GetMobTimerSession()
        {
            var currentUser = _authService.GetCurrentUser();

            return await _context.MobTimerSession
                .Where(m => m.Owner.Username.Equals(currentUser.Username) ||
                            m.Members.Any(s => s.User.Username.Equals(currentUser.Username)))
                .Include(e => e.Members)
                .Select(m => _mapper.Map<MobTimerResponseDto>(m))
                .ToListAsync();
        }

        // GET: api/MobTimerSession/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MobTimerResponseDto>> GetMobTimerSession(int id)
        {
            var currentUser = _authService.GetCurrentUser();

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

        [HttpPut("{id}")]
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

        [HttpPost]
        public async Task<ActionResult<MobTimerResponseDto>> PostMobTimerSession(CreateMobTimerDto request)
        {
            var currentUser = _authService.GetCurrentUser();
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

        [HttpDelete("{id}")]
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

        private bool mobTimerSessionExists(int id)
        {
            return _context.MobTimerSession.Any(e => e.Id == id);
        }
    }
}
