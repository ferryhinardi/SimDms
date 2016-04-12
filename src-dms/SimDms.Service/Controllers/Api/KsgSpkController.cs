using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using ClosedXML.Excel;
using System.IO;
using System.Web.Script.Serialization;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class KsgSpkController : BaseController
    {
        public JsonResult Default()
        {
            /* GET DATA BRANCH HOLDING */
            var isUserHolding = false;
            var dataHolding = ctx.OrganizationDtls.Where(x => x.CompanyCode == CompanyCode && x.IsBranch == false);

            GnMstLookUpDtl oGnMstLookUpDtl =   ctx.GnMstLookUpDtls.Find(CompanyCode, "SRV_FLAG", "KSG_HOLDING");
            if (oGnMstLookUpDtl != null)
            {
                if (oGnMstLookUpDtl.ParaValue == "1")
                {
                    if (dataHolding.Count() == 1)
                    {
                        var row = dataHolding.FirstOrDefault();
                        isUserHolding = true;

                        if (row.BranchCode != BranchCode)
                        {
                            isUserHolding = false;
                            return Json(new { isEnable = false, message = "User bukan termasuk user Holding!", IsUserHolding = isUserHolding });
                        }
                    }
                }
                else isUserHolding = false;
            }
            else
            {
                if (dataHolding.Count() == 1)
                {
                    var row = dataHolding.FirstOrDefault();
                    isUserHolding = true;
                    if (row.BranchCode != BranchCode)
                    {
                        isUserHolding = false;
                        return Json(new { isEnable = false, message = "User bukan termasuk user Holding!", IsUserHolding = isUserHolding });
                    }
                }
            }
            
            var defaultTime = DateTime.Now;
            var coProfileService = ctx.CoProfileServices.Find(CompanyCode, BranchCode);
            var gDate = coProfileService.TransDate.Value;
            var endDate = coProfileService.PeriodEnd;

            if (DateTime.Now.Date.CompareTo(endDate) == 1)
            {
                defaultTime = endDate.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);
            }

            return Json(new
            {
                isEnable = true,
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ServiceType = 2,
                GenerateDate = defaultTime,
                PeriodeDateTo = defaultTime,
                IsBranch = IsBranch,
                PeriodeDateFrom = new DateTime(endDate.Year, endDate.Month, 1),
                BranchFrom = BranchCode,
                BranchTo = BranchCode,
                isPDI = true,
                IsUserHolding = isUserHolding
            });
        }


        public ActionResult Save(string data, FscFromSPKModel modelFSC)
        {
            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                DateTime transdate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<PdiFscSave> model = ser.Deserialize<List<PdiFscSave>>(data);
                string parJobOrder = "";
                string parBranch = "";
                string OldBranch = "";

                for (int i = 0; i < model.Count; i++)
                {
                    if (parJobOrder == "")
                    {
                        parJobOrder = "''" + model[i].JobOrderNo + "''";
                    }
                    else
                    {
                        parJobOrder = parJobOrder + ",''" + model[i].JobOrderNo + "''";
                    }
                    if (parBranch == "")
                    {
                        parBranch = "''" + model[i].BranchCode + "''";
                    }
                    else
                    {
                        if (model[i].BranchCode != OldBranch)
                        {
                            parBranch = parBranch + ",''" + model[i].BranchCode + "''";
                        }
                    }
                    OldBranch = model[i].BranchCode;
                }

                string jobFSC = modelFSC.IsFSC == true ? "%FSC%" : "";
                string jobPDI = modelFSC.IsPDI == true ? "%PDI%" : "";

                var qry = "EXEC uspfn_SvTrnListKsgFromSPKPerJobNo '" + CompanyCode + "','" + ProductType + "','" + modelFSC.BranchFrom + "', '" + modelFSC.BranchTo + "','" +
                    modelFSC.PeriodeDateFrom + "','" + modelFSC.PeriodeDateTo + "','" + jobPDI + "','" + jobFSC + "',' " + BranchCode + "', '" + parBranch + "','" + parJobOrder + "'";
                List<PdiFscSave2> dFsc = ctx.Database.SqlQuery<PdiFscSave2>(qry).ToList();

                LookUpHdr gnMstLookUpHdr = ctx.LookUpHdrs.Find(CompanyCode, "SRV_FLAG");
                string branchCode, paraValueKSG = string.Empty;
                if (gnMstLookUpHdr != null)
                {
                    paraValueKSG = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "SRV_FLAG" && p.LookUpValue == "KSG_HOLDING").FirstOrDefault().ParaValue;
                    if (paraValueKSG == "1")
                    {
                        branchCode = BranchCode;
                    }
                    else
                        branchCode = model[0].BranchCode;
                }
                else
                    branchCode = model[0].BranchCode;


                var newDocNo = GetNewDocumentNo(branchCode);

                PdiFsc oPdiFsc = new PdiFsc();
                oPdiFsc.CompanyCode = CompanyCode;
                oPdiFsc.BranchCode = branchCode;
                oPdiFsc.ProductType = ProductType;
                oPdiFsc.GenerateNo = newDocNo;
                oPdiFsc.GenerateDate = transdate;
                oPdiFsc.SourceData = "0";
                oPdiFsc.FromInvoiceNo = dFsc[0].JobOrderNo;
                oPdiFsc.ToInvoiceNo = dFsc[dFsc.Count - 1].JobOrderNo;
                oPdiFsc.FPJNo = "";
                oPdiFsc.FPJDate = new DateTime(1900, 1, 1);
                oPdiFsc.FPJGovNo = "";
                oPdiFsc.SenderDealerCode = CompanyCode;
                oPdiFsc.ReceiverDealerCode = CompanyCode;
                oPdiFsc.SenderDealerName = CompanyName;
                oPdiFsc.RefferenceNo = "";
                oPdiFsc.RefferenceDate = new DateTime(1900, 1, 1);
                oPdiFsc.TotalNoOfItem = dFsc.Count;
                oPdiFsc.IsCampaign = false;
                oPdiFsc.PostingFlag = "2";
                oPdiFsc.TotalLaborAmt = 0;
                oPdiFsc.TotalMaterialAmt = 0;
                oPdiFsc.TotalAmt = 0;

                for (int j = 0; j < dFsc.Count; j++)
                {
                    oPdiFsc.TotalLaborAmt += dFsc[j].LaborGrossAmt;
                    //== null ? 0 : Convert.ToDecimal(data[j].LaborGrossAmt);
                    oPdiFsc.TotalMaterialAmt += dFsc[j].MaterialGrossAmt;
                    //== null ? 0 : Convert.ToDecimal(data[j].MaterialGrossAmt);
                    oPdiFsc.TotalAmt += dFsc[j].PdiFscAmount;
                    //== null ? 0 : Convert.ToDecimal(data[j].PdiFscAmount);
                }
                oPdiFsc.TotalLaborPaymentAmt = 0;
                oPdiFsc.TotalMaterialPaymentAmt = 0;
                oPdiFsc.TotalPaymentAmt = 0;
                oPdiFsc.BatchNo = "";
                oPdiFsc.IsLocked = false;
                oPdiFsc.LockingBy = "";
                oPdiFsc.LockingDate = new DateTime(1900, 1, 1);
                oPdiFsc.CreatedBy = CurrentUser.UserId;
                oPdiFsc.CreatedDate = transdate;
                oPdiFsc.LastupdateBy = CurrentUser.UserId;
                oPdiFsc.LastupdateDate = transdate;
                if (ctx.PdiFscs.Add(oPdiFsc) != null)
                {
                    for (int k = 0; k < dFsc.Count; k++)
                    {
                        PdiFscApplication oPdiFscApp = new PdiFscApplication();
                        oPdiFscApp.CompanyCode = oPdiFsc.CompanyCode;
                        oPdiFscApp.BranchCode = oPdiFsc.BranchCode;
                        oPdiFscApp.ProductType = oPdiFsc.ProductType;
                        oPdiFscApp.GenerateNo = oPdiFsc.GenerateNo;
                        oPdiFscApp.GenerateSeq = k + 1;
                        oPdiFscApp.BranchCodeInv = dFsc[k].BranchCode.ToString();
                        oPdiFscApp.InvoiceNo = dFsc[k].JobOrderNo.ToString();
                        oPdiFscApp.PaymentNo = "";
                        oPdiFscApp.PaymentDate = new DateTime(1900, 1, 1);
                        oPdiFscApp.SuzukiRefferenceNo = "";
                        oPdiFscApp.JudgementFlag = "";
                        oPdiFscApp.PdiFscStatus = "0";
                        oPdiFscApp.ServiceBookNo = dFsc[k].ServiceBookNo.ToString();
                        oPdiFscApp.BasicModel = dFsc[k].BasicModel.ToString();
                        oPdiFscApp.TransmissionType = dFsc[k].TransmissionType.ToString();
                        oPdiFscApp.ChassisCode = dFsc[k].ChassisCode.ToString();
                        oPdiFscApp.ChassisNo = Convert.ToDecimal(dFsc[k].ChassisNo);
                        oPdiFscApp.EngineCode = dFsc[k].EngineCode.ToString();
                        oPdiFscApp.EngineNo = Convert.ToDecimal(dFsc[k].EngineNo);

                        oPdiFscApp.PdiFsc = dFsc[k].PdiFscSeq;
                        //== null ? 0 : Convert.ToDecimal(data[k].PdiFscSeq);

                        oPdiFscApp.ServiceDate = dFsc[k].JobOrderDate.ToString().Trim() != "" ? Convert.ToDateTime(dFsc[k].JobOrderDate) : new DateTime(1900, 1, 1);
                        oPdiFscApp.DeliveryDate = dFsc[k].BPKDate.ToString().Trim() != "" ? Convert.ToDateTime(dFsc[k].BPKDate) : new DateTime(1900, 1, 1);
                        oPdiFscApp.RegisteredDate = dFsc[k].FakturPolisiDate.ToString().Trim() != "" ? Convert.ToDateTime(dFsc[k].FakturPolisiDate) : new DateTime(1900, 1, 1);
                        oPdiFscApp.Odometer = Convert.ToDecimal(dFsc[k].Odometer);
                        oPdiFscApp.LaborAmount = dFsc[k].LaborGrossAmt;
                        //== null ? 0 : Convert.ToDecimal(data[k].LaborGrossAmt);
                        oPdiFscApp.MaterialAmount = dFsc[k].MaterialGrossAmt;
                        //== null ? 0 : Convert.ToDecimal(data[k].MaterialGrossAmt);
                        oPdiFscApp.PdiFscAmount = dFsc[k].PdiFscAmount;
                        //== null ? 0 : Convert.ToDecimal(data[k].PdiFscAmount);
                        oPdiFscApp.CreatedBy = oPdiFsc.CreatedBy;
                        oPdiFscApp.CreatedDate = oPdiFsc.CreatedDate;
                        oPdiFscApp.LastupdateBy = oPdiFsc.LastupdateBy;
                        oPdiFscApp.LastupdateDate = oPdiFsc.LastupdateDate;
                        Helpers.ReplaceNullable(oPdiFscApp);
                        ctx.PdiFscApplications.Add(oPdiFscApp);
                    }
                }
                try
                {
                    ctx.SaveChanges();
                    trans.Commit();
                    return Json(new { success = true, genNo = newDocNo, message = "Data Saved" });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }

        public ActionResult Delete(string GenerateNo)
        {
            bool deleted = false;
            try
            {
                object[] parameters = { CompanyCode, BranchCode, ProductType, GenerateNo, CurrentUser.UserId };
                ctx.Database.ExecuteSqlCommand("exec uspfn_SvUtlKsgDelete {0},{1},{2},{3},{4}", parameters);
                deleted = true;
            }
            catch { }

            if (deleted)
            { return Json(new { success = true, message = "Data Deleted" }); }
            return View();
        }

        #region Populate Data

        public DataSet GetPdiFscFromSPkUseSP(string branchFrom, string branchTo, string periodeDateFrom, string periodeDateTo, bool IsPDI, bool IsFSC)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            branchFrom = branchFrom == null ? "0" : branchFrom;
            branchTo = branchTo == null ? "0" : branchTo;
            string periodeFrom = Convert.ToDateTime(periodeDateFrom).ToString("yyyyMMdd");
            string periodeTo = Convert.ToDateTime(periodeDateTo).ToString("yyyyMMdd");
            string jobFSC = IsFSC == true ? "%FSC%" : "";
            string jobPDI = IsPDI == true ? "%PDI%" : "";

            cmd.CommandText = "uspfn_SvTrnListKsgFromSPKNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@BranchFrom", branchFrom);
            cmd.Parameters.AddWithValue("@BranchTo", branchTo);
            cmd.Parameters.AddWithValue("@PeriodFrom", periodeFrom);
            cmd.Parameters.AddWithValue("@PeriodTo", periodeTo);
            cmd.Parameters.AddWithValue("@JobPDI", jobPDI);
            cmd.Parameters.AddWithValue("@JobFSC", jobFSC);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public DataSet GetPdiFscFromSPK(string generateNo, string sourceData, bool IsUserHolding)
        {
            DataSet ds = new DataSet();
            SqlConnection con = (SqlConnection)ctx.Database.Connection;
            SqlCommand cmd = con.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            string query = string.Format(@"
declare @CompanyCode varchar(15) 
declare @BranchCode varchar(15)
declare @ProductType varchar(15) 
declare @GenerateNo varchar(15) 
declare @SourceData varchar(15)  

set @CompanyCode = '{0}'
set @ProductType = '{1}'
set @GenerateNo = '{2}'
set @SourceData = '{3}'
set @BranchCode = '{4}'

select * into #t1 from(
select
convert (bit, 1) Process
,a.BranchCodeInv BranchCode
,a.InvoiceNo JobOrderNo
,a.ServiceDate JobOrderDate
,a.BasicModel
,a.ServiceBookNo
,a.PdiFsc PdiFscSeq
,a.Odometer
,a.LaborAmount LaborGrossAmt
,a.MaterialAmount MaterialGrossAmt
,a.PdiFscAmount
,isnull(case when convert(varchar, a.RegisteredDate, 112) = '19000101' then '' else convert(varchar, a.RegisteredDate, 106) end, '')  FakturPolisiDate
,isnull(case when convert(varchar, a.DeliveryDate, 112) = '19000101' then '' else convert(varchar, a.DeliveryDate, 106) end, '')  BPKDate
,a.ChassisCode
,a.ChassisNo
,a.EngineCode
,a.EngineNo
,d.JobType
,a.GenerateNo
,b.GenerateDate
from svTrnPdiFscApplication a
left join svTrnPdiFsc b
  on b.CompanyCode = a.CompanyCode
 and b.BranchCode = a.BranchCode
 and b.ProductType = a.ProductType
 and b.GenerateNo = a.GenerateNo
left join svTrnInvoice c
  on c.CompanyCode = a.CompanyCode
 and c.BranchCode = a.BranchCodeInv
 and c.ProductType = a.ProductType
 and c.InvoiceNo = a.InvoiceNo
left join svTrnService d
  on d.CompanyCode = a.CompanyCode
 and d.BranchCode = a.BranchCode
 and d.ProductType = a.ProductType
 and d.BasicModel = a.BasicModel
 and d.JobOrderNo = a.InvoiceNo
where 1 = 1
 and a.CompanyCode = @CompanyCode
 and (case @BranchCode when '' then '' else a.BranchCode end) = @BranchCode
 and a.InvoiceNo like 'SPK%'
 and a.ProductType = @ProductType
 and a.GenerateNo = @GenerateNo
) #t1 

select (row_number() over (order by BranchCode, JobOrderNo)) No,* from #t1 

select * into #t2 from(
select 
(row_number() over (order by BasicModel)) RecNo
,BasicModel
,PdiFscSeq
,Count(BasicModel) RecCount
,sum(PdiFscAmount) PdiFscAmount 
from #t1 group by BasicModel, PdiFscSeq) #t2

select * from #t2 order by BasicModel

select '' RecNo, 'Total' BasicModel, '' PdiFscSeq, sum(RecCount) RecCount, sum(PdiFscAmount) PdiFscAmount from #t2

select * from #t1 
order by BranchCode, JobOrderNo

drop table #t1
drop table #t2",

               CompanyCode, ProductType, generateNo, "0", IsUserHolding ? "" : BranchCode);

            using (cmd)
            {
                cmd.CommandText = query;
                da.Fill(ds);
            }

            return ds;
        }

        public JsonResult CekSPK(FscFromSPKModel model)
        {
            var dt = GetPdiFscFromSPkUseSP(model.BranchFrom, model.BranchTo, model.PeriodeDateFrom, model.PeriodeDateTo, model.IsPDI, model.IsFSC);
            if (dt.Tables[0].Rows.Count > 0)
            {
                if (dt.Tables[3].Rows.Count > 0)
                {
                    return Json(new { success = true, jobinfo = GetJson(dt.Tables[0]), total = GetJson(dt.Tables[1]), sumtotal = GetJson(dt.Tables[2]) });
                }
                if (dt.Tables[0].Rows.Count == 0) return Json(new { success = false, jobinfo = GetJson(null), total = GetJson(null), sumtotal = GetJson(null), message = "Tidak ada data yang ditampilkan" });
            }
            return Json(new { success = false, jobinfo = GetJson(dt.Tables[0]), total = GetJson(dt.Tables[1]), sumtotal = GetJson(dt.Tables[2]) });
        }

        public JsonResult getSPK(string generateNo, bool IsUserHolding)
        {
            var isPDI = false;
            var isFSC = false;

            var dt = GetPdiFscFromSPK(generateNo, "0", IsUserHolding);

            foreach (DataRow row in dt.Tables[0].Rows)
            {
                try
                {
                    if (row["JobType"].ToString().Contains("PDI"))
                    {
                        isPDI = true;
                    }
                    else if (row["JobType"].ToString().Contains("FSC"))
                    {
                        isFSC = true;
                    }
                }
                catch
                {
                    isPDI = false;
                    isFSC = false;
                }
            }
            return Json(new
            {
                jobinfo = GetJson(dt.Tables[0]),
                total = GetJson(dt.Tables[1]),
                sumtotal = GetJson(dt.Tables[2]),
                forms = new
                {
                    BranchFrom = dt.Tables[0].Rows[0]["BranchCode"].ToString(),
                    BranchTo = dt.Tables[0].Rows[dt.Tables[0].Rows.Count - 1]["BranchCode"].ToString(),
                    PeriodeDateFrom = Convert.ToDateTime(dt.Tables[3].Rows[0]["JobOrderDate"]),
                    PeriodeDateTo = Convert.ToDateTime(dt.Tables[3].Rows[dt.Tables[3].Rows.Count - 1]["JobOrderDate"]),
                    GenerateDate = Convert.ToDateTime(dt.Tables[0].Rows[0]["GenerateDate"]),
                    isPDI = isPDI,
                    isFSC = isFSC
                }
            });
        }

        public string GetNewDocumentNo(string branch)
        {
            var query = "exec uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            object[] parameters = { CompanyCode, branch, "FSC", CurrentUser.UserId, DateTime.Now };

            var newDocument = ctx.Database.SqlQuery<string>(query, parameters).First();

            return newDocument;
        }

        public JsonResult SPKOutstanding(FscFromSPKModel model)
        {
            var dt = GetPdiFscFromSPkUseSP(model.BranchFrom, model.BranchTo, model.PeriodeDateFrom, model.PeriodeDateTo, model.IsPDI, model.IsFSC).Tables[3];

            var data = from e in dt.AsEnumerable()
                       select new SPKOutstandingView
                       {
                           BranchCode = e[0].ToString(),
                           JobOrderNo = e[3].ToString(),
                           JobOrderDate = Convert.ToDateTime(e[4].ToString()),
                           PoliceRegNo = e[5].ToString(),
                           BasicModel = e[6].ToString(),
                           JobType = e[7].ToString(),
                           EmployeeName = e[2].ToString(),
                           Status = e[1].ToString()
                       };

            return Json(GeLang.DataTables<SPKOutstandingView>.Parse(data.AsQueryable(), Request));
        }

        #endregion

        public ActionResult GetKsgFromSPKXls(string GenerateNoFrom, string GenerateNoTo, string SourceData)
        {
            var ret = ctx.Database.SqlQuery<SvTrnPDIFSC>(string.Format("exec uspfn_SvTrnPDIFSC {0},'{1}','{2}','{3}',{4}", CompanyCode, ProductType,
                GenerateNoFrom, GenerateNoTo, SourceData));

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;



            cmd.CommandText = "uspfn_SvTrnPDIFSC";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@GenerateNoFrom", GenerateNoFrom);
            cmd.Parameters.AddWithValue("@GenerateNoTo", GenerateNoTo);
            cmd.Parameters.AddWithValue("@SourceData", SourceData);


            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("KSG");
            ws.Cell(1, 1).Value = "Hello World!";
            ws.Cell(2, 5).Value = "Hello World!";


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var dt = ds.Tables[0];
            int x = 1;
            int y = 1;

            foreach (DataColumn clmn in dt.Columns)
            {
                ws.Cell(y, x).Value = clmn.ColumnName;
                x++;
            }
            x = 1;


            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    //switch (Type.GetTypeCode(typ))
                    //{
                    //    case TypeCode.DateTime:
                    //        ws.Cell(y + 1, x).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    //        ws.Cell(y + 1, x).Style.DateFormat.Format = "dd-MMM-yyyy";
                    //        break;
                    //    case TypeCode.Int16:
                    //    case TypeCode.Int32:
                    //    case TypeCode.Int64:
                    //    case TypeCode.Double:
                    //    case TypeCode.Single:                            
                    //        ws.Cell(y + 1, x).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    //        ws.Cell(y + 1, x).Style.NumberFormat.Format = "#,##0";
                    //        break;
                    //    case TypeCode.Decimal:
                    //        ws.Cell(y + 1, x).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    //        ws.Cell(y + 1, x).Style.NumberFormat.Format = "#,##0.0";
                    //        break;
                    //    case TypeCode.Boolean:
                    //        ws.Cell(y + 1, x).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    //        break;
                    //    default:
                    //        if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                    //        {
                    //            val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                    //        }
                    //        ws.Cell(y + 1, x).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    //        break;
                    //};
                    ws.Cell(y + 1, x).Value = val;
                    x++;
                }
                x = 1;
                y++;
            }
            
            
            MemoryStream ms = new MemoryStream();
            //workbook.SaveAs(ms)
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + "KSGSPK-" + GenerateNoFrom + "-" + GenerateNoTo + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            
            




            //workbook.SaveAs(@"d:\HelloWorld.xlsx");
            workbook.SaveAs(ms);
            Response.BinaryWrite(ms.ToArray());            
            Response.End();
            return new EmptyResult();
        }

    }
}
