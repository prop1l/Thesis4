using System;
using System.Collections.Generic;

namespace ThesisCourse_4.Database.Models;

public partial class Node
{
    public int Id { get; set; }

    public int? GraphId { get; set; }

    public string Label { get; set; } = null!;

    public double XPosition { get; set; }

    public double YPosition { get; set; }

    public virtual ICollection<Edge> EdgeFromNodes { get; set; } = new List<Edge>();

    public virtual ICollection<Edge> EdgeToNodes { get; set; } = new List<Edge>();

    public virtual Graph? Graph { get; set; }
}
