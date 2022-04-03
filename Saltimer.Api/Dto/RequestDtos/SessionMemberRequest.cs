using System.ComponentModel.DataAnnotations;

namespace Saltimer.Api.Dto
{
    public class SessionMemberRequest
    {
        [Required]
        public int UserId { get; set; }

    }
}
