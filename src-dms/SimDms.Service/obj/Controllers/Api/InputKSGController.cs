using ExcelLibrary.SpreadSheet;
using OfficeOpenXml;
using SimDms.Service.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class InputKSGController : BaseController
    {
        #region -- Constants --
        private const int ROW_DATA_START = 0,
           COL_RECNO = 0,
           COL_BASICMODEL = 1,
           COL_TRANSTYPE = 2,
           COL_SVBOOKNO = 3,
           COL_PDIFSCSEQ = 4,
           COL_ODOMETER = 5,
           COL_SVDATE = 6,
           COL_REGDATE = 7,
           COL_DLVDATE = 8,
           COL_CHASSISCODE = 9,
           COL_CHASSISNO = 10,
           COL_ENGINECODE = 11,
           COL_ENGINENO = 12;

        private const int XROW_DATA_START = 1,
            XCOL_RECNO = 1,
            XCOL_BASICMODEL = 2,
            XCOL_TRANSTYPE = 3,
            XCOL_SVBOOKNO = 4,
            XCOL_PDIFSCSEQ = 5,
            XCOL_ODOMETER = 6,
            XCOL_SVDATE = 7,
            XCOL_REGDATE = 8,
            XCOL_DLVDATE = 9,
            XCOL_CHASSISCODE = 10,
            XCOL_CHASSISNO = 11,
            XCOL_ENGINECODE = 12,
            XCOL_ENGINENO = 13; 
        #endregion

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                GenerateDate = DateTime.Now,
                RefferenceDate = DateTime.Now,
                FPJDate = DateTime.Now
            });
        }

        public JsonResult Get(string GenerateNo, string SourceData)
        {
            #region -- Query --
            var query = @"
select * into #t1 from(
select
 a.GenerateNo
,a.GenerateDate
,a.FromInvoiceNo
,a.ToInvoiceNo
,a.TotalNoOfItem
,a.IsCampaign
,a.RefferenceNo
,a.RefferenceDate
,a.FPJNo
,a.FPJDate
,a.SenderDealerCode
,a.SenderDealerName
,a.FPJGovNo
from svTrnPdiFsc a
where 1 = 1
 and a.CompanyCode = @CompanyCode
 and a.BranchCode = @BranchCode
 and a.ProductType = @ProductType
 and a.GenerateNo = @GenerateNo
 and a.SourceData = @SourceData
) #t1

select * into #t2 from(
select
 (row_number() over (order by a.GenerateSeq)) RecNo
,a.GenerateSeq
,a.InvoiceNo
,c.InvoiceDate
,c.JobOrderNo
,c.JobOrderDate
,a.BasicModel
,a.TransmissionType
,a.ServiceBookNo
,a.PdiFsc
,a.Odometer
,a.LaborAmount
,a.MaterialAmount
,a.PdiFscAmount
,case a.RegisteredDate when '19000101' then null else a.RegisteredDate end RegisteredDate 
,case a.DeliveryDate when '19000101' then null else a.DeliveryDate end DeliveryDate
,a.ChassisCode
,a.ChassisNo
,a.EngineCode
,a.EngineNo
,a.GenerateNo
,b.GenerateDate
,b.IsCampaign
,a.PdiFsc PdiFscSeq
,a.ServiceDate
from svTrnPdiFscApplication a
left join svTrnPdiFsc b
  on b.CompanyCode = a.CompanyCode
 and b.BranchCode = a.BranchCode
 and b.ProductType = a.ProductType
 and b.GenerateNo = a.GenerateNo
left join svTrnInvoice c
  on c.CompanyCode = a.CompanyCode
 and c.BranchCode = a.BranchCode
 and c.ProductType = a.ProductType
 and c.InvoiceNo = a.InvoiceNo
where 1 = 1
 and a.CompanyCode = @CompanyCode
 and a.BranchCode = @BranchCode
 and a.ProductType = @ProductType
 and a.GenerateNo = @GenerateNo
) #t2 

select * from #t1
select * from #t2
select 
(row_number() over (order by BasicModel)) RecNo
,BasicModel
,PdiFscSeq
,Count(BasicModel) RecCount
,sum(PdiFscAmount) PdiFscAmount 
from #t2 group by BasicModel, PdiFscSeq

