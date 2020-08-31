using Blazor.Startechmanager.Server.Models;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IPersistedGrantDbContext , IDisposable, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
        {
        }


        public DbSet<ApplicationUser> Users { get; set; }
        DbSet<PersistedGrant> IPersistedGrantDbContext.PersistedGrants { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        DbSet<DeviceFlowCodes> IPersistedGrantDbContext.DeviceFlowCodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        int IPersistedGrantDbContext.SaveChanges()
        {
            throw new NotImplementedException();
        }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
