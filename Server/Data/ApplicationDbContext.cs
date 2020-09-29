using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IPersistedGrantDbContext , IDisposable, IApplicationDbContext
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

#nullable disable
        DbSet<PersistedGrant> IPersistedGrantDbContext.PersistedGrants { get; set; }
        DbSet<DeviceFlowCodes> IPersistedGrantDbContext.DeviceFlowCodes { get; set; }

        public virtual DbSet<MappingStartechUser> MappingStartechs { get; set; }

        public virtual DbSet<StarpointsItem> StarpointsItem { get; set; }

        public virtual DbSet<StarpointsType> StarpointsType { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

#nullable enable

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
        }
    }
}
