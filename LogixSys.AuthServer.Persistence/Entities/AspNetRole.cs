using System;
using System.Collections.Generic;

namespace LogixSys.AuthServer.Persistence.Entities;

public partial class AspNetRole
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Discriminator { get; set; } = null!;

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();
}
