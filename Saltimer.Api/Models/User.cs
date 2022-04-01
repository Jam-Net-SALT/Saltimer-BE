using System.Text.Json.Serialization;

namespace Saltimer.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public string ProfileImage { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }

        public virtual List<MobTimerSession> MobTimers { get; set; }
    }
}
