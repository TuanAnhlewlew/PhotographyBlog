using System;
using System.Collections.Generic;

namespace PhotographyBlog.Data;

/// <summary>
/// Visitor List
/// </summary>
public partial class Visitor
{
    public int VisitorId { get; set; }

    public string Name { get; set; } = null!;

    public string? TimeFirstVisit { get; set; }

    public string? TimeLastVisit { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();
}
