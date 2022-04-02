using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class MobTimerResponseDto
    {
        public int Id { get; set; }

        public string UniqueId { get; set; }

        public string DisplayName { get; set; }

        public int RoundTime { get; set; }

        public DateTime StartTime { get; set; }

        public int BreakTime { get; set; }

        public DateTime PausedTime { get; set; }

        public virtual List<string> MembersUsername { get; set; }
    }

    public class MobTimerSessionToMobTimerResponseDtoProfile : Profile
    {
        public MobTimerSessionToMobTimerResponseDtoProfile()
        {
            CreateMap<MobTimerSession, MobTimerResponseDto>()
            .ForMember(
                dest => dest.MembersUsername,
                opt => opt.MapFrom(src => src.Members.Select(m => m.User.Username))
            );
        }
    }
}
