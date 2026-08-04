using Microsoft.EntityFrameworkCore;

namespace LogixSys.AuthServer.Persistence.Context;

public sealed class OAuthDbContext : DbContext
{
    public OAuthDbContext(
        DbContextOptions<OAuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.UseOpenIddict();
    }
}