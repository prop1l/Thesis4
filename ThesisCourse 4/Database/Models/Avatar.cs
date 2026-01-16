using System;
using System.Collections.Generic;

namespace ThesisCourse_4.Database.Models;

public partial class Avatar
{
    public int Id { get; set; }

    public byte[] ImageData { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
