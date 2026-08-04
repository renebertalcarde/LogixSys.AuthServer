namespace LogixSys.AuthServer.Application.Authentication;

public sealed class UserProfile
{
    public string UserId { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public bool Disabled { get; init; }

    public IReadOnlyList<string> Roles { get; init; } =
        Array.Empty<string>();
}