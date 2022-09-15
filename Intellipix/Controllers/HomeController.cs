using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Intellipix.Models;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Intellipix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public IActionResult Index()
        {
            var thumbnailsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Photos");
            var thumbnailsFolder = Directory.CreateDirectory(thumbnailsFolderPath);
            ViewBag.Thumbnails = Directory.EnumerateFiles(thumbnailsFolder.FullName).Select(Path.GetFileName);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file?.Length > 0)
            {
                if (!file.ContentType.StartsWith("image"))
                {
                    TempData["Message"] = "Only image files may be uploaded";
                }
                else
                {
                    try
                    {
                        var filename = Path.GetFileName(file.FileName);

                        // Save the original image into a "Photos" folder
                        var photosFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Photos");
                        var photosFolder = Directory.CreateDirectory(photosFolderPath);

                        using (var photo = System.IO.File.Create(Path.Combine(photosFolder.FullName, filename)))
                        {
                            await file.CopyToAsync(photo);
                        }

                        // Generate a thumbnail and save it to a "Thumbnails" folder
                        //var thumbnailsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Thumbnails");
                        //var thumbnailsFolder = Directory.CreateDirectory(thumbnailsFolderPath);

                        //using (var formFileStream = file.OpenReadStream())
                        //using (var sourceImage = Image.FromStream(formFileStream))
                        //{
                        //    var newWidth = 192;
                        //    var newHeight = (Int32)(1.0 * sourceImage.Height / sourceImage.Width * newWidth);
                        //    using (var destinationImage = new Bitmap(sourceImage, new Size(newWidth, newHeight)))
                        //    {
                        //        destinationImage.Save(Path.Combine(thumbnailsFolder.FullName, filename));
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        // In case something goes wrong
                        TempData["Message"] = ex.Message;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
