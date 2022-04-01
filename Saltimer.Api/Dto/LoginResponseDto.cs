namespace Saltimer.Api.Dto
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;
    }
}
