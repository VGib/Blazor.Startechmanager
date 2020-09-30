using Blazor.Startechmanager.Server.Data;
using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Controllers
{
    [Route("{controller}/{startechType}/{action}/{userId:int?}")]
    public class StarPointsManagerController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public StarPointsManagerController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<StarpointsType>> GetStarpointsType()
        {
            return await dbContext.StarpointsType.Where(x => x.IsActive).ToListAsync();
        }
    }
}
