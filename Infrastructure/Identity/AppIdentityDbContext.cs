using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    //extending IdentityDbContext class,Passing AppUser to get extended fields in the DB migration
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        //we have to specify the type inside DBContextOptions as there is more than one DBContext
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }

        //to avoid error with identity giving an error for primary key field of AppUser ID
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
