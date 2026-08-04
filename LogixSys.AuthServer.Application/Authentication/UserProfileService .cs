using LogixSys.AuthServer.Application.Interfaces;

namespace LogixSys.AuthServer.Application.Authentication;

public sealed class UserProfileService : IUserProfileService
{
    private readonly ILegacyAuthenticationRepository _repository;

    public UserProfileService(
        ILegacyAuthenticationRepository repository)
    {
        _repository = repository;
    }

    public Task<UserProfile?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetUserProfileAsync(
            userId,
            cancellationToken);
    }
}