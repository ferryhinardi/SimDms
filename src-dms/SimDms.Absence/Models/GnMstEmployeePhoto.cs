using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Absence.Models
{
    [Table("GnMstEmployeeDocument")]
    public class GnMstEmployeeDocument
    {
        [Key]
        public string DocumentID { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public byte[] DocumentContent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}