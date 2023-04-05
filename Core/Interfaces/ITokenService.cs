using Core.Entities.Identity;

namespace Core.Interfaces
{
    //Service to use JSON Web Tokens
    public interface ITokenService
    {
        //Method to create a string as our token will be a string
        //Using appuser to populate the fields inside token
        public string CreateToken(AppUser appUser);
    }
}
