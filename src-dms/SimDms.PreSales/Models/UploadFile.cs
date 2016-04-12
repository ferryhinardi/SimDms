using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class UploadFile
    {
        public long? SeqNo { get; set; }
        public int InquiryNumber { get; set; }
        public long? NewInquiryNumber { get; set; }
        public string Status { get; set; }
    }
}