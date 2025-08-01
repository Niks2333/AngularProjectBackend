using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace InventoryManagementLibrary.Helpers
{
    public static class JwtManager
    {
        private static readonly string SecretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];


        public static string GenerateToken(string username, int expireMinutes = 60)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            };

            try
            {
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}