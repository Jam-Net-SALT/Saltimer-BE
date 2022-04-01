using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class UserUpdateRequestDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string EmailAddress { get; set; }

        [Required]
        [UrlAttribute]
        public string ProfileImage { get; set; }
    }

    public class UserToUserUpdateRequestDtoProfile : Profile
    {
        public UserToUserUpdateRequestDtoProfile()
        {
            CreateMap<UserUpdateRequestDto, User>();
        }
    }
}
