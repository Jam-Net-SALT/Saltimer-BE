namespace Saltimer.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? ProfileImage { get; set; }

        public string EmailAddress { get; set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public virtual List<MobTimerSession> MobTimers { get; set; }
    }
}
