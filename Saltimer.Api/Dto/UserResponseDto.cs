using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ProfileImage { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedAt { get; private set; }
    }

    public class UserToUserResponseDtoProfile : Profile
    {
        public UserToUserResponseDtoProfile()
        {
            CreateMap<User, UserResponseDto>();
        }
    }
}
