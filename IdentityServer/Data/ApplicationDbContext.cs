using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        //Client table
        public DbSet<Client> Clients {get; set; }
        public DbSet<ClientClaim> ClientClaims {get; set; }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins {get; set; }
        public DbSet<ClientGrantType> ClientGrantTypes {get; set; }
        public DbSet<ClientIdPRestriction> ClientIdPRestrictions {get; set; }
        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris {get; set; }
        public DbSet<ClientProperty> ClientProperties {get; set; }
        public DbSet<ClientRedirectUri> ClientRedirectUris {get; set; }
        public DbSet<ClientScope> ClientScopes {get; set; }
        public DbSet<ClientSecret> ClientSecrets {get; set; }

        //Api scope table
        public DbSet<ApiResource> ApiResources {get; set; }
        public DbSet<ApiResourceClaim> ApiResourceClaims {get; set; }
        public DbSet<ApiResourceProperty> ApiResourceProperties {get; set; }
        public DbSet<ApiResourceScope> ApiResourceScopes {get; set; }
        public DbSet<ApiResourceSecret> ApiResourceSecrets {get; set; }
        public DbSet<ApiScope> ApiScopes {get; set; }
        public DbSet<ApiScopeClaim> ApiScopeClaims {get; set; }
        public DbSet<ApiScopeProperty> ApiScopeProperties {get; set; }

        //Pr
        public DbSet<IdentityResource> IdentityResources {get; set; }
        public DbSet<IdentityResourceClaim> IdentityResourceClaims {get; set; }
        public DbSet<IdentityResourceProperty> IdentityResourceProperties {get; set; }
        public DbSet<PersistedGrant> PersistedGrants {get; set;}
        public DbSet<DeviceFlowCodes> DeviceCodes { get; set; }
        //Custom model
        public DbSet<RoleClientGrant> RoleClientGrants {get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<PersistedGrant>()
            .HasKey(p => p.Key);

            builder.Entity<DeviceFlowCodes>()
            .HasKey(p => p.DeviceCode);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseLazyLoadingProxies();
            // other configurations...
        }
    }
}
