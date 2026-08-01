using System;
using System.Collections.Generic;

namespace LogixSys.AuthServer.Persistence.Entities;

public partial class AspNetUserRole
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public DateTime? LastUpdated { get; set; }

    public virtual AspNetRole Role { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
