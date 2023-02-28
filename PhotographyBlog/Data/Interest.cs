using System;
using System.Collections.Generic;

namespace PhotographyBlog.Data;

public partial class Interest
{
    public int VisitorId { get; set; }

    public int? PhotoId { get; set; }

    public int? AlbumId { get; set; }

    public string? Time { get; set; }

    public virtual Album? Album { get; set; }

    public virtual Photo? Photo { get; set; }

    public virtual Visitor Visitor { get; set; } = null!;
}
