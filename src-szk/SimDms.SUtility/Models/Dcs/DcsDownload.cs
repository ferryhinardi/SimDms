using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models.Dcs
{
    [Table("DCS_DOWNLOAD_FILE", Schema = "DCS")]
    public class DcsDownload
    {
        [Key]
        [Column("ID")]
        public decimal ID { get; set; }
        [Column("DATA_ID")]
        public string DataID { get; set; }
        [Column("CUSTOMER_CODE")]
        public string CustomerCode { get; set; }
        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }
        [Column("STATUS")]
        public string Status { get; set; }
        [Column("PRODUCT_TYPE")]
        public string ProductType { get; set; }
        [Column("CLOB_CONTENT")]
        public string ClobContent { get; set; }
    }
}