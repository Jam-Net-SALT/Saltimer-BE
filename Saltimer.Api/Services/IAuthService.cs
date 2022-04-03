using Saltimer.Api.Models;

namespace Saltimer.Api.Services
{
    public interface IAuthService
    {
        public User GetCurrentUser();
        public string CreateToken(User modelUser);

        public int? ValidateToken(string token);

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
