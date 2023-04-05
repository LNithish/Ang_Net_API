using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService:ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly SymmetricSecurityKey key;

        //To create symmenric security key to sign the token
        public TokenService(IConfiguration configuration) 
        {
            this.configuration = configuration;
            this.key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]));
        }

        public string CreateToken(AppUser appUser)
        {
            //Token contains claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim(ClaimTypes.GivenName,appUser.DisplayName)
            };
            //token will be sent over an HTTP request, this token will be stored in client browser, user can view

            //creating credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //describing token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //This information will go inside th payload
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                //issued by this server
                Issuer = configuration["Token:Issuer"]
            };
            //to handle our token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
