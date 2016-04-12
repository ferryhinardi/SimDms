using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sales.Models;
using SimDms.Common.Models;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Data.SqlClient;

namespace SimDms.Sales.BLL
{
    public class UploadBLL
    {
        public DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

        public static bool Validate(string[] lines, UploadType uploadType)
        {
            switch (uploadType)
            {
                case UploadType.SPORD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SPORD"));
                case UploadType.SDORD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SDORD"));
                case UploadType.SSJAL:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SSJAL"));
                case UploadType.SHPOK:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SHPOK"));
                case UploadType.SACCS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SACCS"));
                case UploadType.SFPO1:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPO1"));
                case UploadType.SFPO2:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPO2"));
                case UploadType.SPRIC:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SPRIC"));
                case UploadType.SFPLB:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPLB"));
                case UploadType.SFPLA:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPLA"));
                case UploadType.SFPLR:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPLR"));
                case UploadType.FAPIO:
                    return (lines.Length > 0 && lines[0].Split(';')[1].Equals("FAPIO"));
                case UploadType.SUADE:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SUADE"));
                default:
                    return false;
            }
        }

        public enum UploadType
        {
            SPORD, SDORD, SSJAL, SHPOK, SACCS, SFPO1, SFPO2, SPRIC, SFPLB, SFPLA, SFPLR, FAPIO, SUADE
        }

        #region SPORD

