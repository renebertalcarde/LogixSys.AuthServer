using LogixSys.AuthServer.Application.Interfaces;

namespace LogixSys.AuthServer.Application.Authentication;

public sealed class AuthenticationService
    : IAuthenticationService
{
    private readonly ILegacyAuthenticationRepository _repository;
    private readonly ILegacyPasswordHasher _passwordHasher;


    public AuthenticationService(
        ILegacyAuthenticationRepository repository,
        ILegacyPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }


    public async Task<AuthenticationResult> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        LogixSys.AuthServer.Domain.Authentication.LegacyUser? user = 
            await _repository.FindByUserNameAsync(username, cancellationToken);


        if (user == null)
        {
            return AuthenticationResult.Failed(
                "Invalid username or password.");
        }


        if (user.Disabled)
        {
            return AuthenticationResult.Failed(
                "User account is disabled.");
        }


        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return AuthenticationResult.Failed(
                "User has no password configured.");
        }


        var valid =
            _passwordHasher.Verify(
                user.PasswordHash,
                password);


        if (!valid)
        {
            return AuthenticationResult.Failed(
                "Invalid username or password.");
        }


        var roles =
            await _repository.GetRolesAsync(
                user.Id,
                cancellationToken);


        return new AuthenticationResult
        {
            Success = true,
            UserId = user.Id,
            UserName = user.UserName,
            Roles = roles
        };
    }
}