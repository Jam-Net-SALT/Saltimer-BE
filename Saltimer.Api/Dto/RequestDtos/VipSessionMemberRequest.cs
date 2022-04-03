using System.ComponentModel.DataAnnotations;

namespace Saltimer.Api.Dto
{
    public class VipSessionMemberRequest
    {
        [Required]
        public Guid Uuid { get; set; }

    }
}
