using api.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace api
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(User user)
        {

            var issuer = _configuration.GetSection("JWT:Issuer").Value;
            var audience = _configuration.GetSection("JWT:Audience").Value;

            // Add user claims information into JWT Token 
            List<Claim> claims = GetClaims(user);

            // Option Roles for user claims
            //claims.Add(new Claim("roles", "Admin"));
            //claims.Add(new Claim("roles", "Users"));

            // Create SymmetricSecurityKey for JWT signing credentials
            var key = GetSymmetricSecurityKey("JWT:Token");

            // HmacSha512 Must by more than 128 bits，key must be large than 16 chars
            var signingCredentials = GetSigningCredentials(key);

            var expires = DateTime.Now.AddMinutes(60);
            var jwtSecurityToken = GetJwtSecurityToken(issuer, audience, claims, expires, signingCredentials);

            return GenerateJwtToken(jwtSecurityToken);
        }

        public List<Claim> GetClaims(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Mail),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.NameId, user.Guid.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID
            };

            return claims;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey(String section)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection(section).Value));

            return key;
        }

        public SigningCredentials GetSigningCredentials(SymmetricSecurityKey symmetricSecurityKey)
        {
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            return signingCredentials;
        }

        public JwtSecurityToken GetJwtSecurityToken(String issuer, String audience, List<Claim> claims, DateTime expiresDate, SigningCredentials signingCredentials)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }

        public String GenerateJwtToken(JwtSecurityToken jwtSecurityToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.WriteToken(jwtSecurityToken);

            return securityToken;
        }
    }
}