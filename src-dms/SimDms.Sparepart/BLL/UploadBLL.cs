using SimDms.Common;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class UploadBLL
    {
        public DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

        public static bool Validate(string[] lines, UploadType uploadType)
        {
            switch (uploadType)
            {
                case UploadType.PINVD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PINVD"));
                case UploadType.PINVS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PINVS"));
                case UploadType.PORDD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PORDD"));
                case UploadType.PORDS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PORDS"));
                case UploadType.TSTKD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("TSTKD"));
                case UploadType.PPRCD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PPRCD"));
                case UploadType.PMODP:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMODP"));
                case UploadType.PMDLM:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMDLM"));
                case UploadType.MSMDL:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("MSMDL"));
                case UploadType.SHIST:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SHIST"));
                default:
                    return false;
            }
        }

        public enum UploadType
        {
            PINVD, PINVS, PORDD, PORDS, TSTKD, PPRCD, PMODP, PMDLM, MSMDL, SHIST
        }
    }


}
