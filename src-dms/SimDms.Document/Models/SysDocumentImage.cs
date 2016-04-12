using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Document.Models
{
    [Table("SysDocumentImage")]
    public class SysDocumentImage
    {
        [Key]
        [MaxLength(50)]
        public string ImageId { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        public DateTime? UploadedDate { get; set; }

        public string UploadBy { get; set; }

        [Required]
        [MaxLength(10)]
        public string MimeType { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}