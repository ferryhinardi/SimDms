using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class UploadDataDealerController : BaseController
    {
        private DcsWsSoapClient ws = new DcsWsSoapClient();
        private bool status = false;
        private bool result = false;

        private string msg = "";

        public JsonResult Default()
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;

            try
            {
                status = ws.IsValid();
            }
            catch
            {
                status = false;
            }

            return Json(new
            {
                FirstPeriod = new DateTime(year, month, 1),
                EndPeriod = new DateTime(year, month, 1).AddMonths(1).AddDays(-1),
                stat = status,
                Status = status ? "Online" : "Offline"
            });
        }

        public JsonResult Retrieve(string DataID, DateTime FirstPeriod, DateTime EndPeriod, bool IsAll)
        {
            var data = RetrieveDownloadData(DataID, FirstPeriod, EndPeriod, IsAll);
            return Json(data);
        }

        public JsonResult Upload(string DataID, string Contents, long ID)
        {
            string[] lines = null;

            lines = Regex.Split(Contents, "\n");

            if (lines.Length <= 0)
            {
                msg ="flat file tidak ada data";
                return Json(new {success = false, message= msg});
            }

            string headerText = lines[0];
            if (headerText.Length < 140)
            {
                msg = "flat file text header < 140 karakter";
                                return Json(new {success = false, message= msg});

            }
            // Jika text detail < 110 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 140 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 140 karakter", i, lines[i].Length);
                    return Json(new { success = false, message = msg });
                }

            }

            result = UploadDataPSPAN(lines);

            result = UpdateDownloadDataStatus(ID, result);

            if (result)
            {
                return Json(new { success = result, message = "Proses Upload Berhasil" });
            }
            else
            {
                return Json(new { success = result, message = "Proses Upload Gagal" });
            }
        }

        private List<RetrieveSparepart> RetrieveDownloadData(string DataID, DateTime FirstPeriod, DateTime EndPeriod, bool IsAll)
        {
            var retrieveData = new List<RetrieveSparepart>();

            try
            {
                if (status)
                {
                    long id = (Int64)ctx.GnDcsDownloadFiles.Where(a => a.DataID == DataID).Max(a => a.ID);
                    
                    var custQuery = string.Format("exec uspfn_gnGetDcsDealerCode {0},{1},'{2}'",CompanyCode,BranchCode,DataID);
                    var custCode = ctx.Database.SqlQuery<string>(custQuery);

                    var data = ws.RetrieveDownloadByDataID(DataID, id);

                    if (data.Count > 0 && data[0].StartsWith("FAIL"))
                    {
                        throw new Exception(data[0].Substring(5));
                    }

                    foreach (var var in data)
                    {
                        string dcs = string.Empty;
                        string[] items = new string[8];
                        dcs = var;

                        string s = dcs + ",";
                        items = s.Split(',');
                        int year = Convert.ToInt32(items[3].Substring(0, 4));
                        int month = Convert.ToInt32(items[3].Substring(4, 2));
                        int day = Convert.ToInt32(items[3].Substring(6, 2));
                        int hour = Convert.ToInt32(items[3].Substring(9, 2));
                        int minute = Convert.ToInt32(items[3].Substring(12, 2));
                        int sec = Convert.ToInt32(items[3].Substring(15, 2));
                        DateTime time = new DateTime(year, month, day, hour, minute, sec);
                        items[3] = time.ToString();
                        items[7] = items[4].Split('\n')[0];

                        var record = new GnDcsDownloadFile();
                        record.ID = Convert.ToDecimal(items[0]);
                        record.DataID = items[1];
                        record.CustomerCode = items[2];
                        record.ProductType  = items[6].Substring(0, 1);
                        record.Contents     = items[4];
                        record.Status       = items[5];
                        record.CreatedDate  = Convert.ToDateTime(items[3]);
                        record.Header       = items[7];

                        ctx.GnDcsDownloadFiles.Add(record);
                        ctx.SaveChanges();
                    }
                }

                var query = "";

                if (IsAll)
                {
                    query = @"
select ID, DataID, CustomerCode, ProductType, Contents
     , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
     , CreatedDate, UpdatedDate, Header
  from gnDcsDownloadFile
 where 1 = 1
   and DataID = @p0
   and (convert(varchar, CreatedDate, 112) between convert(varchar, @p1, 112) and convert(varchar, @p2, 112)) 
   and ProductType in (@p3,@p4)
";
                }
                else
                {
                    query = @"
select ID, DataID, CustomerCode, ProductType, Contents
     , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
     , CreatedDate, UpdatedDate, Header
  from gnDcsDownloadFile
 where 1 = 1
   and DataID = @p0
   and (convert(varchar, CreatedDate, 112) between convert(varchar, @p1, 112) and convert(varchar, @p2, 112)) 
   and ProductType in (@p3,@p4)
   and Status = 'A'
";
                }

                var p4 = ProfitCenter == "300" ? (ProductType == "2W" ? "2" : "4") : (ProductType == "2W" ? "A" : "B");

                object[] parameters = { DataID, FirstPeriod, EndPeriod, (ProductType == "2W" ? "2" : "4"), p4 };

                retrieveData = ctx.Database.SqlQuery<RetrieveSparepart>(query, parameters).ToList();

            }
            catch (Exception ex)
            {
            }

            return retrieveData;
        }

        private bool UploadDataPSPAN(string[] lines)
        {
            int counter = 0;
            decimal num = 0;
            bool result = false;
            bool status = false;

            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                string query = "";

                PSPANHdrFile oSHISTHdrFile = new PSPANHdrFile(lines[0]);
                PSPANDtl1File oPSPANDtl1File = new PSPANDtl1File(lines[1]);

                num = lines.Length - 1;
                query = "";
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    string detailText = lines[i];
                    if (string.IsNullOrEmpty(detailText))
                    {
                        break;
                    }

                    if (status == false)
                    {
                        query += @"INSERT INTO [spHstSparePartAnalysis] ([CompanyCode],[BranchCode],[PeriodYear],[PeriodMonth],[TypeOfGoods],[JumlahJaringan],[PenjualanKotor],[PenjualanBersih],[HargaPokok],[PenerimaanPembelian],[NilaiStock],[ITO],[DemandLine],[DemandQuantity],[DemandNilai],[SupplyLine],[SupplyQuantity],[SupplyNilai],[ServiceRatioLine],[ServiceRatioQuantity],[ServiceRatioNilai],[DataStockMC4],[DataStockMC5],[SlowMoving],[CreatedBy],[CreatedDate],[LastUpdateBy],[LastUpdateDate])
     VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}') ";
                        status = true;
                        query = query.Replace("{0}", ccode);
                    }

                    if (detailText.Substring(0, 1) == "1")
                    {
                        oPSPANDtl1File = new PSPANDtl1File(detailText);
                        decimal ito = oPSPANDtl1File.HargaPokok != 0 ? oPSPANDtl1File.NilaiStock / oPSPANDtl1File.HargaPokok : 0;

                        query = query.Replace("{1}", oPSPANDtl1File.BranchCode);
                        query = query.Replace("{2}", oPSPANDtl1File.Year.ToString());
                        query = query.Replace("{3}", oPSPANDtl1File.Month.ToString());
                        query = query.Replace("{4}", oPSPANDtl1File.TypeOfGoods);
                        query = query.Replace("{5}", oPSPANDtl1File.JumlahJaringan);
                        query = query.Replace("{6}", oPSPANDtl1File.PenjualanKotor.ToString());
                        query = query.Replace("{7}", oPSPANDtl1File.PenjualanBersih.ToString());
                        query = query.Replace("{8}", oPSPANDtl1File.HargaPokok.ToString());
                        query = query.Replace("{9}", oPSPANDtl1File.PenerimaanPembelian.ToString());
                        query = query.Replace("{10}", oPSPANDtl1File.NilaiStock.ToString());
                        query = query.Replace("{11}", ito.ToString("n2").Replace(',', '.'));
                    }
                    else if (detailText.Substring(0, 1) == "2")
                    {
                        PSPANDtl2File oPSPANDtl2File = new PSPANDtl2File(detailText);
                        decimal pctLine = oPSPANDtl2File.DemandLine != 0 ? oPSPANDtl2File.SupplyLine / oPSPANDtl2File.DemandLine : 0;
                        decimal pctQty = oPSPANDtl2File.DemandQuantity != 0 ? oPSPANDtl2File.SupplyQuantity / oPSPANDtl2File.DemandQuantity : 0;
                        decimal pctNilai = oPSPANDtl2File.DemandNilai != 0 ? oPSPANDtl2File.SupplyNilai / oPSPANDtl2File.DemandNilai : 0;
                        decimal slow = oPSPANDtl2File.DataStockMC4 + oPSPANDtl2File.DataStockMC5;

                        query = query.Replace("{12}", oPSPANDtl2File.DemandLine.ToString());
                        query = query.Replace("{13}", oPSPANDtl2File.DemandQuantity.ToString());
                        query = query.Replace("{14}", oPSPANDtl2File.DemandNilai.ToString());
                        query = query.Replace("{15}", oPSPANDtl2File.SupplyLine.ToString());
                        query = query.Replace("{16}", oPSPANDtl2File.SupplyQuantity.ToString());
                        query = query.Replace("{17}", oPSPANDtl2File.SupplyNilai.ToString());
                        query = query.Replace("{18}", pctLine.ToString("n2").Replace(',', '.'));
                        query = query.Replace("{19}", pctQty.ToString("n2").Replace(',', '.'));
                        query = query.Replace("{20}", pctNilai.ToString("n2").Replace(',', '.'));
                        query = query.Replace("{21}", oPSPANDtl2File.DataStockMC4.ToString());
                        query = query.Replace("{22}", oPSPANDtl2File.DataStockMC5.ToString());
                        query = query.Replace("{23}", Convert.ToInt32(slow).ToString());
                        query = query.Replace("{24}", oPSPANDtl2File.CreatedBy);
                        query = query.Replace("{25}", oPSPANDtl2File.CreatedDate.ToShortDateString());
                        query = query.Replace("{26}", CurrentUser.UserId);
                        query = query.Replace("{27}", DateTime.Now.ToShortDateString());

                        status = false;
                    }

                    if (!query.Contains("{"))
                    {
                        if (counter > 200)
                        {
                            if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                            {
                                result = false;
                                return result;
                            }
                            query = string.Empty; counter = 0;
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(query)))
                {
                    if (!(ctx.Database.ExecuteSqlCommand(query) > 0))
                        result = false;
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private bool UpdateDownloadDataStatus(long id, bool success)
        {
            string status = (success) ? "P" : "X";
            var query = string.Format("update gnDcsDownloadFile set Status = '{0}', UpdatedDate = getdate() where ID = '{1}'", status, id);
            int i = ctx.Database.SqlQuery<int>(query).FirstOrDefault();

            ws.UpdateDownloadDataStatus(id, status);
            return i > 0 ? true : false;
        }

        public JsonResult DataID()
        {
            List<object> listOfDataID = new List<object>();
            listOfDataID.Add(new { value = "PSPAN", text = "PSPAN" });
            return Json(listOfDataID);
        }

        private class RetrieveSparepart
        {
            public decimal ID { get; set; }
            public string DataID { get; set; }
            public string CustomerCode { get; set; }
            public string ProductType { get; set; }
            public string Contents { get; set; }
            public string Status { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string Header { get; set; }
        }

        #region Upload File PSPAN

        public partial class PSPANHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public PSPANHdrFile(string text)
            {
                line = "#" + text.PadRight(140, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string SubmitterDealerCode { get { return GetString(7, 10); } }

            public string ReceivedDealerCode { get { return GetString(17, 10); } }

            public string SubmitterDealerName { get { return GetString(27, 50); } }

            public string TotalNumberItem { get { return GetString(77, 6); } }

            public string ProductType { get { return GetString(83, 1); } }

            public string BlankFiller { get { return GetString(84, 57); } }

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

        public partial class PSPANDtl1File
        {
            #region -- Initialize --

            private string line = "";

            public PSPANDtl1File(string text)
            {
                line = "#" + text.PadRight(140, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public int Year { get { return GetInt32(2, 4); } }

            public int Month { get { return GetInt32(6, 2); } }

            public string BranchCode { get { return GetString(8, 15); } }

            public string TypeOfGoods { get { return GetString(23, 15); } }

            public string JumlahJaringan { get { return GetString(38, 9); } }

            public int PenjualanKotor { get { return GetInt32(47, 18); } }

            public int PenjualanBersih { get { return GetInt32(65, 18); } }

            public int HargaPokok { get { return GetInt32(83, 18); } }

            public int PenerimaanPembelian { get { return GetInt32(101, 18); } }

            public int NilaiStock { get { return GetInt32(119, 18); } }

            public string BlankFiller { get { return GetString(137, 4); } }


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

        public partial class PSPANDtl2File
        {
            #region -- Initialize --

            private string line = "";

            public PSPANDtl2File(string text)
            {
                line = "#" + text.PadRight(140, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public int DemandLine { get { return GetInt32(2, 9); } }

            public int DemandQuantity { get { return GetInt32(11, 12); } }

            public int DemandNilai { get { return GetInt32(23, 18); } }

            public int SupplyLine { get { return GetInt32(41, 9); } }

            public int SupplyQuantity { get { return GetInt32(50, 12); } }

            public int SupplyNilai { get { return GetInt32(62, 18); } }

            public int DataStockMC4 { get { return GetInt32(80, 18); } }

            public int DataStockMC5 { get { return GetInt32(98, 18); } }

            public string CreatedBy { get { return GetString(116, 15); } }

            public DateTime CreatedDate { get { return GetDate(131, 8); } }

            public string BlankFiller { get { return GetString(139, 2); } }


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
    }
}
