using PhotographyBlog.Data;

namespace PhotographyBlog.ComModels
{
    public class PhotoEditView
    {
        public Photo Photo { get; set; }
        public List<Album> lstAlbum { get; set; }
    }
}
