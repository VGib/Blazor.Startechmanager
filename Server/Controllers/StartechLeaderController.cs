﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace Blazor.Startechmanager.Server.Controllers
{
    [Route("{controller}/{startechType}/{action}/{id?}")]
    [Authorize(Policy = Roles.Admin)]
    [ApiController]
    public class StartechLeaderController : ControllerBase
    {
        public StartechLeaderController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, HttpContextAccessor httpContext)
        {
            DbContext = dbContext;
            UserManager = userManager;
            HttpContextAccessor = httpContext;
        }

        public ApplicationDbContext  DbContext { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public HttpContextAccessor HttpContextAccessor { get; }

        [HttpGet]
        public async Task<IList<UserObject>> GetLeaders([FromRoute] string startechType)
        {
            if(startechType == Roles.Admin)
            {
                return await DbContext.Users.Include(x => x.Roles)
                    .Where(x => x.Roles.Any(y => y.Name == Roles.Admin))
                    .Select(x => new UserObject { Id = x.Id, UserName = x.UserName }).ToListAsync();
            }
            else
            {
                Startechs startech = GetStarttechAsEnum(startechType);
                return await DbContext.Users.Include(x => x.Startechs)
                       .Where(x => x.Startechs.Any(y => y.Startech == startech && y.IsLeader))
                       .Select(x => new UserObject { Id = x.Id, UserName = x.UserName }).ToListAsync();
            }
        }

        private static Startechs GetStarttechAsEnum(string startechType)
        {
            if (!Enum.TryParse(typeof(Startechs), startechType, out var startechAsObject))
            {
                throw new ArgumentException("unknow startech type");
            }

            return (Startechs)startechAsObject;
        }

        [HttpGet]
        public async Task<IActionResult> AddLeader([FromRoute] string startechType, [FromRoute] int userId)
        {
            var user = await DbContext.Users.Include(x => x.Startechs).Include(x => x.Roles)
                            .FirstOrDefaultAsync(x => x.Id == userId);

            if(startechType == Roles.Admin)
            {
                await UserManager.AddToRoleAsync(user, Roles.Admin);

                return Ok();
            }
            else
            {
                var startech = GetStarttechAsEnum(startechType);

                var startechMapper = user.Startechs.Find(x => x.Startech == startech && x.UserId == userId);

                if(startechMapper is null)
                {
                    startechMapper = new MappingStartechUser
                    {
                        IsLeader = true,
                        Startech = startech,
                        UserId = user.Id
                    };

                    DbContext.MappingStartechs.Add(startechMapper);
                }
                else
                {
                    startechMapper.IsLeader = true;
                    DbContext.Entry(startechMapper).Property(x => x.IsLeader).IsModified = true;
                }

                await DbContext.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpGet]
        public async Task<IActionResult> RemoveLeader([FromRoute] string startechType, [FromRoute] int userId)
        {
            var userToDelete = DbContext.Users.Include(x => x.Startechs).FirstOrDefault(x => x.Id == userId);

            if (userToDelete is null)
            {
                return BadRequest("unknow user");
            }

            if (startechType == Roles.Admin)
            {
                if (userId == (await UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User)).Id)
                {
                    return BadRequest("you can't delete yourself");
                }

                await UserManager.RemoveFromRoleAsync(userToDelete, Roles.Admin);

                return Ok();
            }
            else
            {
                var startech = GetStarttechAsEnum(startechType);
                var mappingStartechForUser = userToDelete.Startechs.FirstOrDefault(x => x.Startech == startech);

                if(mappingStartechForUser != null)
                {
                    mappingStartechForUser.IsLeader = false;
                    DbContext.Entry(mappingStartechForUser).Property(x => x.IsLeader).IsModified = true;
                    await DbContext.SaveChangesAsync();
                }

                return Ok();
            }
        }
    }
}
