using Blazor.Startechmanager.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Startechmanager.Server.Data
{
    public static class SeedDatas
    {
        public static async void  Seed( ApplicationDbContext dbContext )
        {
            if(!dbContext.Roles.Any())
            {
                var adminRole = new ApplicationRole
                {
                    Name = Roles.Admin,
                    NormalizedName = Roles.Admin
                };
                var roleStore = new RoleStore<ApplicationRole, ApplicationDbContext, int>(dbContext);
                await roleStore.CreateAsync(adminRole);
            }

            if (!dbContext.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    Email = "admin@softeam.fr",
                    NormalizedEmail = "ADMIN@SOFTEAM.FR",
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Admin } },
                    Roles = new List<ApplicationRole> { dbContext.Roles.First() }                     
                };

                SetPasswordModel(admin, dbContext);

                await dbContext.SaveChangesAsync();
            }

        }

        private static async void SetPasswordModel(ApplicationUser user, ApplicationDbContext dbContext)
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, "password");
            user.PasswordHash = hashed;

            var userStore = new UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext, int>(dbContext);
            await userStore.CreateAsync(user);
        }
    }
}
