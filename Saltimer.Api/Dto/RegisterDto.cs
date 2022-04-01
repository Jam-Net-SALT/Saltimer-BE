using System.ComponentModel.DataAnnotations;

namespace Saltimer.Api.Dto
{
    public class RegisterDto
    {
        private const string passwordErrorMessage = "Password must contain minimum eight characters, at least one letter and one number";

        [Required]
        [MinLength(2)]
        [MaxLength(8)]
        public string Username { get; set; }

        [Required]
        [EmailAddressAttribute]
        public string Email { get; set; }

        [Required]
        [UrlAttribute]
        public string ProfileImageUrl { get; set; }

        [Required]
        [RegularExpressionAttribute(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
                                    ErrorMessage = passwordErrorMessage)]
        public string Password { get; set; }
    }

}
