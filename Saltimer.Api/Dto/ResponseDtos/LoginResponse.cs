namespace Saltimer.Api.Dto
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;
    }
}
