using System.Text;
using System.Security.Cryptography;
using api.Models;

namespace api
{
    public class UserAuthService
    {
        private readonly ILogger<UserAuthService> _logger;
        private readonly IConfiguration _configuration;
        public UserAuthService(ILogger<UserAuthService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public byte[] CreatePasswordHashByte(string password, string salt)
        {
            string passwordSalt = string.Empty;
            passwordSalt = salt + password;

            _logger.LogInformation($"passwordSalt: {passwordSalt}");

            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] hashValue = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordSalt));
                return hashValue;
            }
        }

        public String ByteToHexString(Byte[] result)
        {
            string hexString = string.Empty;

            for (int i = 0; i < result.Length; i++)
            {
                hexString += result[i].ToString("X2");
            }
            return hexString;
        }

        public bool VertifyUserCreds(UserLoginRequest requestCreds, User userCredsFromServer, String salt)
        {
            Byte[] result = CreatePasswordHashByte(requestCreds.Password, salt);

            return (ByteToHexString(result) == ByteToHexString(userCredsFromServer.Password));
        }
    }
}