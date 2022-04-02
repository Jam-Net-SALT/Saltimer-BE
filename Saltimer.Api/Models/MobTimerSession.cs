using System.ComponentModel.DataAnnotations;

namespace Saltimer.Api.Models;

public class MobTimerSession
{
    [Key]
    public int Id { get; set; }

    public string UniqueId { get; set; } = Guid.NewGuid().ToString();

    public string DisplayName { get; set; }

    public int RoundTime { get; set; }

    public DateTime StartTime { get; set; }

    public int BreakTime { get; set; }

    public DateTime PausedTime { get; set; }

    public virtual User Owner { get; set; }

    public virtual List<SessionMember> Members { get; set; }

}