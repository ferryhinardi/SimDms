using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Document.Models
{
    [Table("DocImage")]
    public class DocImage
    {
        [Key]
        [Column(Order=1)]
        public string  ImageId { get; set; }
        public string MenuID { get; set; }
        public byte[] ImageData { get; set; }
        public string Caption { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadBy { get; set; }
    }
}