namespace Saltimer.Api.Dto
{
    public class ErrorResponse
    {

        public int Status { get; set; }

        public string Message { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;
    }
}
