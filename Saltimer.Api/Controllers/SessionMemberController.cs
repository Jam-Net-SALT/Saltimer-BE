#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionMemberController : ControllerBase
    {
        private readonly SaltimerDBContext _context;

        public SessionMemberController(SaltimerDBContext context)
        {
            _context = context;
        }

        // GET: api/SessionMember
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionMember>>> GetSessionMember()
        {
            return await _context.SessionMember.ToListAsync();
        }

        // GET: api/SessionMember/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionMember>> GetSessionMember(int id)
        {
            var SessionMember = await _context.SessionMember.FindAsync(id);

            if (SessionMember == null)
            {
                return NotFound();
            }

            return SessionMember;
        }

        // PUT: api/SessionMember/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessionMember(int id, SessionMember sessionMember)
        {
            if (id != sessionMember.Id)
            {
                return BadRequest();
            }

            _context.Entry(sessionMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionMemberExists(id))
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

        // POST: api/SessionMember
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SessionMember>> PostSessionMember(SessionMember sessionMember)
        {
            _context.SessionMember.Add(sessionMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSessionMember", new { id = sessionMember.Id }, sessionMember);
        }

        // DELETE: api/SessionMember/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSessionMember(int id)
        {
            var SessionMember = await _context.SessionMember.FindAsync(id);
            if (SessionMember == null)
            {
                return NotFound();
            }

            _context.SessionMember.Remove(SessionMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SessionMemberExists(int id)
        {
            return _context.SessionMember.Any(e => e.Id == id);
        }
    }
}
