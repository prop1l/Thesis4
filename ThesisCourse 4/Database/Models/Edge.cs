using System;
using System.Collections.Generic;

namespace ThesisCourse_4.Database.Models;

public partial class Edge
{
    public int Id { get; set; }

    public int? GraphId { get; set; }

    public int FromNodeId { get; set; }

    public int ToNodeId { get; set; }

    public virtual Node FromNode { get; set; } = null!;

    public virtual Graph? Graph { get; set; }

    public virtual Node ToNode { get; set; } = null!;
}
