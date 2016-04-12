using SimDms.Common;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.BLL
{
    public class UploadBLL
    {
        public DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

        public static bool Validate(string[] lines, UploadType uploadType)
        {
            switch (uploadType)
            {
                case UploadType.WCAMP:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WCAMP"));
                case UploadType.WFRAT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WFRAT"));
                case UploadType.WJUDG:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WJUDG"));
                case UploadType.WPDFS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WPDFS"));
                case UploadType.WSECT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WSECT"));
                case UploadType.WTROB:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WTROB"));
                case UploadType.WWRNT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WWRNT"));
                default:
                    return false;
            }
        }

        public enum UploadType
        {
            WJUDG, WTROB, WSECT, WFRAT, WWRNT, WCAMP, WPDFS, WFREE, WFRMB, WCLAM, WCMRB
        }
    }
}