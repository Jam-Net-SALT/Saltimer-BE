using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class SessionMemberResponse
    {
        public int Id { get; set; }
        public int Turn { get; set; }
        public virtual UserResponseDto User { get; set; }
        public virtual MobTimerResponse Session { get; set; }
    }

    public class UserToSessionMemberResponseProfile : Profile
    {
        public UserToSessionMemberResponseProfile()
        {
            CreateMap<SessionMember, SessionMemberResponse>()
            .ForMember(
                dest => dest.User,
                opt => opt.MapFrom(src => src.User)
            )
            .ForMember(
                dest => dest.Session,
                opt => opt.MapFrom(src => src.Session)
            );
        }
    }
}
