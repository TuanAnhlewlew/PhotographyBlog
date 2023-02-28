using System;
using System.Collections.Generic;

namespace PhotographyBlog.Data;

/// <summary>
/// Store Album information
/// </summary>
public partial class Album
{
    public int AlbumId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? TimeCreate { get; set; }

    public string? TimeLastEdit { get; set; }

    public int ViewCount { get; set; }

    public int ViewOrder { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Photo> Photos { get; } = new List<Photo>();
}
