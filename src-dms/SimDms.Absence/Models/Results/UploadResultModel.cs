using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models.Results
{
    public class UploadResultModel
    {
        public bool status { get; set; }
        public string fileID { get; set; }
        public string fileName { get; set; }
        public string fileSize { get; set; }
        public int size { get; set; }
        public string uploadedBy { get; set; }
        public DateTime? uploadedDate { get; set; }
    }
}