using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Extensions
{
    //extension class for usermanager, it should be static
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserByClaimsPrincipleWithAddress(this UserManager<AppUser> userManager
            ,ClaimsPrincipal user)
        {
            var email=user.FindFirstValue(ClaimTypes.Email);
            //finding user with address details
            return await userManager.Users.Include(x=>x.Address).SingleOrDefaultAsync(x=>x.Email==email);
        }

        public static async Task<AppUser> FindUserByEmailFromClaimPrinciple(this UserManager<AppUser> userManager,
            ClaimsPrincipal user)
        {
            return await userManager.Users.SingleOrDefaultAsync(x => x.Email == user
            .FindFirstValue(ClaimTypes.Email));
        }
    }
}
