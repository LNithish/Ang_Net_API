using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        //instead of using context directly like StoreContext to interact with Identity database,
        //We will be making use of things like UserManager ,SignInManager
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            //Creates users if there is no user 
            if(!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Nithish",
                    Email = "Nithish@test.com",
                    UserName = "Nithish@test.com",
                    Address = new Address
                    {
                        FirstName = "Nithish",
                        LastName = "Lingadurai",
                        Street = "21 Noyyal Street",
                        City = "Coimbatore",
                        State = "Tamilnadu",
                        Zipcode = "641033"
                    }
                };
                await userManager.CreateAsync(user,"Pa$$w0rd");
            }
        }
    }
}
