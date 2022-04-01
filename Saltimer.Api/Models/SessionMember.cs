using System.ComponentModel.DataAnnotations;

namespace Saltimer.Api.Models;

public class SessionMember
{
    [Key]
    public int Id { get; set; }

    public int Turn { get; set; }

    public virtual User User { get; set; }

    public virtual MobTimerSession Session { get; set; }
}