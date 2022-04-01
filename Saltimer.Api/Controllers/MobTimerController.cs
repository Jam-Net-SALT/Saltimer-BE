#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Models;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobTimerSessionController : ControllerBase
    {
        private readonly SaltimerDBContext _context;

        public MobTimerSessionController(SaltimerDBContext context)
        {
            _context = context;
        }

        // GET: api/MobTimerSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobTimerSession>>> GetMobTimerSession()
        {
            //return await _context.MobTimerSession.ToListAsync();
            return await _context.Set<MobTimerSession>()
                .Include(e => e.UserMobSessions)
                .ToListAsync();

        }

        // GET: api/MobTimerSession/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MobTimerSession>> GetMobTimerSession(int id)
        {
            var mobTimerSession = await _context.MobTimerSession.FindAsync(id);

            if (mobTimerSession == null)
            {
                return NotFound();
            }

            return mobTimerSession;
        }

        // PUT: api/MobTimerSession/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/MobTimerSession
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MobTimerSession>> PostMobTimerSession(MobTimerSession mobTimerSession)
        {
            _context.MobTimerSession.Add(mobTimerSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobTimerSession", new { id = mobTimerSession.Id }, mobTimerSession);
        }

        // DELETE: api/MobTimerSession/5
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
