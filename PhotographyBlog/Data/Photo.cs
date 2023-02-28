using System;
using System.Collections.Generic;

namespace PhotographyBlog.Data;

/// <summary>
/// Store photo information
/// </summary>
public partial class Photo
{
    public int PhotoId { get; set; }

    public int AlbumId { get; set; }

    public string? Name { get; set; }

    public int ViewOrder { get; set; }

    public string TimeCreate { get; set; } = null!;

    public bool? Active { get; set; }

    public string? Link { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();
}
