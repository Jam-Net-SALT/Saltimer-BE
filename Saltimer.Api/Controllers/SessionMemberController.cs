#nullable disable
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Dto;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    public class SessionMemberController : BaseController
    {
        public SessionMemberController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
            : base(mapper, authService, context) { }

        // GET: api/SessionMember
        [HttpGet("{mobTimerId}")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetSessionMember(int mobTimerId)
        {
            var currentUser = _authService.GetCurrentUser();
            var targetMobTimer = await _context.SessionMember
                    .Where(sm => sm.User.Id == currentUser.Id)
                    .Where(sm => sm.Session.Id == mobTimerId)
                    .Select(sm => sm.Session)
                    .FirstOrDefaultAsync();

            if (targetMobTimer == null) return NotFound();

            return await _context.SessionMember
                    .Where(sm => sm.Session.Id == targetMobTimer.Id)
                    .Select(sm => _mapper.Map<UserResponseDto>(sm.User))
                    .ToListAsync();
        }

        // GET: api/SessionMember
        [HttpGet("{mobTimerId}/vip/{uuid}")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetSessionMemberByUUID(int mobTimerId, Guid uuid)
        {
            var currentUser = _authService.GetCurrentUser();
            var targetMobTimer = await _context.SessionMember
                    .Where(sm => sm.Session.UniqueId.Equals(uuid.ToString()))
                    .Where(sm => sm.Session.Id == mobTimerId)
                    .Select(sm => sm.Session)
                    .FirstOrDefaultAsync();

            if (targetMobTimer == null) return NotFound(new ErrorResponse()
            {
                Message = $"Mobtimer session {uuid} with Id {mobTimerId} not found.",
                Status = StatusCodes.Status404NotFound
            });

            return await _context.SessionMember
                    .Where(sm => sm.Session.Id == targetMobTimer.Id)
                    .Select(sm => _mapper.Map<UserResponseDto>(sm.User))
                    .ToListAsync();
        }

        [HttpPost("{mobTimerId}")]
        [ActionName(nameof(PostSessionMember))]
        public async Task<ActionResult<SessionMemberResponse>> PostSessionMember(int mobTimerId, SessionMemberRequest request)
        {
            var currentUser = _authService.GetCurrentUser();
            var targetUser = await _context.User.FindAsync(request.UserId);

            if (targetUser == null) return NotFound(new ErrorResponse()
            {
                Message = "Target user not found.",
                Status = StatusCodes.Status404NotFound
            });

            var targetMobTimer = await _context.SessionMember
                    .Include(sm => sm.Session.Members)
                    .Where(sm => sm.User.Id == currentUser.Id)
                    .Where(sm => sm.Session.Id == mobTimerId)
                    .Select(sm => sm.Session)
                    .FirstOrDefaultAsync();

            if (targetMobTimer == null) return NotFound(new ErrorResponse()
            {
                Message = "Mobtimer session not found.",
                Status = StatusCodes.Status404NotFound
            });

            var userAlreadyMember = _context.SessionMember
                    .Any(sm => sm.Session.Id == mobTimerId && sm.User.Id == targetUser.Id);

            if (userAlreadyMember) return BadRequest(new ErrorResponse()
            {
                Message = "Provided user is already a member.",
                Status = StatusCodes.Status400BadRequest
            });

            var newRecord = _context.SessionMember.Add(new SessionMember()
            {
                User = targetUser,
                Session = targetMobTimer,
                Turn = targetMobTimer.Members.Max(m => m.Turn) + 1,
            }).Entity;
            await _context.SaveChangesAsync();

            var sessionMember = _context.SessionMember
                        .Include(sm => sm.User)
                        .Include(sm => sm.Session)
                        .Where(s => s.Id == newRecord.Id)
                        .Single();

            var response = _mapper.Map<SessionMemberResponse>(sessionMember);

            return CreatedAtAction(nameof(PostSessionMember), response);
        }

        [HttpPost("{mobTimerId}/vip")]
        [ActionName(nameof(PostSessionMember))]
        public async Task<ActionResult<SessionMemberResponse>> PostSessionMemberByUUID(int mobTimerId, VipSessionMemberRequest request)
        {
            var currentUser = _authService.GetCurrentUser();

            var targetMobTimer = await _context.SessionMember
                    .Include(sm => sm.Session.Members)
                    .Where(sm => sm.Session.UniqueId.Equals(request.Uuid.ToString()))
                    .Where(sm => sm.Session.Id == mobTimerId)
                    .Select(sm => sm.Session)
                    .FirstOrDefaultAsync();

            if (targetMobTimer == null) return NotFound(new ErrorResponse()
            {
                Message = $"Mobtimer session {request.Uuid} with Id {mobTimerId} not found.",
                Status = StatusCodes.Status404NotFound
            });

            var userAlreadyMember = _context.SessionMember
                    .Any(sm => sm.Session.Id == mobTimerId && sm.User.Id == currentUser.Id);

            if (userAlreadyMember) return BadRequest(new ErrorResponse()
            {
                Message = "You are already a member.",
                Status = StatusCodes.Status400BadRequest
            });

            var newRecord = _context.SessionMember.Add(new SessionMember()
            {
                User = currentUser,
                Session = targetMobTimer,
                Turn = targetMobTimer.Members.Max(m => m.Turn) + 1,
            }).Entity;
            await _context.SaveChangesAsync();

            var sessionMember = _context.SessionMember
                        .Include(sm => sm.User)
                        .Include(sm => sm.Session)
                        .Where(s => s.Id == newRecord.Id)
                        .Single();

            var response = _mapper.Map<SessionMemberResponse>(sessionMember);

            return CreatedAtAction(nameof(PostSessionMember), response);
        }


        // DELETE: api/SessionMember/5
        [HttpDelete("{mobTimerId}")]
        public async Task<IActionResult> DeleteSessionMember(int mobTimerId, SessionMemberRequest request)
        {
            var currentUser = _authService.GetCurrentUser();

            var sessionMember = await _context.SessionMember
                        .Include(sm => sm.Session.Owner)
                        .Include(sm => sm.User)
                        .SingleOrDefaultAsync(sm => sm.Session.Id == mobTimerId && sm.User.Id == request.UserId);

            if (sessionMember == null) return NotFound();


            if (sessionMember.Session.Owner.Id != currentUser.Id && sessionMember.User.Id != currentUser.Id)
                return Forbid();

            if (sessionMember.Session.Owner.Id == currentUser.Id)
            {
                var mobTimer = await _context.MobTimerSession
                        .Include(ms => ms.Members)
                        .SingleOrDefaultAsync(ms => ms.Id == mobTimerId);

                _context.RemoveRange(mobTimer);
                _context.RemoveRange(mobTimer.Members);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            _context.SessionMember.Remove(sessionMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SessionMemberExists(int id)
        {
            return _context.SessionMember.Any(e => e.Id == id);
        }
    }
}
