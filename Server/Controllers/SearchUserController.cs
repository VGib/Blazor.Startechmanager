using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Controllers
{
    public class SearchUserController
    {
        public SearchUserController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; set; }

        [HttpPost("SearchUser")]
        public async Task<IList<UserObject>> Search([FromBody] string userToSearch)
        {
            if(userToSearch?.Length < 3)
            {
                return Array.Empty<UserObject>();
            }

#pragma warning disable CS8604 // Possible null reference argument.
            return await DbContext.Users.Where(x => x.UserName.Contains(userToSearch)).Take(10)
#pragma warning restore CS8604 // Possible null reference argument.
                .Select(x => new UserObject { Id = x.Id, UserName = x.UserName }).ToListAsync();
        }
    }
}
