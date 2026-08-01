namespace LogixSys.AuthServer.Application.Models;

public sealed class LegacyUser
{
    public string Id { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;

    public string? SecurityStamp { get; init; }

    public string? Email { get; init; }

    public bool LockoutEnabled { get; init; }

    public int AccessFailedCount { get; init; }
}