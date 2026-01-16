using System;
using System.Collections.Generic;

namespace ThesisCourse_4.Database.Models;

public partial class Graph
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Edge> Edges { get; set; } = new List<Edge>();

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    public virtual User User { get; set; } = null!;
}
