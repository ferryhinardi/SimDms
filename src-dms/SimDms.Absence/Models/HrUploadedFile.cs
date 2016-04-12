using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    [Table("HrUploadedFile")]
    public class HrUploadedFile
    {
        [Key]
        [Column(Order=1)]
        public string Checksum { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] Contents { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? UploadedDate { get; set; }
    }
}