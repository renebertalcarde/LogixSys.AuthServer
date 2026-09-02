using System.Threading.Tasks;

namespace LogixSys.AuthClient.Services;

public record AuthResult(bool IsError, string? Error, string? UserName, string? AccessToken);

public interface IAuthenticationService
{
    Task<AuthResult> LoginAsync();
    Task LogoutAsync();
}
