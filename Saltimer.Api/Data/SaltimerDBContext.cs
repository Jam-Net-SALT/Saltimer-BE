#nullable disable
using Microsoft.EntityFrameworkCore;
using Saltimer.Api.Models;

public class SaltimerDBContext : DbContext
{
    public SaltimerDBContext(DbContextOptions<SaltimerDBContext> options)
        : base(options)
    {
    }

    public DbSet<User> User { get; set; }

    public DbSet<MobTimerSession> MobTimerSession { get; set; }

    public DbSet<SessionMember> SessionMember { get; set; }


}