        public partial class SPORDHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SPORDHdrFile(string text)
            {
                line = "#" + text.PadRight(260, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 178); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SPORDDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SPORDDtl1File(string text)
            {
                line = "#" + text.PadRight(260, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SKPNo { get { return GetString(2, 15); } }

            public DateTime SKPDate { get { return GetDate(17, 8); } }

            public string BlankFilter { get { return GetString(25, 236); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SPORDDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SPORDDtl2File(string text)
            {
                line = "#" + text.PadRight(260, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public int SalesModelYear { get { return GetInt32(17, 4); } }

            public decimal BeforeDiscDPP { get { return GetDecimal(21, 18); } }

            public decimal BeforeDiscPPN { get { return GetDecimal(39, 18); } }

            public decimal BeforeDiscPPNBM { get { return GetDecimal(57, 18); } }

            public decimal BeforeDiscTotal { get { return GetDecimal(75, 18); } }

            public decimal DiscountExcludePPN { get { return GetDecimal(93, 18); } }

            public decimal DiscountIncludePPN { get { return GetDecimal(111, 18); } }

            public decimal AfterDiscDPP { get { return GetDecimal(129, 18); } }

            public decimal AfterDiscPPN { get { return GetDecimal(147, 18); } }

            public decimal AfterDiscPPNBM { get { return GetDecimal(165, 18); } }

            public decimal AfterDiscTotal { get { return GetDecimal(183, 18); } }

            public decimal PPNBMPaid { get { return GetDecimal(201, 18); } }

            public decimal OthersDPP { get { return GetDecimal(219, 18); } }

            public decimal OthersPPN { get { return GetDecimal(237, 18); } }

            public int Quantity { get { return GetInt32(255, 6); } }

            public string BlankFilter { get { return GetString(261, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SPORDDtl3File
        {
            #region -- Initialize --

            private string line = "";

            public SPORDDtl3File(string text)
            {
                line = "#" + text.PadRight(260, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ColourCode { get { return GetString(2, 15); } }

            public int Quantity { get { return GetInt32(17, 6); } }

            public string BlankFilter { get { return GetString(23, 238); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        } 
        #endregion

        #region SDORD

        public partial class SDORDHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SDORDHdrFile(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 178); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SDORDDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SDORDDtl1File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DONo { get { return GetString(2, 15); } }

            public DateTime DODate { get { return GetDate(17, 8); } }

            public string SKPNo { get { return GetString(25, 15); } }

            public string BlankFilter { get { return GetString(40, 57); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SDORDDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SDORDDtl2File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public int SalesModelYear { get { return GetInt32(17, 4); } }

            public int Quantity { get { return GetInt32(21, 6); } }

            public string BlankFilter { get { return GetString(27, 70); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SDORDDtl3File
        {
            #region -- Initialize --

            private string line = "";

            public SDORDDtl3File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ColourCode { get { return GetString(2, 15); } }

            public string ChassisCode { get { return GetString(17, 15); } }

            public int ChassisNo { get { return GetInt32(32, 10); } }

            public string EngineCode { get { return GetString(42, 15); } }

            public int EngineNo { get { return GetInt32(57, 10); } }

            public string ServiceBookNo { get { return GetString(67, 15); } }

            public string KeyNo { get { return GetString(82, 15); } }

            public string BlankFilter { get { return GetString(97, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SSJAL

        public partial class SSJALHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SSJALHdrFile(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 178); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SSJALDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SSJALDtl1File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SJNo { get { return GetString(2, 15); } }

            public DateTime SJDate { get { return GetDate(17, 8); } }

            public string SKPNo { get { return GetString(25, 15); } }

            public string DONo { get { return GetString(40, 15); } }

            public DateTime DODate { get { return GetDate(55, 8); } }

            public string IsBlokir { get { return GetString(63, 1); } }

            public string FlagRevisi { get { return GetString(64, 1); } }

            public string BlankFilter { get { return GetString(65, 33); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SSJALDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SSJALDtl2File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public int SalesModelYear { get { return GetInt32(17, 4); } }

            public int Quantity { get { return GetInt32(21, 6); } }

            public string BlankFilter { get { return GetString(27, 70); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SSJALDtl3File
        {
            #region -- Initialize --

            private string line = "";

            public SSJALDtl3File(string text)
            {
                line = "#" + text.PadRight(96, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ColourCode { get { return GetString(2, 15); } }

            public string ChassisCode { get { return GetString(17, 15); } }

            public int ChassisNo { get { return GetInt32(32, 10); } }

            public string EngineCode { get { return GetString(42, 15); } }

            public int EngineNo { get { return GetInt32(57, 10); } }

            public string ServiceBookNo { get { return GetString(67, 15); } }

            public string KeyNo { get { return GetString(82, 15); } }

            public string BlankFilter { get { return GetString(97, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SHPOK

        public partial class SHPOKHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKHdrFile(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 106); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SHPOKDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKDtl1File()
            {
            }

            public SHPOKDtl1File(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string InvoiceNo { get { return GetString(2, 15); } }

            public DateTime InvoiceDate { get { return GetDate(17, 8); } }

            public string SKPNo { get { return GetString(25, 15); } }

            public string FakturPajakNo { get { return GetString(40, 20); } }

            public DateTime FakturPajakDate { get { return GetDate(60, 8); } }

            public DateTime DueDate { get { return GetDate(68, 8); } }

            public string Remark { get { return GetString(76, 100); } }

            public string BlankFilter { get { return GetString(176, 13); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SHPOKDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKDtl2File()
            {
            }

            public SHPOKDtl2File(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DocNo { get { return GetString(2, 15); } }

            public string DocType { get { return GetString(17, 1); } }

            public string BlankFilter { get { return GetString(18, 171); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SHPOKDtl3File
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKDtl3File()
            {
            }

            public SHPOKDtl3File(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public int SalesModelYear { get { return GetInt32(17, 4); } }

            public int Quantity { get { return GetInt32(21, 6); } }

            public decimal BeforeDiscDPP { get { return GetDecimal(27, 18); } }

            public decimal DiscountExcludePPN { get { return GetDecimal(45, 18); } }

            public decimal AfterDiscDPP { get { return GetDecimal(63, 18); } }

            public decimal AfterDiscPPN { get { return GetDecimal(81, 18); } }

            public decimal AfterDiscPPNBM { get { return GetDecimal(99, 18); } }

            public decimal AfterDiscTotal { get { return GetDecimal(117, 18); } }

            public decimal PPNBMPaid { get { return GetDecimal(135, 18); } }

            public decimal OthersDPP { get { return GetDecimal(153, 18); } }

            public decimal OthersPPN { get { return GetDecimal(171, 18); } }

            public string BlankFilter { get { return GetString(189, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SHPOKDtl4File
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKDtl4File(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ColourCode { get { return GetString(2, 15); } }

            public string ChassisCode { get { return GetString(17, 15); } }

            public int ChassisNo { get { return GetInt32(32, 10); } }

            public string EngineCode { get { return GetString(42, 15); } }

            public int EngineNo { get { return GetInt32(57, 10); } }

            public string BlankFilter { get { return GetString(97, 122); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SHPOKDtlOFile
        {
            #region -- Initialize --

            private string line = "";

            public SHPOKDtlOFile(string text)
            {
                line = "#" + text.PadRight(188, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string OthersCode { get { return GetString(2, 15); } }

            public decimal OthersDPP { get { return GetDecimal(17, 18); } }

            public decimal OthersPPN { get { return GetDecimal(35, 18); } }

            public string BlankFilter { get { return GetString(53, 136); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SACCS

        public partial class SACCSHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SACCSHdrFile(string text)
            {
                line = "#" + text.PadRight(82, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SACCSDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SACCSDtl1File(string text)
            {
                line = "#" + text.PadRight(82, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string BPPNo { get { return GetString(2, 15); } }

            public DateTime BPPDate { get { return GetDate(17, 8); } }

            public string SJNo { get { return GetString(25, 15); } }

            public string BlankFilter { get { return GetString(40, 43); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SACCSDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SACCSDtl2File(string text)
            {
                line = "#" + text.PadRight(82, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string PerlengkapanCode { get { return GetString(2, 15); } }

            public int Quantity { get { return GetInt32(17, 6); } }

            public string BlankFilter { get { return GetString(23, 60); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SFPOL

        public partial class SFPOLHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SFPOLHdrFile(string text)
            {
                line = "#" + text.PadRight(162, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 7); } }

            public string RcvDealerCode { get { return GetString(14, 7); } }

            public string DealerName { get { return GetString(21, 50); } }

            public int TotalItem { get { return GetInt32(71, 6); } }

            public string BatchNo { get { return GetString(77, 6); } }

            public string BlankFilter { get { return GetString(83, 80); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPOLDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SFPOLDtl1File(string text)
            {
                line = "#" + text.PadRight(162, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string FakturPolisiNo { get { return GetString(2, 15); } }

            public string SalesModelCode { get { return GetString(17, 15); } }

            public int SalesModelYear { get { return GetInt32(32, 4); } }

            public string ColourCode { get { return GetString(36, 15); } }

            public string ChassisCode { get { return GetString(51, 15); } }

            public int ChassisNo { get { return GetInt32(66, 10); } }

            public string EngineCode { get { return GetString(76, 15); } }

            public int EngineNo { get { return GetInt32(91, 10); } }

            public string IsBlanko { get { return GetString(101, 1); } }

            public DateTime FakturPolisiDate { get { return GetDate(102, 8); } }

            public DateTime FakturPolisiProcess { get { return GetDate(110, 8); } }

            public string SJNo { get { return GetString(118, 15); } }

            public string DONo { get { return GetString(133, 15); } }

            public string ReqNo { get { return GetString(148, 15); } }

            public string BlankFilter { get { return GetString(163, 0); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SFPLB

        public partial class SFPLBHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBHdrFile(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 10); } }

            public string RcvDealerCode { get { return GetString(17, 10); } }

            public string DealerName { get { return GetString(27, 50); } }

            public int TotalNumberofItem { get { return GetInt32(77, 6); } }

            public string ProductType { get { return GetString(83, 1); } }

            public string BatchNo { get { return GetString(84, 5); } }

            public string Filler { get { return GetString(89, 3); } }

            public string BlankFiller { get { return GetString(92, 359); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPLBDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl1File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DeliveryOrder { get { return GetString(2, 8); } }

            public DateTime DeliveryOrderDate { get { return GetDate(10, 8); } }

            public string DeliveryOrderAtasNama { get { return GetString(18, 50); } }

            public string BlankFilter { get { return GetString(68, 383); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPLBDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl2File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SuratJalan { get { return GetString(2, 8); } }

            public DateTime SuratJalanDate { get { return GetDate(10, 8); } }

            public string SuratJalanAtasNama { get { return GetString(18, 50); } }

            public string ModelCode { get { return GetString(68, 15); } }

            public string SalesModelDescription { get { return GetString(83, 50); } }

            public string FpolisiModelDescription { get { return GetString(133, 50); } }

            public string ModelLine { get { return GetString(183, 50); } }

            public string OldDealerCode { get { return GetString(233, 8); } }

            public string BlankFilter { get { return GetString(241, 210); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPLBDtl3File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl3File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DealerClass { get { return GetString(2, 2); } }

            public string DealerName { get { return GetString(4, 50); } }

            public string NoSKPK { get { return GetString(54, 15); } }

            public string NoSuratPermohonan { get { return GetString(69, 15); } }

            public string SalesmanName { get { return GetString(84, 50); } }

            public string NamaSKPK { get { return GetString(134, 50); } }

            public string Alamat1SKPK { get { return GetString(184, 40); } }

            public string Alamat2SKPK { get { return GetString(224, 40); } }

            public string Alamat3SKPK { get { return GetString(264, 34); } }

            public string CityCode { get { return GetString(298, 4); } }

            public string TeleponNo1 { get { return GetString(302, 12); } }

            public string TeleponNo2 { get { return GetString(314, 12); } }

            public string HandPhoneNo { get { return GetString(326, 15); } }

            public string BirthdaySKPK { get { return GetString(341, 8); } }

            public string NamaSKPK2 { get { return GetString(349, 40); } }

            public string BlankFilter { get { return GetString(389, 62); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPLBDtl4File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl4File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string Nama { get { return GetString(2, 50); } }

            public string Alamat1 { get { return GetString(52, 40); } }

            public string Alamat2 { get { return GetString(92, 40); } }

            public string Alamat3 { get { return GetString(132, 34); } }

            public string PostCode { get { return GetString(166, 5); } }

            public string CityCode { get { return GetString(171, 4); } }

            public string Telepon1 { get { return GetString(175, 12); } }

            public string Telepon2 { get { return GetString(187, 12); } }

            public string HandPhone { get { return GetString(199, 15); } }

            public DateTime BirthdayFpol { get { return GetDate(214, 8); } }

            public string PostName { get { return GetString(222, 55); } }

            public string Nama2 { get { return GetString(277, 40); } }

            public string IDNO { get { return GetString(317, 40); } }

            public string IsProject { get { return GetString(357, 1); } }

            public string KodeKecamatan { get { return GetString(358, 7); } }

            public string BlankFilter { get { return GetString(365, 86); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SFPLBDtl5File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl5File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string FakturPolisiNo { get { return GetString(2, 8); } }

            public DateTime FakturPolisiDate { get { return GetDate(10, 8); } }

            public string KodeRangka { get { return GetString(18, 12); } }

            public int NoRangka { get { return GetInt32(30, 8); } }

            public string KodeMesin { get { return GetString(38, 12); } }

            public int NoMesin { get { return GetInt32(50, 8); } }

            public string ColorCode { get { return GetString(58, 3); } }

            public string ColorDescription { get { return GetString(61, 50); } }

            public int Year { get { return GetInt32(111, 4); } }

            public string ReasonCode { get { return GetString(115, 1); } }

            public string ReasonDescription { get { return GetString(116, 200); } }

            public string ProcessDate { get { return GetString(316, 8); } }

            public string IsCityTransport { get { return GetString(324, 1); } }

            public string ServiceBookNo { get { return GetString(325, 10); } }

            public string BlankFilter { get { return GetString(335, 116); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion
        }

        public partial class SFPLBDtl6File
        {
            #region -- Initialize --

            private string line = "";

            public SFPLBDtl6File(string text)
            {
                line = "#" + text.PadRight(450, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string JenisKelamin { get { return GetString(2, 5); } }

            public string TempatPembelianMotor { get { return GetString(7, 5); } }

            public string TPSOther { get { return GetString(12, 30); } }

            public string MotorYgPernahDipakai { get { return GetString(42, 5); } }

            public string SumberPembelian { get { return GetString(47, 5); } }

            public string SUPOthers { get { return GetString(52, 30); } }

            public string AsalPembelian { get { return GetString(82, 5); } }

            public string ASPOthers { get { return GetString(87, 30); } }

            public string InformasiSepedaMotor { get { return GetString(117, 5); } }

            public string SRIOthers { get { return GetString(122, 30); } }

            public string FaktorPentingSpdMotor { get { return GetString(152, 5); } }

            public string PendidikanTerakhir { get { return GetString(157, 5); } }

            public string PDKOthers { get { return GetString(162, 30); } }

            public string PenghasilanKeluarga { get { return GetString(192, 1); } }

            public string Filler1 { get { return GetString(193, 4); } }

            public string Pekerjaan { get { return GetString(197, 5); } }

            public string PEKOthers { get { return GetString(202, 30); } }

            public string MotorCycleFunction { get { return GetString(232, 5); } }

            public string USEOthers { get { return GetString(237, 30); } }

            public string CaraPembelian { get { return GetString(267, 5); } }

            public string LeasingYgDipakai { get { return GetString(272, 5); } }

            public string LSGOthers { get { return GetString(277, 30); } }

            public string JangkaWaktuKredit { get { return GetString(307, 1); } }

            public string Filler2 { get { return GetString(308, 4); } }

            public string JWKOthers { get { return GetString(312, 30); } }

            public string TypeKendaraan { get { return GetString(342, 5); } }

            public string BlankFilter { get { return GetString(347, 104); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion
        }

        #endregion

        #region FAPIO

        public partial class FAPIOHdrFile
        {
            #region -- Initialize --

            private string[] line;

            public FAPIOHdrFile(string text)
            {
                line = text.Split(';');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return line[0]; } }

            public string DataID { get { return line[1]; } }

            public string SupplierCode { get { return line[2]; } }

            public string RcvDealerCode { get { return line[3]; } }

            public string DealerName { get { return line[4]; } }

            public int TotalNumberofItem { get { return GetInt32(line[5]); } }

            #endregion

            #region -- Private Methods --

            private int GetInt32(string value)
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            #endregion

        }

        public partial class FAPIODtl1File
        {
            #region -- Initialize --

            private string[] line;

            public FAPIODtl1File()
            {
            }

            public FAPIODtl1File(string text)
            {
                line = text.Split(';');
            }

            #endregion

            #region -- Public Properties --

            public string ID { get { return line[0]; } }

            public string TransactionType { get { return line[1]; } }

            public string FakturNo { get { return line[2]; } }

            public DateTime FakturDate { get { return GetDate(line[3]); } }

            public string TaxNo { get { return line[4]; } }

            public DateTime TaxDate { get { return GetDate(line[5]); } }

            public decimal TotalDPP { get { return GetDecimal(line[6]); } }

            public decimal TotalPPN { get { return GetDecimal(line[7]); } }

            public decimal TotalInvoice { get { return GetDecimal(line[8]); } }

            public string TermOfPayment { get { return line[9]; } }

            public string Description { get { return line[10]; } }

            public int TotalNumberOfInquiry { get { return GetInt32(line[11]); } }

            #endregion

            #region -- Private Methods --
            private int GetInt32(string value)
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(string value)
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(string value)
            {
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class FAPIODtl2File
        {
            #region -- Initialize --

            private string[] line;

            public FAPIODtl2File(string text)
            {
                line = text.Split(';');
            }

            #endregion

            #region -- Public Properties --

            public string ID { get { return line[0]; } }

            public string MemoLineDescription { get { return line[1]; } }

            public DateTime DueDate { get { return GetDate(line[2]); } }

            public decimal NilaiDPP { get { return GetDecimal(line[3]); } }

            public decimal NilaiPPN { get { return GetDecimal(line[4]); } }

            public decimal NilaiInvoice { get { return GetDecimal(line[5]); } }

            public string Description { get { return line[6]; } }

            #endregion

            #region -- Private Methods --

            private decimal GetDecimal(string value)
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(string value)
            {
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region SUADE

        public partial class SUADEHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SUADEHdrFile(string text)
            {
                line = "#" + text.PadRight(110, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 10); } }

            public string RcvDealerCode { get { return GetString(17, 10); } }

            public string DealerName { get { return GetString(27, 50); } }

            public int TotalNumberOfRow { get { return GetInt32(77, 4); } }

            public string BlankFilter { get { return GetString(81, 30); } }

            #endregion

            #region -- Private Methods --

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            #endregion

        }

        public partial class SUADEDtl1File : UploadBLL
        {
            #region -- Initialize --

            private string line = "";

            public SUADEDtl1File()
            {
            }

            public SUADEDtl1File(string text)
            {
                line = "#" + text.PadRight(110, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string ID { get { return GetString(1, 1); } }

            public int Year { get { return GetInt32(2, 4); } }

            public int Month { get { return GetInt32(6, 2); } }

            public string TypeCode
            {
                get
                {
                    OmMstIndentMapping oOmMstIndentMapping = ctx.OmMstIndentMappings.Find(GetString(8, 20), GetString(28, 50));
                    if (oOmMstIndentMapping != null)
                        return oOmMstIndentMapping.GroupCode;
                    else
                        return GetString(8, 20);
                }
            }

            public string Variant
            {
                get
                {
                    OmMstIndentMapping oOmMstIndentMapping = ctx.OmMstIndentMappings.Find(GetString(8, 20), GetString(28, 50));
                    if (oOmMstIndentMapping != null)
                        return oOmMstIndentMapping.TypeCode;
                    else
                        return GetString(28, 50);
                }
            }

            public string MarketModelCode { get { return GetString(78, 20); } }

            public int ModelYear { get { return GetInt32(98, 4); } }

            public int TotalQuotaUnits { get { return GetInt32(102, 9); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SUADEDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SUADEDtl2File(string text)
            {
                line = "#" + text.PadRight(110, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string ID { get { return GetString(1, 1); } }

            public string ColourCode { get { return GetString(2, 15); } }

            public string ColourName { get { return GetString(17, 50); } }

            public int QuotaUnits { get { return GetInt32(67, 9); } }

            public string UnitStatus { get { return GetString(76, 1); } }

            public string ColourStatus { get { return GetString(77, 1); } }

            public string UpdateUser { get { return GetString(78, 15); } }

            public DateTime UpdateDate { get { return GetDate(93, 14); } }

            public string BlankFiller { get { return GetString(107, 4); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #region MASTER

        #region SMCLR

        public partial class SMCLRHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SMCLRHdrFile(string text)
            {
                line = "#" + text.PadRight(250, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 10); } }

            public string RcvDealerCode { get { return GetString(17, 10); } }

            public string DealerName { get { return GetString(27, 50); } }

            public int TotalItem { get { return GetInt32(77, 6); } }

            public string BatchNo { get { return GetString(83, 6); } }

            public string BlankFilter { get { return GetString(89, 162); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            #endregion
        }

        public partial class SMCLRDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public SMCLRDtl1File(string text)
            {
                line = "#" + text.PadRight(250, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public string SalesModelDesc { get { return GetString(17, 50); } }

            public string FakturPolisiDesc { get { return GetString(67, 50); } }

            public string BlankFilter { get { return GetString(117, 134); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            #endregion
        }

        public partial class SMCLRDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public SMCLRDtl2File(string text)
            {
                line = "#" + text.PadRight(250, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ColorCode { get { return GetString(2, 15); } }

            public string ColorCodeDesc { get { return GetString(17, 100); } }

            public string FakturPolisiColorDesc { get { return GetString(117, 100); } }

            public string BlankFilter { get { return GetString(217, 34); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            #endregion
        }

        #endregion

        #region SPRIC

        public partial class SPRICHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public SPRICHdrFile(string text)
            {
                line = "#" + text.PadRight(100, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode { get { return GetString(7, 10); } }

            public string RcvDealerCode { get { return GetString(17, 10); } }

            public string DealerName { get { return GetString(27, 50); } }

            public int TotalItem { get { return GetInt32(77, 6); } }

            public string BatchNo { get { return GetString(83, 6); } }

            public string BlankFilter { get { return GetString(89, 12); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        public partial class SPRICDtlFile
        {
            #region -- Initialize --

            private string line = "";

            public SPRICDtlFile(string text)
            {
                line = "#" + text.PadRight(100, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string SalesModelCode { get { return GetString(2, 15); } }

            public int SalesModelYear { get { return GetInt32(17, 4); } }

            public DateTime PriceStartDate { get { return GetDate(21, 8); } }

            public int ModelPriceDPP { get { return GetInt32(29, 12); } }

            public int ModelPricePPn { get { return GetInt32(41, 12); } }

            public int ModelPriceTotal { get { return GetInt32(53, 12); } }

            public int PPnBMPaid { get { return GetInt32(65, 12); } }

            public string BlankFilter { get { return GetString(77, 24); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Substring(start, length).Trim();
            }

            private int GetInt32(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return Int32.MinValue;
                }
            }

            private decimal GetDecimal(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }

            private DateTime GetDate(int start, int length)
            {
                string value = line.Substring(start, length);
                try
                {
                    int year = Convert.ToInt32(value.Substring(0, 4));
                    int month = Convert.ToInt32(value.Substring(4, 2));
                    int day = Convert.ToInt32(value.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }

            #endregion

        }

        #endregion

        #endregion

        public class BPUNobyReffFJ
        {
            public string BPUNo { get; set; }
            public string PONo { get; set; }
        }
    }
}