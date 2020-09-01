using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Startechmanager.Server.Controllers
{


    [Route("{controller}/{startechType}/{action}/{id?:int}")]
    [Authorize(Roles.Admin)]
    [ApiController]
    public class StartechLeaderController : ControllerBase
    {
        public ApplicationDbContext  DbContext { get; set; }


        [HttpGet]
        public async Task<IList<UserObject>> GetLeaders(string startechType)
        {
            if(startechType == Roles.Admin)
            {
                return await DbContext.Users.Include(x => x.Roles)
                    .Where(x => x.Roles.Any(y => y.Name == Roles.Admin))
                    .Select(x => new UserObject { Id = x.Id, UserName = x.UserName }).ToListAsync();
            }
            else
            {
                if(!Enum.TryParse(typeof(Startechs),startechType, out var startechAsObject))
                {
                    throw new ArgumentException("unknow startech type");
                }

                var startech = (Startechs)startechAsObject;
                return await DbContext.Users.Include(x => x.Startechs)
                       .Where(x => x.Startechs.Any(y => y.Startech == startech && y.IsLeader))
                       .Select(x => new UserObject { Id = x.Id, UserName = x.UserName }).ToListAsync();
            }
        }

    }
}
