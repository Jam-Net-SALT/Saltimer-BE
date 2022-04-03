#nullable disable
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    public class MobTimerController : BaseController
    {
        public MobTimerController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
           : base(mapper, authService, context) { }

        // GET: api/MobTimerSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobTimerResponse>>> GetMobTimerSession()
        {
            var currentUser = _authService.GetCurrentUser();

            return await _context.MobTimerSession
                .Where(m => m.Owner.Username.Equals(currentUser.Username) ||
                            m.Members.Any(s => s.User.Username.Equals(currentUser.Username)))
                .Include(e => e.Members)
                .Select(m => _mapper.Map<MobTimerResponse>(m))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MobTimerResponse>> GetMobTimerSession(int id)
        {
            var currentUser = _authService.GetCurrentUser();

            var mobTimerSession = await _context.MobTimerSession.Where(m => m.Owner.Username.Equals(currentUser.Username) ||
                            m.Members.Any(s => s.User.Username.Equals(currentUser.Username)))
                .Include(e => e.Members)
                .Where(m => m.Id == id)
                .Select(m => _mapper.Map<MobTimerResponse>(m)).SingleOrDefaultAsync();

            if (mobTimerSession == null)
            {
                return NotFound();
            }

            return mobTimerSession;
        }

        [HttpGet("{id}/vip/{uuid}")]
        public async Task<ActionResult<MobTimerResponse>> GetMobTimerSessionByUUID(int id, Guid uuid)
        {
            var mobTimerSession = await _context.MobTimerSession
                .Where(m => m.UniqueId.Equals(uuid.ToString()))
                .Include(e => e.Members)
                .Where(m => m.Id == id)
                .Select(m => _mapper.Map<MobTimerResponse>(m)).SingleOrDefaultAsync();

            if (mobTimerSession == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Message = $"Mobtimer session {uuid} with Id {id} not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return mobTimerSession;
        }

        [HttpPost]
        public async Task<ActionResult<MobTimerResponse>> PostMobTimerSession(CreateMobTimerDto request)
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
            var response = _mapper.Map<MobTimerResponse>(newMobTimer);

            return CreatedAtAction("GetMobTimerSession", new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMobTimerSession(int id)
        {
            var currentUser = _authService.GetCurrentUser();
            var mobTimerSession = await _context.MobTimerSession
                .Include(ms => ms.Members)
                .Where(ms => ms.Owner.Id == currentUser.Id)
                .Where(ms => ms.Id == id)
                .SingleOrDefaultAsync();

            if (mobTimerSession == null)
            {
                return NotFound();
            }

            _context.RemoveRange(mobTimerSession);
            _context.RemoveRange(mobTimerSession.Members);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool mobTimerSessionExists(int id)
        {
            return _context.MobTimerSession.Any(e => e.Id == id);
        }
    }
}
