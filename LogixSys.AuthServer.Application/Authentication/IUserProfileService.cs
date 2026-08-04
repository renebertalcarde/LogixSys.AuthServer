namespace LogixSys.AuthServer.Application.Authentication;

public interface IUserProfileService
{
    Task<UserProfile?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
}