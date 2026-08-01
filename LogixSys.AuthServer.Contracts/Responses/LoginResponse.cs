namespace LogixSys.AuthServer.Contracts.Responses;

public sealed class LoginResponse
{
    public bool Success { get; init; }

    public string? UserId { get; init; }

    public string? UserName { get; init; }

    public IReadOnlyList<string> Roles { get; init; }
        = Array.Empty<string>();

    public string? Message { get; init; }
}