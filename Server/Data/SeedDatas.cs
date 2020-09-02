using Blazor.Startechmanager.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Data
{
    public static class SeedDatas
    {
        public static async Task Seed(IApplicationBuilder builder)
        {
            var provider = builder.ApplicationServices;
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {


                if (!dbContext.Roles.Any())
                {
                    var adminRole = new ApplicationRole
                    {
                        Name = Roles.Admin,
                        NormalizedName = Roles.Admin
                    };
                    await CreateRole(dbContext, adminRole);
                }

                if (!dbContext.Users.Any())
                {
                    var admin = new ApplicationUser
                    {
                        Email = "admin@softeam.fr",
                        NormalizedEmail = "ADMIN@SOFTEAM.FR",
                        UserName = "admin@softeam.fr",
                        NormalizedUserName = "ADMIN@SOFTEAM.FR",
                        PhoneNumber = "+111111111111",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Admin } }
                    };

                    await SetPasswordModel(admin, dbContext);
                    await AddRole(admin, Roles.Admin, dbContext);

                }

                dbContext.SaveChanges();
            }

        }

        private static async Task CreateRole(ApplicationDbContext dbContext, ApplicationRole adminRole)
        {
            var roleStore = new RoleStore<ApplicationRole, ApplicationDbContext, int>(dbContext);
            await roleStore.CreateAsync(adminRole);
        }

        private static async Task AddRole (ApplicationUser user, string role, ApplicationDbContext dbContext)
        {
            var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, int>(dbContext);
            await userStore.AddToRoleAsync(user, role);
        }

        private static async Task SetPasswordModel(ApplicationUser user, ApplicationDbContext dbContext)
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, "password");
            user.PasswordHash = hashed;

            var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, int>(dbContext);
            await userStore.CreateAsync(user);
        }
    }
}
