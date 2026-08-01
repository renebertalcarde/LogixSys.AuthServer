namespace LogixSys.AuthServer.Application.Authentication;

public sealed class AuthenticationResult
{
    public bool Success { get; init; }

    public string? UserId { get; init; }

    public string? UserName { get; init; }

    public IReadOnlyList<string> Roles { get; init; }
        = Array.Empty<string>();

    public string? Error { get; init; }

    public static AuthenticationResult Failed(
        string error)
    {
        return new()
        {
            Success = false,
            Error = error
        };
    }
}