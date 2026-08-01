using LogixSys.AuthServer.Application.Interfaces;
using LogixSys.AuthServer.Domain.Authentication;
using LogixSys.AuthServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LogixSys.AuthServer.Persistence.Repositories;

public sealed class LegacyAuthenticationRepository
    : ILegacyAuthenticationRepository
{
    private readonly LegacyIdentityDbContext _context;

    public LegacyAuthenticationRepository(
        LegacyIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<LegacyUser?> FindByUserNameAsync(
     string username,
     CancellationToken cancellationToken = default)
    {
        var entity = await _context.AspNetUsers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                u => u.UserName == username,
                cancellationToken);

        if (entity is null)
            return null;

        return new LegacyUser
        {
            Id = entity.Id,
            UserName = entity.UserName,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            SecurityStamp = entity.SecurityStamp,
            LockoutEnabled = entity.LockoutEnabled,
            AccessFailedCount = entity.AccessFailedCount,
            Disabled = entity.Disabled ?? false
        };
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.AspNetUserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);
    }
}