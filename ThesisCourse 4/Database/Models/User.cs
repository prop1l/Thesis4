using System;
using System.Collections.Generic;

namespace ThesisCourse_4.Database.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? AvatarId { get; set; }

    public virtual Avatar? Avatar { get; set; }

    public virtual ICollection<Graph> Graphs { get; set; } = new List<Graph>();
}
