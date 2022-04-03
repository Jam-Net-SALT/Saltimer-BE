using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class UpdateUserDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string EmailAddress { get; set; }

        [Required]
        [UrlAttribute]
        public string ProfileImage { get; set; }
    }

    public class UserToUpdateUserDtoProfile : Profile
    {
        public UserToUpdateUserDtoProfile()
        {
            CreateMap<UpdateUserDto, User>();
        }
    }
}
