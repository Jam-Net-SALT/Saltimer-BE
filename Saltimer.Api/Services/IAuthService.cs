using Saltimer.Api.Models;

namespace Saltimer.Api.Services
{
    public interface IAuthService
    {
        public string CreateToken(User modelUser);

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