drop table #t2
drop table #t1
";
            #endregion

            var cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@GenerateNo", GenerateNo);
            cmd.Parameters.AddWithValue("@SourceData", SourceData);
            var ds = new DataSet();
            new SqlDataAdapter(cmd).Fill(ds);

            var table1 = GetJson(ds.Tables[0]); 
            var table2 = GetJson(ds.Tables[1]);
            var table3 = GetJson(ds.Tables[2]);

            return Json(new { success = true, branchInfo = table1, pdiInfo = table2, rpInfo = table3 });
        }

        public JsonResult SaveHeader(PdiFscHeaderWrapper wrapper)
        {
            try
            {
                var dtv = DateTransValidation(wrapper.GenerateDate.Value);
                if (dtv != "") throw new Exception(dtv);

                var header = ctx.PdiFscs.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.GenerateNo == wrapper.GenerateNo);
                var isNew = false;
                if (header == null)
                {
                    isNew = true;
                    header = new PdiFsc
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        SourceData = "1",
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,                       
                        PostingFlag = "1",
                        GenerateNo = GetNewDocumentNo("FSC", wrapper.GenerateDate.Value)
                    };
                }
                header.GenerateDate = wrapper.GenerateDate;
                header.SenderDealerCode = wrapper.SenderDealerCode;
                header.SenderDealerName = wrapper.SenderDealerName;
                header.ReceiverDealerCode = CompanyCode;
                header.RefferenceNo = wrapper.RefferenceNo;
                header.RefferenceDate = wrapper.RefferenceDate;
                header.FPJNo = wrapper.FPJNo;
                header.FPJDate = wrapper.FPJDate;
                header.FPJGovNo = wrapper.FPJGovNo;
                header.IsCampaign = wrapper.IsCampaign;
                header.LastupdateBy = CurrentUser.UserId;
                header.LastupdateDate = DateTime.Now;

                if (isNew) ctx.PdiFscs.Add(header);
                ctx.SaveChanges();

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult SaveDetailValidation(PdiFscDetailWrapper wrapper)
        {
            try
            {
                PdiFscApplication detail = null;
                if (wrapper.GenerateSeq > 0)
                {
                    detail = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode && x.ProductType == ProductType
                        && x.GenerateNo == wrapper.GenerateNo && x.GenerateSeq == wrapper.GenerateSeq);
                }

                if (wrapper.GenerateSeq == -1 || detail == null)
                {
                    var fs = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceBookNo == wrapper.ServiceBookNo && x.PdiFsc == wrapper.PdiFscSeq);
                    if (fs != null) throw new Exception("No. Buku Service dan FS# sudah pernah dimasukkan!!");

                    var test = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode && x.ProductType == ProductType
                        && x.GenerateNo == wrapper.GenerateNo && x.BasicModel == wrapper.BasicModel
                        && x.TransmissionType == wrapper.TransmissionType
                        && x.ServiceBookNo == wrapper.ServiceBookNo && x.PdiFsc == wrapper.PdiFscSeq);

                    if (test != null) return Json(new { Success = true, 
                        Message = "Informasi Pdi Fsc sudah ada di database, tetap ditambahkan?" });
                }
                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
            
        }

        public JsonResult SaveDetail(PdiFscDetailWrapper wrapper)
        {
            try
            {
                var rate = ctx.PdiFscRates.Where(x => x.CompanyCode == CompanyCode
                    && x.ProductType == ProductType && x.BasicModel == wrapper.BasicModel
                    && x.IsCampaign == wrapper.IsCampaign && x.TransmissionType == wrapper.TransmissionType
                    && x.PdiFscSeq == wrapper.PdiFscSeq && x.IsActive
                    && x.EffectiveDate <= wrapper.ServiceDate.Value).OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();

                if (rate == null) throw new Exception("Nilai Jasa dan Nilai Material belum disetting");

                #region -- Insert/Update Detail --
                PdiFscApplication detail = null;
                if (wrapper.GenerateSeq > 0)
                {
                    detail = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                            && x.BranchCode == BranchCode && x.ProductType == ProductType
                            && x.GenerateNo == wrapper.GenerateNo && x.GenerateSeq == wrapper.GenerateSeq);
                }
                var isNew = false;
                if (wrapper.GenerateSeq == -1 || detail == null)
                {
                    var apps = ctx.PdiFscApplications.Where(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode && x.ProductType == ProductType
                        && x.GenerateNo == wrapper.GenerateNo);
                    var seq = apps.Count() == 0 ? 0 : apps.Max(x => x.GenerateSeq);
                    seq += 1;

                    isNew = true;

                    detail = new PdiFscApplication
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        GenerateNo = wrapper.GenerateNo,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,
                        GenerateSeq = seq
                    };
                }
                detail.PdiFscStatus = "0";
                detail.ServiceBookNo = wrapper.ServiceBookNo;
                detail.BasicModel = wrapper.BasicModel;
                detail.TransmissionType = wrapper.TransmissionType;
                detail.ChassisCode = wrapper.ChassisCode;
                detail.ChassisNo = wrapper.ChassisNo;
                detail.EngineCode = wrapper.EngineCode;
                detail.EngineNo = wrapper.EngineNo;
                detail.PdiFsc = wrapper.PdiFscSeq;
                detail.ServiceDate = wrapper.ServiceDate;
                detail.DeliveryDate = wrapper.DeliveryDate;
                detail.RegisteredDate = wrapper.RegisteredDate;
                detail.Odometer = wrapper.Odometer;
                detail.LastupdateBy = CurrentUser.UserId;
                detail.LastupdateDate = DateTime.Now;
                if (isNew) ctx.PdiFscApplications.Add(detail);
                ctx.SaveChanges(); 
                #endregion

                #region -- ReCalcPdiFsc --
                var sumDetail = (from x in ctx.PdiFscApplications
                                 where x.CompanyCode == CompanyCode
                                 && x.BranchCode == BranchCode && x.ProductType == ProductType
                                 && x.GenerateNo == wrapper.GenerateNo
                                 group x by new { x.GenerateNo, x.LaborAmount, x.MaterialAmount, x.PdiFscAmount } into _
                                 select new
                                 {
                                     TotalNoOfItem = _.Count(),
                                     TotalLaborAmt = _.Sum(x => x.LaborAmount) ?? 0,
                                     TotalMaterialAmt = _.Sum(x => x.MaterialAmount) ?? 0,
                                     TotalAmt = _.Sum(x => x.PdiFscAmount) ?? 0
                                 }).FirstOrDefault();
                var header = ctx.PdiFscs.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.GenerateNo == wrapper.GenerateNo);
                header.TotalNoOfItem = sumDetail.TotalNoOfItem;
                header.TotalLaborAmt = sumDetail.TotalLaborAmt;
                header.TotalMaterialAmt = sumDetail.TotalMaterialAmt;
                header.TotalAmt = sumDetail.TotalAmt;
                header.LastupdateBy = CurrentUser.UserId;
                header.LastupdateDate = DateTime.Now;
                ctx.SaveChanges();
                #endregion

                var res = ctx.Database.ExecuteSqlCommand("exec uspfn_SvUpdatePdiFscRate @p0, @p1, @p2, @p3",
                    CompanyCode, BranchCode, detail.GenerateNo, detail.GenerateSeq);

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult DeleteDetail(string GenerateNo, decimal GenerateSeq)
        {
            try
            {
                var detail = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.GenerateNo == GenerateNo
                    && x.GenerateSeq == GenerateSeq);

                ctx.PdiFscApplications.Remove(detail);
                ctx.SaveChanges();

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        #region -- Input KSG Upload --
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file == null) throw new Exception("File excel belum dipilih");

                var fileType = file.FileName.ToLower().EndsWith("xlsx") ? "xlsx" :
                    file.FileName.ToLower().EndsWith("xls") ? "xls" : string.Empty;
                var list = new object();

                if (fileType == "xls")
                {
                    var res = OpenXLS(file.InputStream);
                    if (res.Message != "") throw new Exception(res.Message);
                    list = res.Data;
                }
                else if (fileType == "xlsx")
                {
                    var res = OpenXLSX(file.InputStream);
                    if (res.Message != "") throw new Exception(res.Message);
                    list = res.Data;
                }
                else throw new Exception(
                    "File format is not supported. Please use only XLS or XLSX Excel File");

                return Json(new { message = "", fileName = file.FileName, data = list });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        private PdiFscDetailPacket OpenXLS(Stream stream)
        {
            try
            {
                var workSheet = Workbook.Load(stream).Worksheets.FirstOrDefault();
                if (workSheet == null) throw new Exception(
                    "Tidak ada Worksheet yang terbaca. Harap cek kembali.");
                var cells = workSheet.Cells;
                if (cells.LastRowIndex < 1) throw new Exception("File minimal harus berisi 1 baris data");

                var data = new List<PdiFscDetailWrapper>();
                for (int i = ROW_DATA_START; i <= cells.LastRowIndex; i++)
                {
                    var record = new PdiFscDetailWrapper
                    {
                        GenerateSeq = Convert.ToDecimal(cells[i, COL_RECNO].StringValue),
                        BasicModel = cells[i, COL_BASICMODEL].StringValue,
                        TransmissionType = cells[i, COL_TRANSTYPE].StringValue,
                        ServiceBookNo = cells[i, COL_SVBOOKNO].StringValue,
                        PdiFscSeq = Convert.ToDecimal(cells[i, COL_PDIFSCSEQ].StringValue),
                        Odometer = Convert.ToDecimal(cells[i, COL_ODOMETER].StringValue),
                        ServiceDate = cells[i, COL_SVDATE].DateTimeValue,
                        RegisteredDate = cells[i, COL_REGDATE].DateTimeValue,
                        DeliveryDate = cells[i, COL_DLVDATE].DateTimeValue,
                        ChassisCode = cells[i, COL_CHASSISCODE].StringValue,
                        ChassisNo = Convert.ToDecimal(cells[i, COL_CHASSISNO].StringValue),
                        EngineCode = cells[i, COL_ENGINECODE].StringValue,
                        EngineNo = Convert.ToDecimal(cells[i, COL_ENGINENO].StringValue)
                    };
                    data.Add(record);
                }
                return new PdiFscDetailPacket { Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new PdiFscDetailPacket { Message = ex.Message };
            }
        }

        private PdiFscDetailPacket OpenXLSX(Stream stream)
        {
            try
            {
                var workSheet = new ExcelPackage(stream).Workbook.Worksheets.FirstOrDefault();
                if (workSheet == null) throw new Exception(
                    "Tidak ada Worksheet yang terbaca. Harap cek kembali.");
                var cells = workSheet.Cells;

                var data = new List<PdiFscDetailWrapper>();
                var defaultDT = new DateTime(1900, 1, 1);
                for (int i = XROW_DATA_START; i <= workSheet.Dimension.End.Row; i++)
                {
                    var record = new PdiFscDetailWrapper
                    {
                        GenerateSeq = cells[i, XCOL_RECNO].Text == null ? 0 :
                            Convert.ToDecimal(cells[i, XCOL_RECNO].Text),
                        BasicModel = cells[i, XCOL_BASICMODEL].Text == null ? "" :
                            cells[i, XCOL_BASICMODEL].Text,
                        TransmissionType = cells[i, XCOL_TRANSTYPE].Text == null ? "" :
                            cells[i, XCOL_TRANSTYPE].Text,
                        ServiceBookNo = cells[i, XCOL_SVBOOKNO].Text == null ? "" :
                            cells[i, XCOL_SVBOOKNO].Text,
                        PdiFscSeq = cells[i, XCOL_PDIFSCSEQ].Text == null ? 0 :
                            Convert.ToDecimal(cells[i, XCOL_PDIFSCSEQ].Text),
                        Odometer = cells[i, XCOL_ODOMETER].Text == null ? 0 :
                            Convert.ToDecimal(cells[i, XCOL_ODOMETER].Text),
                        ServiceDate = cells[i, XCOL_SVDATE].Text == null ? defaultDT :
                            Convert.ToDateTime(cells[i, XCOL_SVDATE].Text),
                        RegisteredDate = cells[i, XCOL_REGDATE].Text == null ? defaultDT :
                            Convert.ToDateTime(cells[i, XCOL_REGDATE].Text),
                        DeliveryDate = cells[i, XCOL_DLVDATE].Text == null ? defaultDT :
                            Convert.ToDateTime(cells[i, XCOL_DLVDATE].Text),
                        ChassisCode = cells[i, XCOL_CHASSISCODE].Text == null ? "" :
                            cells[i, XCOL_CHASSISCODE].Text,
                        ChassisNo = cells[i, XCOL_CHASSISNO].Text == null ? 0 :
                            Convert.ToDecimal(cells[i, XCOL_CHASSISNO].Text),
                        EngineCode = cells[i, XCOL_ENGINECODE].Text == null ? "" :
                            cells[i, XCOL_ENGINECODE].Text,
                        EngineNo = cells[i, XCOL_ENGINENO].Text == null ? 0 :
                            Convert.ToDecimal(cells[i, XCOL_ENGINENO].Text)
                    };
                    data.Add(record);
                }
                return new PdiFscDetailPacket { Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new PdiFscDetailPacket { Message = ex.Message };
            }
        }

        public JsonResult SaveUpload(PdiFscSaveUploadWrapper wrapper)
        {
            try
            {
                var data = wrapper.Detail;

                var existingData = ctx.PdiFscApplications.Where(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.GenerateNo == wrapper.GenerateNo);
                if (existingData.Count() > 0)
                    foreach (var row in existingData) ctx.PdiFscApplications.Remove(row);

                for (int i = 0; i < data.Count; i++)
                {
                    var dt = data[i].ServiceDate.Value.ToString("yyyy/MM/dd hh:mm:ss");

                    var basicModel = data[i].BasicModel;
                    var transType = data[i].TransmissionType;
                    var pdiFscSeq = data[i].PdiFscSeq;

                    var query = string.Format(@"
SELECT * FROM svMstPdiFscRate
WHERE CompanyCode = '{0}' AND ProductType = '{1}' AND BasicModel = '{2}' AND IsCampaign = {3} 
AND TransmissionType = '{4}' AND PdiFscSeq = {5} AND CAST(EffectiveDate AS SMALLDATETIME) = '{6}'
", CompanyCode, ProductType, basicModel, wrapper.IsCampaign ? 1 : 0,
 transType, pdiFscSeq, dt);

                    var rate = ctx.Database.SqlQuery<PdiFscRate>(query).FirstOrDefault();


                    var record = new PdiFscApplication
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        GenerateNo = wrapper.GenerateNo,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,
                        PdiFscStatus = "0",
                        ServiceBookNo = data[i].ServiceBookNo,
                        BasicModel = basicModel,
                        TransmissionType = transType,
                        ChassisCode = data[i].ChassisCode,
                        ChassisNo = data[i].ChassisNo,
                        EngineCode = data[i].EngineCode,
                        EngineNo = data[i].EngineNo,
                        PdiFsc = pdiFscSeq,
                        ServiceDate = data[i].ServiceDate,
                        DeliveryDate = data[i].DeliveryDate,
                        RegisteredDate = data[i].RegisteredDate,
                        Odometer = data[i].Odometer,
                        LaborAmount = rate == null ? 0 : rate.RegularLaborAmount,
                        MaterialAmount = rate == null ? 0 : rate.RegularMaterialAmount,
                        PdiFscAmount = rate == null ? 0 : rate.RegularLaborAmount + rate.RegularMaterialAmount,
                        LastupdateBy = CurrentUser.UserId,
                        LastupdateDate = DateTime.Now,
                        GenerateSeq = data[i].GenerateSeq
                    };
                    ctx.PdiFscApplications.Add(record);
                }
                ctx.SaveChanges();
                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        } 
        #endregion

        #region -- Upload PDIFSC --
        [HttpPost]
        public JsonResult UploadPDIFSC(HttpPostedFileBase file)
        {
            try
            {
                if (!file.FileName.ToLower().EndsWith("txt")) 
                    throw new Exception("File harus berupa text document (.txt)");
                var textFile = new StreamReader(file.InputStream);
                var content = textFile.ReadToEnd();
                return Json(new { Message = "", Content = content, FileName = file.FileName });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult UploadPDIFSCCallback(string content, string senderDealerCode, string refNo, string refDate)
        {
            try
            {
                var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var hdr = new PdiFscFlatHdrWrapper(lines[0]);
                var receiptDate = DateTime.Parse(refDate);

                if (hdr.DataID != "WFREE") throw new Exception("Data Header tidak sesuai dengan PDI & FSC !");
                if (hdr.DealerCode != senderDealerCode) throw new Exception(
                    "Kode Dealer pada File tidak sesuai dengan yang Anda masukkan !");
                if (hdr.RcvDealerCode != CompanyCode) throw new Exception(
                    "Kode Dealer Penerima pada File tidak sesuai dengan Kode Dealer Anda !");
                if (hdr.TotalItem != GetTotalItem(lines)) throw new Exception(
                    "Jumlah Record di Detail Tidak Valid dengan Informasi di Header!");
                if (hdr.ReceiptNo != refNo) throw new Exception(
                    "No Kwitansi pada File tidak sesuai dengan No Kwitansi yang Anda Masukkan !");
                if (hdr.ReceiptDate != receiptDate) throw new Exception(
                    "Tgl Kwitansi pada File tidak sesuai dengan tgl Kwitansi yang Anda Masukkan !");
                if (hdr.ProductType != (
                    ProductType == "2W" ? "A" : 
                    ProductType == "4W" ? "B" : 
                    ProductType == "OB" ? "C" : ProductType)) throw new Exception(
                    "Product Type Pada File Tidak Sesuai Dengan Product Type Dealer Anda !");

                var dtls = new List<PdiFscFlatDtlWrapper>();
                var costs = new List<PdiFscCostWrapper>();
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("1"))
                    {
                        // Pdi Fsc Table
                        var dtl = new PdiFscFlatDtlWrapper(lines[i]);
                        if (i + 1 < lines.Length && lines[i + 1].StartsWith("2"))
                        {
                            dtl.Remark = lines[i + 1].Substring(1).Trim();
                        }
                        dtls.Add(dtl);
                        // Cost Table
                        var cost = costs.FirstOrDefault(x => x.BasicModel == dtl.BasicModel 
                            && x.PdiFscSeq == Convert.ToDecimal(dtl.FS));
                        if (cost == null)
                        {
                            cost = new PdiFscCostWrapper
                            {
                                BasicModel = dtl.BasicModel,
                                PdiFscSeq = Convert.ToDecimal(dtl.FS),
                                Count = 1,
                                Total = dtl.MaterialAmount + dtl.LaborAmount
                            };
                            costs.Add(cost);
                        }
                        else
                        {
                            cost.Count += 1;
                            cost.Total += (dtl.MaterialAmount + dtl.LaborAmount);
                        }
                    }
                }
                for (int i = 0; i < costs.Count; i++) costs[i].GenerateSeq = i + 1;

                return Json(new { Message = "", header = hdr, details = dtls, costs = costs });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult UploadPDIFSCSaveValidation(string genDate, string dealerCode, string refNo)
        {
            try
            {
                var dtvMsg = DateTransValidation(DateTime.Parse(genDate));
                if (dtvMsg != "") throw new Exception(dtvMsg);
                
                var record = ctx.PdiFscs.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.SenderDealerCode == dealerCode && x.RefferenceNo == refNo
                    && x.SourceData == "2");
                if (record != null)
                {
                    if (Convert.ToInt32(record.PostingFlag) >= 2) throw new Exception(
                        "Doc No " + record.GenerateNo + " sudah pernah di-posting");
                    else
                    {
                        return Json(new 
                        {
                            Message = "Data sudah pernah diupload. Apakah Anda ingin update ?",
                            Overwrite = true
                        });
                    }
                }

                return Json(new { Message = "", Overwrite = false });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Overwrite = false });
            }
        }

        public JsonResult UploadPDIFSCSaveData(string genDate, PdiFscSaveHdrWrapper header, List<PdiFscDetailWrapper> details)
        {
            try
            {
                var generateDate = DateTime.Parse(genDate);
                var record = ctx.PdiFscs.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.SenderDealerCode == header.DealerCode && x.RefferenceNo == header.ReceiptNo
                    && x.SourceData == "2");
                if (record == null)
                {
                    #region -- Insert New PdiFsc Header --
                    record = new PdiFsc
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        GenerateNo = GetNewDocumentNo("FSC", generateDate),
                        GenerateDate = generateDate,
                        SourceData = "2",
                        SenderDealerCode = header.DealerCode,
                        ReceiverDealerCode = header.RcvDealerCode,
                        SenderDealerName = header.DealerName,
                        RefferenceNo = header.ReceiptNo,
                        RefferenceDate = header.ReceiptDate,
                        TotalNoOfItem = header.TotalItem,
                        IsCampaign = header.IsCampaign,
                        PostingFlag = "1",
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,
                        LastupdateBy = CurrentUser.UserId,
                        LastupdateDate = DateTime.Now
                    };
                    ctx.PdiFscs.Add(record);
                    ctx.SaveChanges(); 
                    #endregion
                }
                else
                {
                    #region -- Update PdiFsc Header --
                    record.GenerateDate = generateDate;
                    record.SenderDealerCode = header.DealerCode;
                    record.ReceiverDealerCode = header.RcvDealerCode;
                    record.SenderDealerName = header.DealerName;
                    record.RefferenceNo = header.PaymentNumber;
                    record.RefferenceDate = header.PaymentDate;
                    record.TotalNoOfItem = header.TotalItem;
                    record.IsCampaign = header.IsCampaign;
                    record.LastupdateBy = CurrentUser.UserId;
                    record.LastupdateDate = DateTime.Now;
                    ctx.SaveChanges(); 
                    #endregion
                }
                var result = UploadPDIFSCSaveDetails(record.GenerateNo, details);
                if (result != "") throw new Exception(result);
                return Json(new { Message = "", GenerateNo = record.GenerateNo });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        private int GetTotalItem(string[] lines)
        {
            int i = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("1")) i++;
            }
            return i;
        }

        private string UploadPDIFSCSaveDetails(string generateNo, List<PdiFscDetailWrapper> details)
        {
            try
            {
                #region -- Upload Details --
                for (int i = 0; i < details.Count(); i++)
                {
                    var dtl = ctx.PdiFscApplications.FirstOrDefault(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode && x.ProductType == ProductType
                        && x.GenerateNo == generateNo && x.GenerateSeq == i + 1);
                    if (dtl == null)
                    {
                        dtl = new PdiFscApplication
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                            ProductType = ProductType,
                            GenerateNo = generateNo,
                            GenerateSeq = i + 1,
                            ServiceBookNo = details[i].ServiceBookNo,
                            BasicModel = details[i].BasicModel,
                            TransmissionType = details[i].TransmissionType,
                            ChassisCode = details[i].ChassisCode,
                            ChassisNo = details[i].ChassisNo,
                            EngineCode = details[i].EngineCode,
                            EngineNo = details[i].EngineNo,
                            PdiFsc = details[i].PdiFscSeq,
                            ServiceDate = details[i].ServiceDate,
                            DeliveryDate = details[i].DeliveryDate,
                            RegisteredDate = details[i].RegisteredDate,
                            Odometer = details[i].Odometer,
                            LaborAmount = details[i].LaborAmount,
                            MaterialAmount = details[i].MaterialAmount,
                            PdiFscAmount = details[i].TotalAmount,
                            PdiFscStatus = "0",
                            IsLocked = false,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                            LastupdateBy = CurrentUser.UserId,
                            LastupdateDate = DateTime.Now
                        };
                        ctx.PdiFscApplications.Add(dtl);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        dtl.ServiceBookNo = details[i].ServiceBookNo;
                        dtl.BasicModel = details[i].BasicModel;
                        dtl.TransmissionType = details[i].TransmissionType;
                        dtl.ChassisCode = details[i].ChassisCode;
                        dtl.ChassisNo = details[i].ChassisNo;
                        dtl.EngineCode = details[i].EngineCode;
                        dtl.EngineNo = details[i].EngineNo;
                        dtl.PdiFsc = details[i].PdiFscSeq;
                        dtl.ServiceDate = details[i].ServiceDate;
                        dtl.DeliveryDate = details[i].DeliveryDate;
                        dtl.RegisteredDate = details[i].RegisteredDate;
                        dtl.Odometer = details[i].Odometer;
                        dtl.LaborAmount = details[i].LaborAmount;
                        dtl.MaterialAmount = details[i].MaterialAmount;
                        dtl.PdiFscAmount = details[i].TotalAmount;
                        dtl.LastupdateBy = CurrentUser.UserId;
                        dtl.LastupdateDate = DateTime.Now;
                        ctx.SaveChanges();
                    }
                }
                #endregion
                #region -- Adjust Total Values on PDIFSC --
                var query = string.Format(@"
update svTrnPdiFsc set 
 TotalLaborAmt = (select sum(LaborAmount) from svTrnPdiFscApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
,TotalMaterialAmt = (select sum(MaterialAmount) from svTrnPdiFscApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
,TotalAmt = (select sum(LaborAmount + MaterialAmount) from svTrnPdiFscApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
,TotalNoOfItem = (select count(GenerateSeq) from svTrnPdiFscApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
where CompanyCode='{0}'
 and BranchCode='{1}'
 and ProductType='{2}'
 and GenerateNo='{3}'", CompanyCode, BranchCode, ProductType, generateNo);
                ctx.Database.ExecuteSqlCommand(query);
                #endregion
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        #endregion

        #region -- Wrappers --
        public class PdiFscHeaderWrapper
        {
            public string GenerateNo { get; set; }
            public DateTime? GenerateDate { get; set; }
            public string SourceData { get; set; }
            public string FPJNo { get; set; }
            public DateTime? FPJDate { get; set; }
            public string FPJGovNo { get; set; }
            public string SenderDealerCode { get; set; }
            public string SenderDealerName { get; set; }
            public string RefferenceNo { get; set; }
            public DateTime? RefferenceDate { get; set; }
            public bool IsCampaign { get; set; }
        }

        public class PdiFscDetailPacket
        {
            public List<PdiFscDetailWrapper> Data { get; set; }
            public string Message { get; set; }
        }

        public class PdiFscSaveUploadWrapper
        {
            public List<PdiFscDetailWrapper> Detail { get; set; }
            public string GenerateNo { get; set; }
            public bool IsCampaign { get; set; }
        }

        public class PdiFscDetailWrapper
        {
            public string GenerateNo { get; set; }
            public bool IsCampaign { get; set; }
            public decimal GenerateSeq { get; set; }
            public string BasicModel { get; set; }
            public string TransmissionType { get; set; }
            public string ServiceBookNo { get; set; }
            public decimal? PdiFscSeq { get; set; }
            public decimal? Odometer { get; set; }
            public DateTime? ServiceDate { get; set; }
            public DateTime? RegisteredDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string ChassisCode { get; set; }
            public decimal? ChassisNo { get; set; }
            public string EngineCode { get; set; }
            public decimal? EngineNo { get; set; }
            public decimal? LaborAmount { get; set; }
            public decimal? MaterialAmount { get; set; }
            public decimal? TotalAmount { get; set; }
        }

        public class PdiFscCostWrapper
        {
            public decimal GenerateSeq { get; set; }
            public string BasicModel { get; set; }
            public decimal PdiFscSeq { get; set; }
            public decimal Count { get; set; }
            public decimal Total { get; set; }
        }

        public class PdiFscSaveHdrWrapper
        {
            public string DealerCode { get; set; }
            public string RcvDealerCode { get; set; }
            public string DealerName { get; set; }
            public int TotalItem { get; set; }
            public string ReceiptNo { get; set; }
            public DateTime ReceiptDate { get; set; }
            public bool IsCampaign { get; set; }
            public string PaymentNumber { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        public class PdiFscFlatHdrWrapper
        {
            private string line = "";

            public PdiFscFlatHdrWrapper()
            {
            }

            public PdiFscFlatHdrWrapper(string text)
            {
                line = "#" + text.PadRight(176, ' ');
            }

            public string RecordID { get { return line.Substring(1, 1).Trim(); } }
            public string DataID { get { return line.Substring(2, 5).Trim(); } }
            public string DealerCode { get { return line.Substring(7, 10).Trim(); } }
            public string RcvDealerCode { get { return line.Substring(17, 10).Trim(); } }
            public string DealerName { get { return line.Substring(27, 50).Trim(); } }
            public int TotalItem { get { return Convert.ToInt32(line.Substring(77, 6)); } }
            public string PaymentNumber { get { return line.Substring(83, 15).Trim(); } }
            public string ReceiptNo { get { return line.Substring(83, 15).Trim(); } }
            public DateTime PaymentDate { get { return GetDate(98, 8); } }
            public DateTime ReceiptDate { get { return GetDate(98, 8); } }
            public string ProductType { get { return line.Substring(106, 1).Trim(); } }
            public bool IsCampaign { get { return line.Substring(107, 1).Trim().Equals("Y"); } }
            public string BlankFilter { get { return line.Substring(108, 33).Trim(); } }
            public string Text { get { return line.Substring(1).Trim(); } }

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
        }

        public class PdiFscFlatDtlWrapper
        {
            #region -- Initialize --

            private string line = "";
            private string remark = "";

            public PdiFscFlatDtlWrapper()
            {
                string text = "1";
                line = "#" + text.PadRight(176, ' ');
            }

            public PdiFscFlatDtlWrapper(string text)
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

            public string ChassisCode
            {
                get { return GetString(29, 15); }
                set { SetValue(29, 15, value); }
            }

            public string ChassisNo
            {
                get { return GetString(44, 10); }
                set { SetValue(44, 10, value); }
            }

            public string EngineCode
            {
                get { return GetString(54, 15); }
                set { SetValue(54, 15, value); }
            }

            public string EngineNo
            {
                get { return GetString(69, 10); }
                set { SetValue(69, 10, value); }
            }

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
        #endregion
    }
}
