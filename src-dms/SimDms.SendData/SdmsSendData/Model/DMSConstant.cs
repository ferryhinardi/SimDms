using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sim.Dms.SendData.Model
{
    public class DMSConstant{
        //Configuration Name
        public const string UrlSendDataSchedule = "UrlSendDataSchedule";
        public const string UrlTokenAccess = "UrlTokenAccess";

        //Object parameter post
        public const string UniqueID = "UniqueID";
        public const string CompanyCode = "CompanyCode";
        public const string DataType = "DataType";
        public const string Segment = "Segment";
        public const string Data = "Data";
        public const string LastSendDate = "LastSendDate";
        public const string ComputerName = "ComputerName";
        public const string TokenID = "TokenID";

        //DMS Status data
        public const char Success = 'S';
        public const char Fail = 'F';
    }

}
