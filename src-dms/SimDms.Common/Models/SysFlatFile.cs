using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Common.Models
{
    [Table("sysFlatFileHdr")]
    public class SysFlatFileHdr
    {
        [Key]
        [Column(Order = 1)]
        public string CodeID { get; set; }
        [Column(Order = 2)]
        public int SeqNo { get; set; }
        public int Position { get; set; }
        public int FieldLength { get; set; }
        public string FieldName { get; set; }
        public string FieldDesc { get; set; }
        public string FieldFormat { get; set; }

        [NotMapped]
        public string FieldValue { get; set; }
    }

    [Table("sysFlatFileDtl")]
    public class SysFlatFileDtl
    {
        [Key]
        [Column(Order = 1)]
        public string CodeID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int DetailID { get; set; }
        [Key]
        [Column(Order = 3)]
        public int SeqNo { get; set; }
        public int Position { get; set; }
        public int FieldLength { get; set; }
        public string FieldName { get; set; }
        public string FieldDesc { get; set; }
        public string FieldFormat { get; set; }
    }
}
