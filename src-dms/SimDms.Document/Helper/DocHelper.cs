using System;
using System.Web.Helpers;

namespace SimDms.Document.Helper
{
    public static class DocHelper
    {
        public static ImageData ConvertFromBase64String(string imageData)
        {
            var delimiter = imageData.IndexOf(',');
            var header = imageData.Substring(0, delimiter);
            var data = imageData.Substring(delimiter + 1);
            var datatype = header.Substring(5, 5);
            if (datatype != "image")
            {
                return null;
            }
            var start = header.IndexOf('/') + 1;
            var len = (header.IndexOf(';') - start) - 1;
            var mime = header.Substring(start, len);
            var imageByte = Convert.FromBase64String(data);
            var wImage = new WebImage(imageByte);
            var model = new ImageData()
            {
                Image = imageByte,
                MimeType = mime,
                Width = wImage.Width,
                Height = wImage.Height
            };
            return model;
        }
    }

    public class ImageData
    {
        public byte[] Image { get; set; }
        public string MimeType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}