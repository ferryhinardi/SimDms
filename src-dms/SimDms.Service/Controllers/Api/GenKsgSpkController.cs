using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using System.Transactions;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class GenKsgSpkController : BaseController
    {
        private const string DataId = "WFRES";
        private string msg = "";
        

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                FullName = CurrentUser.FullName
            });
        }

        public JsonResult Save(List<GenKSGSPKSave> model, string ReceiptNo, DateTime ReceiptDate)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Tidak / belum ada data yang dipilih" });
            }

            string generateNo = "";
            foreach (var row in model) generateNo += string.Format(",'{0}{1}'", row.BranchCode, row.GenerateNo);
            if (generateNo.Length > 0) generateNo = generateNo.Substring(1);

            using (TransactionScope transScope = new TransactionScope())
            {
                try
                {
                    SvTrnPdiFscBatch oPdifscBatch = new SvTrnPdiFscBatch();
                    oPdifscBatch.CompanyCode = CompanyCode;
                    oPdifscBatch.BranchCode = BranchCode;
                    oPdifscBatch.ProductType = ProductType;
                    oPdifscBatch.BatchDate = DateTime.Now;
                    oPdifscBatch.ReceiptNo = ReceiptNo;
                    oPdifscBatch.ReceiptDate = ReceiptDate;
                    oPdifscBatch.BatchNo = GetNewDocumentNo("BAT", oPdifscBatch.BatchDate.Value);
                    oPdifscBatch.CreatedBy = CurrentUser.UserId;
                    oPdifscBatch.CreatedDate = DateTime.Now;
                    oPdifscBatch.LastUpdateBy = CurrentUser.UserId;
                    oPdifscBatch.LastUpdateDate = DateTime.Now;
                    ctx.SvTrnPdiFscBatchs.Add(oPdifscBatch);
                    var result = ctx.SaveChanges() > 0;

                    string sql = string.Format(@"
                                    declare @CompanyCode varchar(15)
                                    declare @ProductType varchar(15) 
                                    declare @BatchNo varchar(15) 
                                    declare @UserID varchar(15) 

                                    set @CompanyCode = '{0}'
                                    set @ProductType = '{1}'
                                    set @BatchNo = '{2}'
                                    set @UserID = '{3}'

                                    update svTrnPdiFsc set
                                     BatchNo=@BatchNo
                                    ,PostingFlag=3
                                    ,LastUpdateBy=@UserID
                                    ,LastUpdateDate=getdate()
                                    where 1 = 1
                                     and CompanyCode=@CompanyCode
                                     and ProductType=@ProductType
                                     and BranchCode+''+GenerateNo in ({4}) 
                                    ", CompanyCode, ProductType, oPdifscBatch.BatchNo, CurrentUser.UserId, generateNo);

                    ctx.Database.ExecuteSqlCommand(sql);
                    transScope.Complete();

                    return Json(new { success = result, data = oPdifscBatch, message = "Data Saved" });

                }
                catch(Exception ex)
                {
                    transScope.Dispose();
                }
            }
            return Json(new { success = false, message = "Gagal Simpan data"});

        }

        public JsonResult CreateWFRES(string BatchNo)
        {
            PdiFscHdrFile header = GetPdiFscHdrFile(BatchNo);
            List<PdiFscDtlFile> details = GetPdiFscDtlFiles(BatchNo, (!IsBranch == true) ? "" : BranchCode);

            string data = header.Text;
            foreach (PdiFscDtlFile detail in details) data = data + "\n" + detail.Text;
            
            var WFRES = new byte[data.Length * sizeof(char)];
            Buffer.BlockCopy(data.ToCharArray(), 0, WFRES, 0, WFRES.Length);
            Session.Add("WFRES", WFRES);

            return Json(new { success = true, contents = data });
        }

        public FileContentResult GetWFRES()
        {
            var file = Session["WFRES"] as byte[];

            Session.Clear();

            var ms = new MemoryStream(file);
            string contentType = "application/text";

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=WFRES.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();

            return File(file, contentType,"WFRES.txt");
        }

        public JsonResult SendFile(string Contents)
        {
            DcsWsSoapClient ws = new DcsWsSoapClient();

            //var file = Session[type] as byte[];
            string header = Contents.Split('\n')[0];

            var msg = "";

            Session.Clear();

            try
            {
                string result = ws.SendToDcs(DataId, CompanyCode, Contents, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(DataId, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", DataId);
                return Json(new { success = true, message = msg });
            }
            catch
            {
                msg = string.Format("{0} gagal digenerate", DataId);
                return Json(new { success = false, message = msg });
            }
        }

        public JsonResult ValidateHeaderFile(string Contents)
        {
            var result = false;
            var msg = "";

            string header = Contents.Split('\n')[0];

            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", DataId, header);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", DataId, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }

        public JsonResult CheckBatchNo(string BatchNo)
        {
            var data = ctx.SvTrnPdiFscBatchs.Find(CompanyCode, BranchCode, ProductType, BatchNo);
            if (data != null) return Json(true);
            else throw new Exception(null);
        }

        public JsonResult GetPdiFscBatchFromSPK(string BatchNo)
        {
            var query = string.Format(@"select
            IsSelected = 1
            ,a.BranchCode+''+substring(CompanyName,patindex('%-%',CompanyName),len(CompanyName)-patindex('%-%',CompanyName)+1) as BranchData
            ,a.BranchCode
            ,a.GenerateNo
            ,a.GenerateDate
            ,a.RefferenceNo
            ,RefferenceDate = (case a.RefferenceDate when '19000101' then null else a.RefferenceDate end)
            ,a.SenderDealerCode
            ,a.SenderDealerName
            ,a.TotalNoOfItem
            ,a.TotalLaborAmt
            ,a.TotalMaterialAmt
            ,a.TotalAmt
            from svTrnPdiFsc a
                left join gnMstCoProfile b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
            where 1 = 1
             and a.CompanyCode = '{0}'
             and a.BranchCode = '{1}'
             and a.ProductType = '{2}'
             and a.BatchNo = '{3}'
            ", CompanyCode, BranchCode, ProductType, BatchNo);

            var data = ctx.Database.SqlQuery<GetKSGSPK>(query);

            return Json(data);
        }

        public JsonResult SelectPdiFscFromSPK()
        {
            var query = string.Format(@"
                    select IsSelected = 0
                    ,a.BranchCode+' '+substring(BranchName,patindex('%-%',BranchName),len(BranchName)-patindex('%-%',BranchName)+1) as BranchData
                    ,a.BranchCode
                    ,a.GenerateNo
                    ,a.GenerateDate
                    ,a.RefferenceNo
                    ,RefferenceDate = (case a.RefferenceDate when '19000101' then null else a.RefferenceDate end)
                    ,a.SenderDealerCode
                    ,a.SenderDealerName
                    ,a.TotalNoOfItem
                    ,a.TotalLaborAmt
                    ,a.TotalMaterialAmt
                    ,a.TotalAmt
                    from svTrnPdiFsc a
	                    left join gnMstOrganizationDtl b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
                    where 1 = 1
                     and a.CompanyCode = '{0}'
                     and (case '{1}' when '' then '' else a.BranchCode end) = '{1}'
                     and a.ProductType = '{2}'
                     and a.PostingFlag in (0,1,2)
                     and (a.BatchNo is null or a.BatchNo = '')
                     and a.TotalNoOfItem > 0
                     and a.FromInvoiceNo like 'SPK%' 
                     and a.ToInvoiceNo like 'SPK%'
                    ", CompanyCode, BranchCode, ProductType);

            var data = ctx.Database.SqlQuery<GetKSGSPK>(query);

            return Json(data);
        }

        public PdiFscHdrFile GetPdiFscHdrFile(string batchNo)
        {
            string sql = string.Format(@"
                    declare @CompanyCode varchar(15)
                    declare @BranchCode varchar(15)
                    declare @ProductType varchar(15) 
                    declare @BatchNo varchar(15) 

                    set @CompanyCode = '{0}'
                    set @BranchCode = '{1}'
                    set @ProductType = '{2}'
                    set @BatchNo = '{3}'

                    declare @flagSender bit
                    set @flagSender = ISNULL((select ParaValue from gnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'WFRES' and LookupValue = '1'),0) 

                    select
                     RecordID = 'H'
                    ,DataID = '{4}'
                    ,case when @FlagSender = 0 then @CompanyCode 
                     else isnull((select LockingBy from gnMstCoProfileService where CompanyCode = @CompanyCode and BranchCode = @BranchCode), @BranchCode)
                     end DealerCode
                    ,RcvDealerCode = isnull((
                        select cus.StandardCode
                          from svMstBillingType bil
                         inner join gnMstCustomer cus
                            on bil.CompanyCode  = cus.CompanyCode
                           and bil.CustomerCode = cus.CustomerCode
                         where 1 = 1
                           and bil.CompanyCode  = a.CompanyCode
                           and bil.BillType     = 'F'
	                    ),'')
                    ,DealerName = isnull((
	                    select top 1 BranchName from gnMstOrganizationDtl
	                    where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode
	                    ),'')
                    ,TotalItem = isnull((
	                    select sum(TotalNoOfItem) from svTrnPdiFsc
	                    where PostingFlag = 3
	                    and CompanyCode = a.CompanyCode
	                    and BranchCode  = a.BranchCode
	                    and ProductType = a.ProductType
	                    and BatchNo = a.BatchNo
	                    ),0)
                    ,isnull(a.ReceiptNo,'''')ReceiptNo
                    ,a.ReceiptDate
                    ,a.ProductType
                    ,a.IsCampaign
                     from svTrnPdiFscBatch a
                    where 1 = 1
                      and a.CompanyCode = @CompanyCode
                      and a.BranchCode  = @BranchCode
                      and a.ProductType = @ProductType
                      and a.BatchNo     = @BatchNo
                    ", CompanyCode, BranchCode, ProductType, batchNo, DataId);

            PdiFscHdrFile file = new PdiFscHdrFile();
            var data = ctx.Database.SqlQuery<PdiFscHdrFileModel>(sql).First();
            if (data != null)
            {
                file.DataID = data.DataID;
                file.DealerCode = data.DealerCode;
                file.RcvDealerCode = data.RcvDealerCode;
                file.DealerName = data.DealerName;
                file.TotalItem = (int)data.TotalItem;
                file.ReceiptNo = data.ReceiptNo;
                file.ReceiptDate = data.ReceiptDate.Value;
                file.ProductType = data.ProductType;
                file.IsCampaign = data.IsCampaign;
            }
            return file;
        }

        public List<PdiFscDtlFile> GetPdiFscDtlFiles(string batchNo, string branchCode)
        {
            string sql = string.Format(@"
                    declare @CompanyCode varchar(15)
                    declare @BranchCode varchar(15)
                    declare @ProductType varchar(15) 
                    declare @BatchNo varchar(15) 

                    set @CompanyCode = '{0}'
                    set @BranchCode = '{1}'
                    set @ProductType = '{2}'
                    set @BatchNo = '{3}'

                    select 
                     a.BatchNo
                    ,a.GenerateNo
                    ,b.GenerateSeq
                    ,b.ServiceBookNo
                    ,b.BasicModel
                    ,b.TransmissionType
                    ,b.ChassisCode
                    ,b.ChassisNo
                    ,b.EngineCode
                    ,b.EngineNo
                    ,b.PdiFsc
                    ,b.ServiceDate
                    ,b.DeliveryDate
                    ,b.RegisteredDate
                    ,b.Odometer
                    ,b.LaborAmount
                    ,b.MaterialAmount
                    ,b.PdiFscAmount
                    from svTrnPdiFsc a
                    left join svTrnPdiFscApplication b on 1 = 1
                     and b.CompanyCode = a.CompanyCode
                     and b.BranchCode = a.BranchCode
                     and b.ProductType = a.ProductType
                     and b.GenerateNo = a.GenerateNo
                    where 1 = 1
                     and a.CompanyCode = @CompanyCode
                     and (case @BranchCode when '' then '' else a.BranchCode end) = @BranchCode
                     and a.ProductType = @ProductType
                     and a.BatchNo = @BatchNo
                    order by b.BasicModel, b.PdiFsc, b.ServiceDate
                    ", CompanyCode, branchCode, ProductType, batchNo);

            List<PdiFscDtlFile> files = new List<PdiFscDtlFile>();
            var data = ctx.Database.SqlQuery<PdiFscDtlFileModel>(sql);
            foreach (var x in data)
            {
                PdiFscDtlFile file = new PdiFscDtlFile();
                file.ServiceBookNo = x.ServiceBookNo;
                file.BasicModel = x.BasicModel;
                file.TransmissionType = x.TransmissionType;
                //file.ChassisCode = x.ChassisCode;
                //file.ChassisNo = x.ChassisNo.ToString();
                //file.EngineCode = x.EngineCode;
                //file.EngineNo = x.EngineNo.ToString();
                file.ChassisCodeChassisNo = x.ChassisCode + x.ChassisNo.ToString();
                file.EngineCodeEngineNo = x.EngineCode + x.EngineNo.ToString();
                file.FS = x.PdiFsc.ToString();
                file.ServiceDate = x.ServiceDate.Value;
                if (x.DeliveryDate != null)
                {
                    file.DeliveryDate = x.DeliveryDate.Value;
                }
                else
                {
                    file.DeliveryDate = new DateTime(1900, 1, 1);
                }
                file.RegisteredDate = x.RegisteredDate.Value;
                file.Odometer = x.Odometer.Value;
                file.LaborAmount = x.LaborAmount.Value;
                file.MaterialAmount = x.MaterialAmount.Value;
                file.TotalAmount = x.PdiFscAmount.Value;
                files.Add(file);
            }

            return files;
        }

        public partial class PdiFscHdrFile
        {
            #region -- Initialize --

            private string line = "";

            public PdiFscHdrFile()
            {
                string text = "HWFREE";
                line = "#" + text.PadRight(176, ' ');
            }

            public PdiFscHdrFile(string text)
            {
                line = "#" + text.PadRight(176, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID
            {
                get { return GetString(2, 5); }
                set { SetValue(2, 5, value); }
            }

            public string DealerCode
            {
                get { return GetString(7, 10); }
                set { SetValue(7, 10, value); }
            }

            public string RcvDealerCode
            {
                get { return GetString(17, 10); }
                set { SetValue(17, 10, value); }
            }

            public string DealerName
            {
                get { return GetString(27, 50); }
                set { SetValue(27, 50, value); }
            }

            public int TotalItem
            {
                get { return GetInt32(77, 6); }
                set { SetValue(77, 6, value); }
            }

            public string PaymentNumber
            {
                get { return GetString(83, 15); }
                set { SetValue(83, 15, value); }
            }

            public string ReceiptNo
            {
                get { return GetString(83, 15); }
                set { SetValue(83, 15, value); }
            }

            public DateTime PaymentDate
            {
                get { return GetDate(98, 8); }
                set { SetValue(98, 8, value); }
            }

            public DateTime ReceiptDate
            {
                get { return GetDate(98, 8); }
                set { SetValue(98, 8, value); }
            }

            public string ProductType
            {
                get { return GetString(106, 1); }
                set
                {
                    string val = value;
                    if (val == "2W") val = "A";
                    if (val == "4W") val = "B";
                    if (val == "OB") val = "C";
                    SetValue(106, 1, val);
                }
            }

            public bool IsCampaign
            {
                get { return GetString(107, 1).Equals("Y"); }
                set { SetValue(107, 1, value); }
            }

            public string BlankFilter
            {
                get { return GetString(108, 33); }
            }

            public string Text { get { return line.Substring(1); } }

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

            private void SetValue(int start, int length, string value)
            {
                string a = line.Substring(0, start);
                string b = value.PadRight(length, ' ').Substring(0, length);
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }

            private void SetValue(int start, int length, int value)
            {
                string a = line.Substring(0, start);
                string b = value.ToString().PadLeft(length, '0').Substring(0, length);
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }

            private void SetValue(int start, int length, DateTime value)
            {
                if (length == 8)
                {
                    string a = line.Substring(0, start);
                    string b = value.ToString("yyyMMdd");
                    string c = line.Substring(start + length);
                    if (b == "19000101") b = "        ";
                    line = string.Format("{0}{1}{2}", a, b, c);
                }
            }

            private void SetValue(int start, int length, bool value)
            {
                if (length == 1)
                {
                    string a = line.Substring(0, start);
                    string b = value ? "Y" : "N";
                    string c = line.Substring(start + length);
                    line = string.Format("{0}{1}{2}", a, b, c);
                }
            }

            #endregion
        }

        public partial class PdiFscDtlFile
        {
            #region -- Initialize --

            private string line = "";
            private string remark = "";

            public PdiFscDtlFile()
            {
                string text = "1";
                line = "#" + text.PadRight(176, ' ');
            }

            public PdiFscDtlFile(string text)
            {
                line = "#" + text.PadRight(176, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string ServiceBookNo
            {
                get { return GetString(2, 10); }
                set { SetValue(2, 10, value); }
            }

            public string BasicModel
            {
                get { return GetString(12, 15); }
                set { SetValue(12, 15, value); }
            }

            public string TransmissionType
            {
                get { return GetString(27, 2); }
                set { SetValue(27, 2, value); }
            }

            #region ** ChassisCode+ChassisNo **
            //public string ChassisCode
            //{
            //    get { return GetString(29, 15); }
            //    set { SetValue(29, 15, value); }
            //}

            //public string ChassisNo
            //{
            //    get { return GetString(44, 10); }
            //    set { SetValue(44, 10, value); }
            //}

            public string ChassisCodeChassisNo
            {
                get { return GetString(29, 25); }
                set { SetValue(29, 25, value); }
            }
            #endregion

            #region ** EngineCode+EngineNo **
            //public string EngineCode
            //{
            //    get { return GetString(54, 15); }
            //    set { SetValue(54, 15, value); }
            //}

            //public string EngineNo
            //{
            //    get { return GetString(69, 10); }
            //    set { SetValue(69, 10, value); }
            //}

            public string EngineCodeEngineNo
            {
                get { return GetString(54, 25); }
                set { SetValue(54, 25, value); }
            }

            #endregion

            public string FS
            {
                get { return GetString(79, 2); }
                set { SetValue(79, 2, Convert.ToInt32(value)); }
            }

            public DateTime ServiceDate
            {
                get { return GetDate(81, 8); }
                set { SetValue(81, 8, value); }
            }

            public DateTime DeliveryDate
            {
                get { return GetDate(89, 8); }
                set { SetValue(89, 8, value); }
            }

            public DateTime RegisteredDate
            {
                get { return GetDate(97, 8); }
                set { SetValue(97, 8, value); }
            }

            public decimal Odometer
            {
                get { return GetDecimal(105, 8); }
                set { SetValue(105, 8, value); }
            }

            public decimal LaborAmount
            {
                get { return GetDecimal(113, 9); }
                set { SetValue(113, 9, value); }
            }

            public decimal MaterialAmount
            {
                get { return GetDecimal(122, 9); }
                set { SetValue(122, 9, value); }
            }

            public decimal TotalAmount
            {
                get { return GetDecimal(131, 9); }
                set { SetValue(131, 9, value); }
            }

            public string RefferenceNo
            {
                get { return GetString(140, 15); }
                set { SetValue(140, 15, value); }
            }

            public string ReceiptNo
            {
                get { return GetString(155, 15); }
                set { SetValue(155, 15, value); }
            }

            public bool IsCampaign
            {
                get { return GetString(170, 1).Equals("Y"); }
                set { SetValue(170, 1, value); }
            }

            public string JudgementFlag
            {
                get { return GetString(171, 1); }
                set { SetValue(171, 1, value); }
            }

            public string BlankFiller { get { return GetString(172, 4); } }

            public string Remark
            {
                get { return remark.Trim(); }
                set { remark = value; }
            }

            public string Text { get { return line.Substring(1); } }

            #endregion

            #region -- Private Methods --

            private string GetString(int start, int length)
            {
                return line.Replace("\n", " ").Replace("\t", " ").Substring(start, length).Trim();
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

            private void SetValue(int start, int length, string value)
            {
                string a = line.Substring(0, start);
                string b = value.PadRight(length, ' ').Substring(0, length);
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }

            private void SetValue(int start, int length, int value)
            {
                string a = line.Substring(0, start);
                string b = value.ToString().PadLeft(length, '0').Substring(0, length);
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }

            private void SetValue(int start, int length, decimal value)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("#").PadLeft(length, '0').Substring(0, length);
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }

            private void SetValue(int start, int length, DateTime value)
            {
                if (length == 8)
                {
                    string a = line.Substring(0, start);
                    string b = value.ToString("yyyMMdd");
                    string c = line.Substring(start + length);
                    if (b == "19000101") b = "00000000";
                    line = string.Format("{0}{1}{2}", a, b, c);
                }
            }

            private void SetValue(int start, int length, bool value)
            {
                if (length == 1)
                {
                    string a = line.Substring(0, start);
                    string b = value ? "Y" : "N";
                    string c = line.Substring(start + length);
                    line = string.Format("{0}{1}{2}", a, b, c);
                }
            }

            #endregion
        }
    }
}