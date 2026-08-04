using LogixSys.AuthServer.Application.Authentication;
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

    public async Task<UserProfile?> GetUserProfileAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _context.AspNetUsers
            .AsNoTracking()
            .Include(u => u.AspNetUserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken);

        if (user == null)
            return null;

        return new UserProfile
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Disabled = user.Disabled ?? false,

            Roles = user.AspNetUserRoles
                .Select(r => r.Role.Name)
                .OrderBy(r => r)
                .ToList()
        };
    }
}