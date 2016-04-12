using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Document.Helper;
using SimDms.Document.Models;

namespace SimDms.Document.Controllers
{    
    public class ImageController : BaseController
    {
        private const string NoImagePath = "~/assets/img/none.jpg";
        public FileResult GetImage(string id)
        {
            if (!ctx.SysDocumentImages.Any(a => a.ImageId == id))
                return File(HttpContext.Server.MapPath(NoImagePath), "jpg");
            var image = ctx.SysDocumentImages.First(a => a.ImageId == id);
            return File(image.ImageData, image.MimeType);
        }

        public ActionResult ImageForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(string imageData)
        {
            if (string.IsNullOrEmpty(imageData)) return Json("none");
            var image = DocHelper.ConvertFromBase64String(imageData);
            var newImage = new SysDocumentImage()
            {
                Height = image.Height,
                Width = image.Width,
                ImageData = image.Image,
                ImageId = Guid.NewGuid().ToString(),
                MimeType = image.MimeType,
                UploadedDate = DateTime.Now
            };
            ctx.SysDocumentImages.Add(newImage);
            return Json(ctx.SaveChanges() == 0 ? "none" : newImage.ImageId);
        }
    }
}
