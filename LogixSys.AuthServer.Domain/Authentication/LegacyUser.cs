namespace LogixSys.AuthServer.Domain.Authentication;

public sealed class LegacyUser
{
    public string Id { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? PasswordHash { get; init; }

    public string? SecurityStamp { get; init; }

    public bool LockoutEnabled { get; init; }

    public int AccessFailedCount { get; init; }

    public bool Disabled { get; init; }
}