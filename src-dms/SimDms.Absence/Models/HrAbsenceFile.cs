using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("HrAbsenceFile")]
    public class HrAbsenceFile
    {
        [Key]
        [Column(Order = 1)]
        public string FileID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileContent { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class HrAbsenceFileView
    {
        public string FileID { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public int MyProperty { get; set; }
    }
}