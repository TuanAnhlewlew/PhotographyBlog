using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotographyBlog.Data;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using PhotographyBlog.ComModels;

namespace PhotographyBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly PhotographyBlogContext dbContext;
        private readonly IAmazonS3 amazoneS3;
        private readonly IConfiguration config;
        public AdminController(PhotographyBlogContext dbContext, IAmazonS3 amazoneS3,IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.amazoneS3 = amazoneS3;
            this.config = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login( string uname, string pword)
        {
            if(uname== this.config.GetValue<string>("Admin:username") && pword== this.config.GetValue<string>("Admin:password"))
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
            return RedirectToAction("Albums");
        }
        [HttpPost]
        public string AlbumSave(int[] order)
        {
            for(int i = 0; i<order.Count();i++)
            {
                Album? album = dbContext.Albums.Find(order[i]);
                if (album != null)
                {
                    album.ViewOrder = i+1;
                    dbContext.SaveChanges();
                }
            }
            return "saved success";
        }
        public IActionResult AlbumDelete(int id)
        {
            Album? album = dbContext.Albums.Find(id);
            if (album != null)
            {
                dbContext.Albums.Remove(album);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Albums");
        }
        public IActionResult AlbumEdit(int id)
        {
            Album? album = dbContext.Albums.Find(id);
            if (album != null)
                return View("AlbumEdit", album);
            else
                return Content("Album not exist!");
        }
        [HttpPost]
        public IActionResult AlbumEdit(int id, string albumName, string albumDescription, bool active)
        {
            Album? album = dbContext.Albums.Find(id);
            if (album != null)
            {
                album.Name = albumName;
                album.Description = albumDescription;
                album.Active = active;
                album.TimeLastEdit = DateTime.Now.ToString();
                dbContext.SaveChanges();    
                return RedirectToAction("Albums");
            }
            else
                return Content("Album not exist!");
        }
        public IActionResult Photos(int? AlbumId)
        {
            if(AlbumId == null)
            {
                List<Photo> photos = new List<Photo>();
                photos = dbContext.Photos.Include(a => a.Album).ToList();
                ViewBag.Sortable = false;
                return View("lstPhotos", photos);
            }
            else
            {
                List<Photo> photos = new List<Photo>();
                photos = dbContext.Photos.Where(p => p.AlbumId == AlbumId).OrderBy(a => a.ViewOrder).Include(a => a.Album).ToList();
                ViewBag.Sortable = true;
                return View("lstPhotos", photos);
            }

            
        }
        public IActionResult PhotoCreate()
        {
            List<Album> albums = new List<Album>();
            albums = dbContext.Albums.OrderBy(o => o.ViewOrder).ToList();
            return View("PhotoCreate",albums);
        }
        [HttpPost]
        public async Task<IActionResult> PhotoCreate([FromForm] List<IFormFile> imageFiles,int albumSelect, bool active)
        {
            try
            {
                foreach (IFormFile image in imageFiles)
                {
                    string link = this.config.GetValue<string>("AmazonS3:storageDomain") + "/" + this.config.GetValue<string>("AmazonS3:photoFolder") + "/" + image.FileName;
                    //upload image
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        using (Image img = Image.FromStream(memoryStream))
                        {
                            //resize to width 1024 keep aspect ratio
                            int width = 1920;
                            int height = (int)(img.Height*width/img.Width);
                            Image resized = (Image)(new Bitmap(img,new Size(width,height)));
                            using (MemoryStream ms = new MemoryStream())
                            {
                                resized.Save(ms,img.RawFormat);
                                var putRequest = new PutObjectRequest()
                                {
                                    BucketName = "photoblogstorage",
                                    Key = "imgs/" + image.FileName,
                                    InputStream = ms,
                                    ContentType = image.ContentType
                                };
                                await this.amazoneS3.PutObjectAsync(putRequest);
                            }
                        }
                    }
                    Photo photo = new Photo()
                    {
                        AlbumId = albumSelect,
                        TimeCreate = DateTime.Now.ToString(),
                        Active = active,
                        Link= link
                    };
                    dbContext.Photos.Add(photo);   
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return RedirectToAction("Photos");
        }
        public IActionResult PhotoEdit(int id)
        {
            Photo? photo = dbContext.Photos.Find(id);
            if (photo != null)
            {
                List<Album> albums = new List<Album>();
                albums = dbContext.Albums.OrderBy(o => o.ViewOrder).ToList();
                return View("PhotoEdit", new PhotoEditView{ Photo=photo,lstAlbum=albums});
            }
            else
            {
                return Content("photo not exist!");
            }
        }
        [HttpPost]
        public IActionResult PhotoEdit(int id, int albumSelect, bool active)
        {
            Photo? photo = dbContext.Photos.Find(id);
            if (photo != null)
            {
                photo.AlbumId = albumSelect;
                photo.Active = active;
                dbContext.SaveChanges();
                return RedirectToAction("Photos", new { AlbumId = albumSelect });
            }
            else
                return Content("Photo not exist!");
        }
        public IActionResult PhotoDelete(int id)
        {
            Photo? photo = dbContext.Photos.Find(id);
            if (photo != null)
            {
                int albumId = photo.PhotoId;
                dbContext.Photos.Remove(photo);
                dbContext.SaveChanges();
                return RedirectToAction("Photos",new { AlbumId = albumId});
            }
            else
                return Content("photo not exist!");
        }
        [HttpPost]
        public string PhotoSave(int[] order)
        {
            for (int i = 0; i < order.Count(); i++)
            {
                Photo? photo = dbContext.Photos.Find(order[i]);
                if (photo != null)
                {
                    photo.ViewOrder = i + 1;
                    dbContext.SaveChanges();
                }
            }
            return "saved success!";
        }
    }
}
