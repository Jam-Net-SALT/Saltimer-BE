using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class MobTimerResponse
    {
        public int Id { get; set; }

        public string UniqueId { get; set; }

        public string DisplayName { get; set; }

        public int RoundTime { get; set; }

        public DateTime StartTime { get; set; }

        public int BreakTime { get; set; }

        public DateTime PausedTime { get; set; }

    }

    public class MobTimerSessionToMobTimerResponseProfile : Profile
    {
        public MobTimerSessionToMobTimerResponseProfile()
        {
            CreateMap<MobTimerSession, MobTimerResponse>();
        }
    }
}
