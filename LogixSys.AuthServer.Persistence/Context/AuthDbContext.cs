using LogixSys.AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogixSys.AuthServer.Persistence.Context;

public class AuthDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AuthDbContext(
        DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
               .ToTable("AspNetUsers");

        builder.Entity<ApplicationRole>()
               .ToTable("AspNetRoles");

        builder.Entity<IdentityUserRole<string>>()
               .ToTable("AspNetUserRoles");

        builder.Entity<IdentityUserClaim<string>>()
               .ToTable("AspNetUserClaims");

        builder.Entity<IdentityUserLogin<string>>()
               .ToTable("AspNetUserLogins");

        builder.Entity<IdentityRoleClaim<string>>()
               .ToTable("AspNetRoleClaims");

        builder.Entity<IdentityUserToken<string>>()
               .ToTable("AspNetUserTokens");
    }
}