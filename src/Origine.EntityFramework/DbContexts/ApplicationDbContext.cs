using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Origine.Models;

namespace Origine.StorageProviders
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        static ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable(nameof(Users));
            builder.Entity<IdentityUserClaim<string>>().ToTable(nameof(UserClaims));
            builder.Entity<IdentityUserLogin<string>>().ToTable(nameof(UserLogins));
            builder.Entity<IdentityUserToken<string>>().ToTable(nameof(UserTokens));

            builder.Entity<IdentityRole>().ToTable(nameof(Roles));
            builder.Entity<IdentityUserRole<string>>().ToTable(nameof(UserRoles));
            builder.Entity<IdentityRoleClaim<string>>().ToTable(nameof(RoleClaims));

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
