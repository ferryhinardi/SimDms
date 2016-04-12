using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.SUtility.Models
{
    [Table("DMS_UPLOAD")]
    public class DmsUpload
    {
        [Key]
        public decimal ID { get; set; }

        [Column("DATA_ID")]
        public string DataID { get; set; }

        [Column("CUSTOMER_CODE_BILLING")]
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

    [Table("DmsUploadFileView")]
    public class DmsUploadFileView
    {
        [Key]
        public decimal ID { get; set; }

        [Column("DATA_ID")]
        public string DataID { get; set; }

        [Column("CUSTOMER_CODE_BILLING")]
        public string CustomerCodeBilling{ get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }

        [Column("STATUS")]
        public string Status { get; set; }

        [Column("PRODUCT_TYPE")]
        public string ProductType { get; set; }

        [Column("CLOB_CONTENT")]
        public string ClobContent { get; set; }

        [Column("DEALER_NAME")]
        public string DealerName { get; set; }

        [Column("SHORT_NAME")]
        public string ShortName { get; set; }
    }
}