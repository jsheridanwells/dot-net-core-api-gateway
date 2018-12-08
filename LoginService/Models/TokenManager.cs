using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoginService.Models
{
    public class TokenManager
    {
        // private readonly string _secret;
        // public TokenManager(IOptions<TokenSecret> secret)
        // {
        //     _secret = secret.Value.Secret;
        // }
        private static string _secret = "SmYH2i3naXS6ihUyBqCI5Gl0p75adTr1S0GVpx+yoVdPa90av6UsWEGR5oBmwxpePfIu11ZazjyW680eFYy3zA==";
        
        public static string GenerateToken(string name)
        {
            byte[] key = Convert.FromBase64String(_secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new [] { new Claim(ClaimTypes.Name, name) }),
                Expires = DateTime.UtcNow.AddMinutes(45),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (token == null)
                    return null;
                
                byte[] key = Convert.FromBase64String(_secret);

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(
                    token,
                    parameters,
                    out securityToken
                );
                return principal;                
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;                
            }
            catch(NullReferenceException)
            {
                return null;
            }

            Claim usernameClaim =identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }
    }
}
