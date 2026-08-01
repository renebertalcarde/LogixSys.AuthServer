using LogixSys.AuthServer.Domain.Authentication;

namespace LogixSys.AuthServer.Application.Interfaces;

public interface ILegacyAuthenticationRepository
{
    Task<LegacyUser?> FindByUserNameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(
        string userId,
        CancellationToken cancellationToken = default);
}