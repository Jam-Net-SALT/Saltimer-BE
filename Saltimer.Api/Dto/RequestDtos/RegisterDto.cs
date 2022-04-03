using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Saltimer.Api.Models;

namespace Saltimer.Api.Dto
{
    public class RegisterDto
    {
        private const string passwordErrorMessage = "Password must contain minimum eight characters, at least one letter and one number";

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
        [RegularExpressionAttribute(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
                                    ErrorMessage = passwordErrorMessage)]
        public string Password { get; set; }
    }

    public class RegisterDtoToUserProfile : Profile
    {
        public RegisterDtoToUserProfile()
        {
            CreateMap<RegisterDto, User>();
        }
    }

}
