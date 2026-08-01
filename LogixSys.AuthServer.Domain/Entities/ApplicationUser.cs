using Microsoft.AspNetCore.Identity;

namespace LogixSys.AuthServer.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}