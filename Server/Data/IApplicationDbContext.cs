using Blazor.Startechmanager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Startechmanager.Server.Data
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; set; }
    }
}