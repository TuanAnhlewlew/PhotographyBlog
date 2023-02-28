using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using PhotographyBlog.Data;
using PhotographyBlog.Models;
using System.Diagnostics;

namespace PhotographyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly PhotographyBlogContext dbContext;
        private readonly IAmazonS3 amazoneS3;
        public HomeController(PhotographyBlogContext dbContext, IAmazonS3 amazoneS3)
        {
            this.dbContext = dbContext;
            this.amazoneS3 = amazoneS3;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> upload([FromForm] List<IFormFile> images)
        {
            try
            {
                foreach( IFormFile image in images)
                {
                    var putRequest = new PutObjectRequest()
                    {
                        BucketName = "photoblogstorage",
                        Key = "imgs/"+image.FileName,
                        InputStream = image.OpenReadStream(),
                        ContentType = image.ContentType
                    };
                    await this.amazoneS3.PutObjectAsync(putRequest);
                }
            }catch(Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok();
        }
    }
}