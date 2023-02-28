using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using PhotographyBlog.Data;

namespace PhotographyBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly PhotographyBlogContext dbContext;
        private readonly IAmazonS3 amazoneS3;
        private string username = "admin";
        private string password = "tuananh00";
        public AdminController(PhotographyBlogContext dbContext, IAmazonS3 amazoneS3)
        {
            this.dbContext = dbContext;
            this.amazoneS3 = amazoneS3;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login( string uname, string pword)
        {
            if(uname== this.username && pword== this.password)
            {
                return View("AdminMenu");
            }else
                return Content("Login Fail! Try Again");
        }

        public IActionResult Albums() { 
            List<Album> albums = new List<Album>();
            albums = dbContext.Albums.OrderBy(o=>o.ViewOrder).ToList();

            return View(albums);
        }
        [HttpGet]
        public IActionResult AlbumCreate() {
            return View("AlbumCreate");
        }
        [HttpPost]
        public IActionResult AlbumCreate(string albumName, string albumDescription, bool active)
        {
            Album album = new Album()
            {
                Name = albumName,
                Description = albumDescription,
                Active = active,
                TimeCreate = DateTime.Now.ToString()
            };
            dbContext.Albums.Add(album);
            dbContext.SaveChanges();
            return Albums();

        }
    }
}
