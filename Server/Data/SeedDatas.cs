using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
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
                    await AddAdmin(dbContext);
                    await AddStartechLeaders(dbContext);
                    await AddStartechMembers(dbContext);
                }

                dbContext.SaveChanges();
            }

        }

        private static async Task AddAdmin(ApplicationDbContext dbContext)
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
            };

            await SetPasswordModel(admin, dbContext);
            await AddRole(admin, Roles.Admin, dbContext);
        }

        private static async Task AddStartechLeaders(ApplicationDbContext dbContext)
        {
            var startechleaderjava = new ApplicationUser
            {
                Email = "startechleaderjava@softeam.fr",
                NormalizedEmail = "STARTECHLEADERJAVA@SOFTEAM.FR",
                UserName = "startechleaderjava@softeam.fr",
                NormalizedUserName = "STARTECHLEADERJAVA@SOFTEAM.FR",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Java , IsLeader = true } }
            };

            await SetPasswordModel(startechleaderjava, dbContext);

            var startechleaderdotnet = new ApplicationUser
            {
                Email = "startechleaderdotnet@softeam.fr",
                NormalizedEmail = "STARTECHLEADERDOTNET@SOFTEAM.FR",
                UserName = "startechleaderdotnet@softeam.fr",
                NormalizedUserName = "STARTECHLEADERDOTNET@SOFTEAM.FR",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true } }
            };

            await SetPasswordModel(startechleaderdotnet, dbContext);

            var startechleaderagile = new ApplicationUser
            {
                Email = "startechleaderagile@softeam.fr",
                NormalizedEmail = "STARTECHLEADERAGILE@SOFTEAM.FR",
                UserName = "startechleaderagile@softeam.fr",
                NormalizedUserName = "STARTECHLEADERAGILE@SOFTEAM.FR",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Agile, IsLeader = true } }
            };

            await SetPasswordModel(startechleaderagile, dbContext);
        }

        private static async Task AddStartechMembers(ApplicationDbContext dbContext)
        {
          foreach(Startechs startech in  Enum.GetValues(typeof(Startechs)))
          for(int n = 1; n <= 5; ++n)
            {
                var member = new ApplicationUser
                {
                    Email = $"startechmember{startech.ToString().ToLower()}{n}@softeam.fr",
                    NormalizedEmail = $"STARTECHMEMBER{startech.ToString().ToUpper()}{n}@SOFTEAM.FR",
                    UserName = $"startechmember{startech.ToString().ToLower()}{n}@softeam.fr",
                    NormalizedUserName = $"STARTECHMEMBER{startech.ToString().ToUpper()}{n}@SOFTEAM.FR",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = startech} }
                };

                await SetPasswordModel(member, dbContext);
            }
        }

        private static async Task AddUsers(ApplicationDbContext dbContext)
        {
            for (int n = 1; n <= 10; ++n)
            {
                var member = new ApplicationUser
                {
                    Email = $"user{n}@softeam.fr",
                    NormalizedEmail = $"USER{n}@SOFTEAM.FR",
                    UserName = $"user{n}@softeam.fr",
                    NormalizedUserName = $"USER{n}@SOFTEAM.FR",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await SetPasswordModel(member, dbContext);
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
