using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.BLL;
using SimDms.Sparepart.Models;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models.Result;
using System.Text.RegularExpressions;
using SimDms.Common;
using SimDms.Common.Models;
using System.Data;
using ClosedXML;
using ClosedXML.Excel;


namespace SimDms.Sparepart.Controllers.Api
{
    public class UploadFileController : BaseController
    {
        private UploadBLL.UploadType uploadType;
        private static string msg = "";
        private bool result = false;

        public JsonResult Default()
        {
            return Json(new
            {
                IsBranch
            });
        }


        [HttpPost]
        public JsonResult UploadFileMasterPart(HttpPostedFileBase file, string uBranchCode, string uploadType, string SupplierCode, string Keterangan, DateTime? POSDate)
        {
            var message = "";
            //IQueryable<UploadFile> result = null;
            try
            {
                var text = "";
                var lst = new List<SpMstItemUpload>();
                if (file != null)
                {
                  
                    var rawData = new byte[file.ContentLength];
                    file.InputStream.Read(rawData, 0, file.ContentLength);

                    var wb = new XLWorkbook(file.InputStream);
                    var ws = wb.Worksheet(1);
                    var fr = ws.FirstRowUsed();
                     var rw = fr.RowUsed();
                     rw = rw.RowBelow();
                    
                     while (!rw.Cell(1).IsEmpty())
                     {
                         var sbordate=rw.Cell(7).GetDouble().ToString();

                         lst.Add(new SpMstItemUpload()
                         {
                             Seqno=rw.Cell(1).GetDouble(),
                             PartNo=rw.Cell(2).GetString(),
                             MovingCode=rw.Cell(3).GetString(),
                             ABCClass=rw.Cell(4).GetString(),
                             LocationCode=rw.Cell(5).GetString(),
                             TypeOfGoods = rw.Cell(6).GetString(),
                             BornDate=new DateTime(int.Parse( sbordate.Substring(0,4)),int.Parse( sbordate.Substring(4,2)),int.Parse( sbordate.Substring(6,2))),
                             Qty=(decimal) rw.Cell(8).GetDouble(),
                             CostPrice=(decimal) rw.Cell(9).GetDouble(),
                             TotalCost=(decimal)rw.Cell(10).GetDouble(),
                             Purchase= (decimal)rw.Cell(11).GetDouble(),
                             Retail=(decimal)rw.Cell(12).GetDouble(),
                             SupplierCode=rw.Cell(13).GetString(),
                             PartName=rw.Cell(14).GetString()
                         });
                         rw = rw.RowBelow();
                     }
                     #region  Process

                     if (BranchCode != null && uploadType != null)
                     {
                         var tx = ctx.Taxes.Where(x => x.CompanyCode == CompanyCode && x.TaxCode == "PPN").FirstOrDefault();
                         if(tx==null)
                             return Json(new { success=false, message = "Tax Belum disetting" });

                         if (uploadType == "0")
                         {
                             #region Direct To Item Master                                
                             ctx.spUtlItemSetups
                             .Where(x => x.CompanyCode == CompanyCode && x.BranchCode == uBranchCode)
                             .ToList()
                             .ForEach(x => ctx.spUtlItemSetups.Remove(x));



                             lst.ForEach(x => ctx.spUtlItemSetups.Add(new spUtlItemSetup()
                             {
                                 CompanyCode = CompanyCode,
                                 BranchCode = uBranchCode,
                                 PartNo = x.PartNo,
                                 MovingCode = x.MovingCode,
                                 ABCClass = x.ABCClass,
                                 LocationCode = x.LocationCode,
                                 TypeOfGoods = x.TypeOfGoods,
                                 BornDate = x.BornDate,
                                 Qty = x.Qty,
                                 CostPrice = x.CostPrice,
                                 PurchasePrice = x.CostPrice,
                                 RetailPrice = x.Retail,
                                 SupplierCode = x.SupplierCode,
                                 PartName = x.PartName
                             }));

                             ctx.SaveChanges();

                             return Json(new {success=true, message ="Sukses initialisasi data sparepart !"});
                             #endregion
                         }
                         else
                         {
                             #region Create PO
                             var ne=new List<string>();
                           
                             lst.ForEach(x =>                                  
                                 {
                                    var p=ctx.spMstItems.Find(CompanyCode,uBranchCode,x.PartNo);
                                    if (p == null)
                                    {
                                        ne.Add(x.Seqno.ToString());
                                        
                                    }
                                    var pc = ctx.spMstItemPrices.Find(CompanyCode, uBranchCode, x.PartNo);
                                      if(pc==null)
                                      {
                                          ne.Add(x.Seqno.ToString());
                                      }
                                 });                            
                             
                             if(ne.Count>0)
                             {
                                 return Json(new {success=false, message ="Harap Periksa initialisasi part items dan part price !",data=ne.Distinct()});
                             }



                             ctx.spUtlItemSetups
                            .Where(x => x.CompanyCode == CompanyCode && x.BranchCode == uBranchCode)
                            .ToList()
                            .ForEach(x => ctx.spUtlItemSetups.Remove(x));

                             lst.ForEach(x => ctx.spUtlItemSetups.Add(new spUtlItemSetup()
                             {
                                 CompanyCode = CompanyCode,
                                 BranchCode = uBranchCode,
                                 PartNo = x.PartNo,
                                 MovingCode = x.MovingCode,
                                 ABCClass = x.ABCClass,
                                 LocationCode = x.LocationCode,
                                 TypeOfGoods = x.TypeOfGoods,
                                 BornDate = x.BornDate,
                                 Qty = x.Qty,
                                 CostPrice = x.CostPrice,
                                 PurchasePrice = x.CostPrice,
                                 RetailPrice = x.Retail,
                                 SupplierCode = x.SupplierCode,
                                 PartName = x.PartName
                             }));


                             var dtTPGO = lst.Select(x => x.TypeOfGoods).Distinct().ToList();

                             dtTPGO.ForEach(x =>
                             {
                                 var POSNo = GetNewDocumentNo("POS",(DateTime) POSDate);
                                 var reslt = ctx.Database.ExecuteSqlCommand(
                                     string.Format("exec uspfn_SpMstSetupItemPO  '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'",
                                     CompanyCode, uBranchCode, ProductType,x, POSNo, POSDate,Keterangan,SupplierCode));
                             });

                             ctx.SaveChanges();
                             return Json(new { success=true, message = "Sukses initialisasi data sparepart !" });
                             #endregion
                         }

                     }

                     #endregion
                     var ttlqty = lst.Sum(x => x.Qty);
                     var ttlcost = lst.Sum(x => x.TotalCost);
                    return Json(new {message = message, data = lst, TotalQty = ttlqty, TotalCostPrice = ttlcost, FileName = file.FileName});
                    
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }

            return Json(new { message = "File Not Found" });
          
        }



        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string uploadType)
        {
            ResultModel result = new ResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (file != null)
            {
                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);
                result.message = file.FileName;
                result.details = Encoding.UTF8.GetString(rawData);
            }

            return Json(result);
        }

        public JsonResult Process(string UploadType, string Contents)
        {
            string[] lines = null;

            lines = Regex.Split(Contents, "\r\n");

            if (UploadBLL.Validate(lines, Type(UploadType)))
            {

                var result = ProcessUploadData(lines, uploadType);
                if (result)
                {
                    return Json(new { success = result });
                }
                else
                {
                    return Json(new { success = result, message = msg });
                }

            }
            else
            {
                return Json(new { success = false, message = "Invalid Format Upload File" });
            }
        }

        private UploadBLL.UploadType Type(string UploadType)
        {
            if (UploadType.Equals("PINVS")) { uploadType = UploadBLL.UploadType.PINVS; }
            if (UploadType.Equals("PORDS")) { uploadType = UploadBLL.UploadType.PORDS; }
            if (UploadType.Equals("TSTKD")) { uploadType = UploadBLL.UploadType.TSTKD; }
            if (UploadType.Equals("PMODP")) { uploadType = UploadBLL.UploadType.PMODP; }
            if (UploadType.Equals("PPRCD")) { uploadType = UploadBLL.UploadType.PPRCD; }
            if (UploadType.Equals("PMDLM")) { uploadType = UploadBLL.UploadType.PMDLM; }
            if (UploadType.Equals("MSMDL")) { uploadType = UploadBLL.UploadType.MSMDL; }

            return uploadType;
        }

        public bool ProcessUploadData(string[] lines, UploadBLL.UploadType uploadType)
        {
            switch (uploadType)
            {
                case UploadBLL.UploadType.PINVD:
                    return UploadDataPINVD(lines);
                case UploadBLL.UploadType.PINVS:
                    return UploadDataPINVS(lines);
                case UploadBLL.UploadType.PORDD:
                    return UploadDataPORDS(lines);
                case UploadBLL.UploadType.PORDS:
                    return UploadDataPORDS(lines);
                case UploadBLL.UploadType.TSTKD:
                    return UploadDataTSTKD(lines);
                case UploadBLL.UploadType.PPRCD:
                    return UploadDataPPRCD(lines);
                case UploadBLL.UploadType.PMODP:
                    return UploadDataPMODP(lines);
                case UploadBLL.UploadType.PMDLM:
                    return UploadDataPMDLM(lines);
                case UploadBLL.UploadType.MSMDL:
                    return UploadDataMSMDL(lines);
                case UploadBLL.UploadType.SHIST:
                    return UploadDataMSMDL(lines);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Upload Data PINVD
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        /// 
        private bool UploadDataPINVS(string[] lines)
        {
            bool result = false;
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }
            string headerText = lines[0];
            if (headerText.Length < 110)
            {
                msg = "flat file text header < 110 karakter";
                result = false;
            }
            // Jika text detail < 110 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 126 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 110 karakter", i, lines[i].Length);
                    result = false;
                }
            }

            // Check jika data sudah ada
            int counter = 0;
            decimal num = 0;
            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SpUtlPINVDHdr oSpUtlPINVSHdr = new SpUtlPINVDHdr();
                oSpUtlPINVSHdr.CompanyCode = ccode;
                oSpUtlPINVSHdr.BranchCode = bcode;
                oSpUtlPINVSHdr.DealerCode = headerText.Substring(6, 10).Trim();
                oSpUtlPINVSHdr.RcvDealerCode = headerText.Substring(16, 10).Trim();
                oSpUtlPINVSHdr.ShipToDealerCode = headerText.Substring(26, 10).Trim();
                oSpUtlPINVSHdr.DeliveryNo = headerText.Substring(42, 15).Trim();
                oSpUtlPINVSHdr.DeliveryNumber = headerText.Substring(42, 15).Trim();
                oSpUtlPINVSHdr.InvoiceNo = headerText.Substring(65, 6).Trim();
                oSpUtlPINVSHdr.InvoiceDate =Convert.ToDateTime(headerText.Substring(57, 8));
                oSpUtlPINVSHdr.DeliveryDate = Convert.ToDateTime(headerText.Substring(71, 8));
                oSpUtlPINVSHdr.BinningNo = string.Empty;
                oSpUtlPINVSHdr.BinningDate = DateTime.MinValue;
                oSpUtlPINVSHdr.Status = "0";
                oSpUtlPINVSHdr.TypeOfGoods = "0";
                oSpUtlPINVSHdr.CreatedBy = uid;
                oSpUtlPINVSHdr.CreatedDate = cdate;
                oSpUtlPINVSHdr.LastUpdateBy = uid;
                oSpUtlPINVSHdr.LastUpdateDate = cdate;
                ctx.SpUtlPINVDHdrs.Add(oSpUtlPINVSHdr);
                ctx.SaveChanges();

                num = lines.Length - 1;
                LookUpDtl oGnMstLookUpDtl = null;
                SpMstItemInfo SpMstItemInfo = null;
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Length > 0)
                    {
                        counter++;
                        string detailText = lines[i];
                        SpUtlPINVDDtl oSpUtlPINVSDtl = new SpUtlPINVDDtl();
                        oSpUtlPINVSDtl.CompanyCode = ccode;
                        oSpUtlPINVSDtl.BranchCode = bcode;
                        oSpUtlPINVSDtl.DealerCode = oSpUtlPINVSHdr.DealerCode;
                        oSpUtlPINVSDtl.OrderNo = detailText.Substring(1, 15).Trim();
                        oSpUtlPINVSDtl.SalesNo = detailText.Substring(16, 6).Trim();
                        oSpUtlPINVSDtl.SalesDate = Convert.ToDateTime(detailText.Substring(22, 8).Trim());
                        oSpUtlPINVSDtl.CaseNumber = detailText.Substring(30, 15).Trim();
                        oSpUtlPINVSDtl.PartNo = detailText.Substring(45, 15).Trim();
                        oSpUtlPINVSDtl.DeliveryNo = oSpUtlPINVSHdr.DeliveryNo;
                        oSpUtlPINVSDtl.PartNoShip = detailText.Substring(60, 15).Trim();
                        oGnMstLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "POCON", oSpUtlPINVSDtl.PartNo);
                        if (oGnMstLookUpDtl != null)
                        {
                            if (oGnMstLookUpDtl.ParaValue == "1")
                            {
                                SpMstItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, oSpUtlPINVSDtl.PartNo);
                                if (SpMstItemInfo != null)
                                {
                                    oSpUtlPINVSDtl.QtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim()) * SpMstItemInfo.OrderUnit;
                                    oSpUtlPINVSDtl.PurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim()) / SpMstItemInfo.OrderUnit;
                                    oSpUtlPINVSDtl.DiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim()) / SpMstItemInfo.OrderUnit;
                                }
                                else
                                {
                                    oSpUtlPINVSDtl.QtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                                    oSpUtlPINVSDtl.PurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                                    oSpUtlPINVSDtl.DiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                                }
                            }
                            else
                            {
                                oSpUtlPINVSDtl.QtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                                oSpUtlPINVSDtl.PurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                                oSpUtlPINVSDtl.DiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                            }
                        }
                        else
                        {
                            oSpUtlPINVSDtl.QtyShipped = Convert.ToDecimal(detailText.Substring(75, 9).Trim());
                            oSpUtlPINVSDtl.PurchasePrice = Convert.ToDecimal(detailText.Substring(84, 10).Trim());
                            oSpUtlPINVSDtl.DiscAmt = Convert.ToDecimal(detailText.Substring(100, 10).Trim());
                        }

                        oSpUtlPINVSDtl.DiscPct = Convert.ToDecimal(detailText.Substring(94, 6).Trim());
                        oSpUtlPINVSDtl.SalesUnit = Convert.ToDecimal(detailText.Substring(75, 9).Trim());

                        spMstItemPrice oMstItemPrice = ctx.spMstItemPrices.Find(ccode, bcode, oSpUtlPINVSDtl.PartNo);
                        if (oMstItemPrice != null) oSpUtlPINVSDtl.CostPrice = oMstItemPrice.CostPrice;

                        oSpUtlPINVSDtl.TotInvoiceAmt = Convert.ToDecimal(detailText.Substring(110, 15));
                        oSpUtlPINVSDtl.ProcessDate = (DateTime)DateTime.Now;
                        oSpUtlPINVSDtl.ProductType = ProductType;
                        oSpUtlPINVSDtl.PartCategory = string.Empty;
                        oSpUtlPINVSDtl.Status = '0';

                        spTrnPPOSHdr dtPosHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, oSpUtlPINVSDtl.OrderNo);
                        if (dtPosHdr != null)
                            oSpUtlPINVSDtl.SupplierCode = dtPosHdr.SupplierCode;
                        else
                            oSpUtlPINVSDtl.SupplierCode = "";

                        oSpUtlPINVSDtl.CreatedBy = uid;
                        oSpUtlPINVSDtl.CreatedDate = cdate;
                        oSpUtlPINVSDtl.LastUpdateBy = uid;
                        oSpUtlPINVSDtl.LastUpdateDate = cdate;
                        ctx.SpUtlPINVDDtls.Add(oSpUtlPINVSDtl);
                        ctx.SaveChanges();

                    }
                }

                GeneratePINVPerType(oSpUtlPINVSHdr.DealerCode, oSpUtlPINVSHdr.DeliveryNo);
                
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// Upload Data PINVD
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataPINVD(string[] lines)
        {
            bool result = false;
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            // Jika text header < 110 karakter, return false
            if (headerText.Length < 110)
            {
                msg = "flat file text header < 110 karakter";
                result = false;
            }

            // Jika text detail < 110 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 110 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 110 karakter", i, lines[i].Length);
                    result = false;
                }
            }

            int counter = 0;
            decimal num = 0;
            try
            {
                string ccode = CompanyCode;
                string bcode = BranchCode;
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;

                SpUtlPINVDHdr oSpUtlPINVDHdr = new SpUtlPINVDHdr();
                oSpUtlPINVDHdr.CompanyCode = ccode;
                oSpUtlPINVDHdr.BranchCode = bcode;
                oSpUtlPINVDHdr.DealerCode = headerText.Substring(6, 10).Trim();
                oSpUtlPINVDHdr.DeliveryNo = headerText.Substring(97, 6).Trim();
                oSpUtlPINVDHdr.TypeOfGoods = "0";

                oSpUtlPINVDHdr.RcvDealerCode = headerText.Substring(16, 10).Trim();
                oSpUtlPINVDHdr.DeliveryDate = Convert.ToDateTime(headerText.Substring(103, 8));
                oSpUtlPINVDHdr.InvoiceNo = headerText.Substring(82, 15).Trim();
                oSpUtlPINVDHdr.BinningNo = string.Empty;
                oSpUtlPINVDHdr.BinningDate = DateTime.MinValue;
                oSpUtlPINVDHdr.Status = "0";
                oSpUtlPINVDHdr.CreatedBy = uid;
                oSpUtlPINVDHdr.CreatedDate = cdate;
                oSpUtlPINVDHdr.LastUpdateBy = uid;
                oSpUtlPINVDHdr.LastUpdateDate = cdate;
                ctx.SpUtlPINVDHdrs.Add(oSpUtlPINVDHdr);
                ctx.SaveChanges();

                num = lines.Length - 1;
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    string detailText = lines[i];
                    SpUtlPINVDDtl oSpUtlPINVDDtl = new SpUtlPINVDDtl();
                    oSpUtlPINVDDtl.CompanyCode = ccode;
                    oSpUtlPINVDDtl.BranchCode = bcode;
                    oSpUtlPINVDDtl.DealerCode = oSpUtlPINVDHdr.DealerCode.Trim();
                    oSpUtlPINVDDtl.DeliveryNo = oSpUtlPINVDHdr.DeliveryNo.Trim();
                    oSpUtlPINVDDtl.OrderNo = detailText.Substring(1, 15).Trim();
                    oSpUtlPINVDDtl.PartNo = detailText.Substring(25, 15);

                    oSpUtlPINVDDtl.SalesNo = detailText.Substring(16, 6);
                    oSpUtlPINVDDtl.PartNoShip = detailText.Substring(40, 15);
                    oSpUtlPINVDDtl.QtyShipped = Convert.ToDecimal(detailText.Substring(55, 9));
                    oSpUtlPINVDDtl.SalesUnit = Convert.ToDecimal(detailText.Substring(64, 3));
                    oSpUtlPINVDDtl.PurchasePrice = Convert.ToDecimal(detailText.Substring(67, 10));

                    decimal disc = Convert.ToDecimal(detailText.Substring(77, 10));
                    spMstItemPrice oMstItemPrice = ctx.spMstItemPrices.Find(ccode, bcode, oSpUtlPINVDDtl.PartNo);
                    if (oMstItemPrice != null) oSpUtlPINVDDtl.CostPrice = oMstItemPrice.CostPrice;
                    if (oSpUtlPINVDDtl.CostPrice > 0) oSpUtlPINVDDtl.DiscPct = Convert.ToDecimal(100) * (disc / oSpUtlPINVDDtl.CostPrice);

                    oSpUtlPINVDDtl.TotInvoiceAmt = Convert.ToDecimal(detailText.Substring(87, 10));
                    oSpUtlPINVDDtl.ProcessDate = Convert.ToDateTime(detailText.Substring(102, 8));
                    oSpUtlPINVDDtl.ProductType = ProductType;
                    oSpUtlPINVDDtl.PartCategory = string.Empty;
                    oSpUtlPINVDDtl.Status = '0';

                    spTrnPPOSHdr dtPosHdr = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, oSpUtlPINVDDtl.OrderNo);
                    if (dtPosHdr != null)
                        oSpUtlPINVDDtl.SupplierCode = dtPosHdr.SupplierCode;
                    else
                        oSpUtlPINVDDtl.SupplierCode = "";

                    oSpUtlPINVDDtl.CreatedBy = uid;
                    oSpUtlPINVDDtl.CreatedDate = cdate;
                    oSpUtlPINVDDtl.LastUpdateBy = uid;
                    oSpUtlPINVDDtl.LastUpdateDate = cdate;
                    ctx.SpUtlPINVDDtls.Add(oSpUtlPINVDDtl);
                    ctx.SaveChanges();

                    GeneratePINVPerType(headerText.Substring(6, 10).Trim(), headerText.Substring(97, 6).Trim());
                }
                result = true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of PRIMARY KEY"))
                {
                    msg = "Data sudah pernah di Upload";
                }
            }
            return result;
        }

        /// <summary>
        /// Upload Data PORDD
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataPORDD(string[] lines)
        {
            bool result = false;
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            // Jika text header < 118 karakter, return false
            if (headerText.Length < 117)
            {
                msg = string.Format("flat file text header : {0}, kurang dari 117 karakter", headerText.Length);
                result = false;
            }

            // Jika text detail < 118 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 117 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 117 karakter", i, lines[i].Length);
                    result = false;
                }
            }

            // Check jika data sudah ada
            if (ctx.SpUtlPORDDHdrs.Find(CompanyCode, BranchCode, headerText.Substring(6, 7), headerText.Substring(76, 15)) != null)
            {
                msg = "flat file data sudah diupload";
                result = false;
            }

            int counter = 0;
            decimal num = 0;
            try
            {
                SpUtlPORDDHdr oSpUtlPORDDHdr = new SpUtlPORDDHdr();
                oSpUtlPORDDHdr.CompanyCode = CompanyCode;
                oSpUtlPORDDHdr.BranchCode = BranchCode;

                //oSpUtlPORDDHdr.DealerCode = headerText.Substring(6, 7);
                // Get Receiving Dealer Code dari file PORDD
                var customer = SelectByStandardCodeCustomer(headerText.Substring(13, 7));
                oSpUtlPORDDHdr.DealerCode = customer == null ? string.Empty : customer.CustomerCode;
                oSpUtlPORDDHdr.OrderNo = headerText.Substring(76, 15);

                oSpUtlPORDDHdr.RcvDealerCode = headerText.Substring(13, 7);
                oSpUtlPORDDHdr.DealerName = headerText.Substring(20, 50);
                oSpUtlPORDDHdr.TotNoItem = Convert.ToDecimal(headerText.Substring(70, 6));
                oSpUtlPORDDHdr.OrderNo = headerText.Substring(76, 15);
                oSpUtlPORDDHdr.OrderDate = Convert.ToDateTime(headerText.Substring(91, 8));
                oSpUtlPORDDHdr.OrderType = headerText.Substring(99, 1).ToString() == "V" ? "01" : headerText.Substring(99, 1).ToString() == "W" ? "01" : "00";
                oSpUtlPORDDHdr.ProductType = headerText.Substring(100, 1);
                oSpUtlPORDDHdr.BackOrderStatus = headerText.Substring(101, 1);
                oSpUtlPORDDHdr.Overseas = headerText.Substring(102, 1);
                oSpUtlPORDDHdr.SpecialInstruction = headerText.Substring(103, 14);
                oSpUtlPORDDHdr.Status = "0";
                oSpUtlPORDDHdr.CreatedBy = CurrentUser.UserId;
                oSpUtlPORDDHdr.CreatedDate = DateTime.Now;

                ctx.SpUtlPORDDHdrs.Add(oSpUtlPORDDHdr);
                ctx.SaveChanges();

                num = lines.Length - 1;
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    SpUtlPORDDDtl oSpUtlPORDDDtl = new SpUtlPORDDDtl();
                    oSpUtlPORDDDtl.CompanyCode = CompanyCode;
                    oSpUtlPORDDDtl.BranchCode = BranchCode;
                    oSpUtlPORDDDtl.DealerCode = oSpUtlPORDDHdr.DealerCode;
                    oSpUtlPORDDDtl.OrderNo = oSpUtlPORDDHdr.OrderNo;
                    oSpUtlPORDDDtl.PartNo = lines[i].Substring(6, 15);

                    oSpUtlPORDDDtl.Qty = Convert.ToDecimal(lines[i].Substring(21, 6));
                    oSpUtlPORDDDtl.ProcessDate = Convert.ToDateTime(lines[i].Substring(27, 8));
                    oSpUtlPORDDDtl.CreatedBy = CurrentUser.UserId;
                    oSpUtlPORDDDtl.CreatedDate = DateTime.Now;
                    ctx.SpUtlPORDDDtls.Add(oSpUtlPORDDDtl);
                    ctx.SaveChanges();

                }
                result = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Upload Data PORDS
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataPORDS(string[] lines)
        {
            bool result = false;
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            // Jika text header < 118 karakter, return false
            if (headerText.Length < 67)
            {
                msg = string.Format("flat file text header : {0}, kurang dari 117 karakter", headerText.Length);
                result = false;
            }

            // Jika text detail < 118 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 67 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 117 karakter", i, lines[i].Length);
                    result = false;
                }
            }

            // Check jika data sudah ada
            if (ctx.SpUtlPORDDHdrs.Find(CompanyCode, BranchCode, BranchCode, headerText.Substring(42, 15)) != null)
            {
                msg = "flat file data sudah diupload";
                result = false;
            }

            int counter = 0;
            decimal num = 0;
            try
            {
                SpUtlPORDDHdr oSpUtlPORDDHdr = new SpUtlPORDDHdr();
                oSpUtlPORDDHdr.CompanyCode = CompanyCode;
                oSpUtlPORDDHdr.BranchCode = BranchCode;

                // Get Receiving Dealer Code dari file PORDD
                oSpUtlPORDDHdr.DealerCode = headerText.Substring(6, 10);
                oSpUtlPORDDHdr.RcvDealerCode = headerText.Substring(16, 10);
                oSpUtlPORDDHdr.TotNoItem = Convert.ToDecimal(headerText.Substring(36, 6));
                oSpUtlPORDDHdr.OrderNo = headerText.Substring(42, 15);
                oSpUtlPORDDHdr.OrderDate = Convert.ToDateTime(headerText.Substring(57, 8));
                oSpUtlPORDDHdr.OrderType = headerText.Substring(65, 1).ToString() == "V" ? "01" : headerText.Substring(65, 1).ToString() == "W" ? "01" : "00";
                oSpUtlPORDDHdr.ProductType = headerText.Substring(66, 1);
                oSpUtlPORDDHdr.BackOrderStatus = headerText.Substring(67, 1);
                oSpUtlPORDDHdr.Status = "0";
                oSpUtlPORDDHdr.CreatedBy = CurrentUser.UserId;
                oSpUtlPORDDHdr.CreatedDate = DateTime.Now;

                ctx.SpUtlPORDDHdrs.Add(oSpUtlPORDDHdr);
                ctx.SaveChanges();

                num = lines.Length - 1;
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    SpUtlPORDDDtl oSpUtlPORDDDtl = new SpUtlPORDDDtl();
                    oSpUtlPORDDDtl.CompanyCode = CompanyCode;
                    oSpUtlPORDDDtl.BranchCode = BranchCode;
                    oSpUtlPORDDDtl.DealerCode = oSpUtlPORDDHdr.DealerCode;
                    oSpUtlPORDDDtl.OrderNo = oSpUtlPORDDHdr.OrderNo;
                    oSpUtlPORDDDtl.PartNo = lines[i].Substring(6, 15);

                    oSpUtlPORDDDtl.Qty = Convert.ToDecimal(lines[i].Substring(21, 6));
                    oSpUtlPORDDDtl.ProcessDate = Convert.ToDateTime(lines[i].Substring(27, 8));
                    oSpUtlPORDDDtl.CreatedBy = CurrentUser.UserId;
                    oSpUtlPORDDDtl.CreatedDate = DateTime.Now;
                    ctx.SpUtlPORDDDtls.Add(oSpUtlPORDDDtl);
                    ctx.SaveChanges();
                }
                result = true;
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Upload Data TSTKD
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataTSTKD(string[] lines)
        {
            bool result = false;
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                result = false;
            }

            string headerText = lines[0];
            // Jika text header < 145 karakter, return false
            if (headerText.Length < 145)
            {
                msg = string.Format("flat file text header : {0}, kurang dari 145 karakter", headerText.Length);
                result = false;
            }

            // Jika text detail < 145 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 145 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 145 karakter", i, lines[i].Length);
                    result = false;
                }
            }

            // Check jika data sudah ada
            if (ctx.SpUtlStockTrfHdrs.Find(CompanyCode, BranchCode, headerText.Substring(16, 10), headerText.Substring(82, 15)) != null)
            {
                msg = "flat file data sudah diupload";
                result = false;
            }

            decimal num = 0;
            int counter = 0;

            if (BranchCode != headerText.Substring(16, 10).Trim())
            {
                msg = "Username anda tidak berhak meng-Upload file ini karena cabang penerima berbeda(" + headerText.Substring(16, 10).Trim() + ")";
                return result;
            }

            try
            {
                SpUtlStockTrfHdr oSpUtlStockTrfHdr = new SpUtlStockTrfHdr();
                oSpUtlStockTrfHdr.CompanyCode = CompanyCode;
                oSpUtlStockTrfHdr.BranchCode = BranchCode;
                oSpUtlStockTrfHdr.DealerCode = headerText.Substring(6, 10); //headerText.Substring(6, 7);
                oSpUtlStockTrfHdr.LampiranNo = Convert.ToString(headerText.Substring(82, 15)).Trim();

                oSpUtlStockTrfHdr.RcvDealerCode = headerText.Substring(16, 10);
                oSpUtlStockTrfHdr.InvoiceNo = headerText.Substring(128, 15);

                oSpUtlStockTrfHdr.Status = "0";
                oSpUtlStockTrfHdr.TypeOfGoods = TypeOfGoods;
                oSpUtlStockTrfHdr.CreatedBy = CurrentUser.UserId;
                oSpUtlStockTrfHdr.CreatedDate = DateTime.Now;
                oSpUtlStockTrfHdr.LastUpdateBy = CurrentUser.UserId;
                oSpUtlStockTrfHdr.LastUpdateDate = DateTime.Now;
                ctx.SpUtlStockTrfHdrs.Add(oSpUtlStockTrfHdr);
                
                result =  ctx.SaveChanges() > 0;

                if (result)
                {
                    num = lines.Length - 1;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        counter++;
                        SpUtlStockTrfDtl oSpUtlStockTrfDtl = new SpUtlStockTrfDtl();
                        oSpUtlStockTrfDtl.CompanyCode = CompanyCode;
                        oSpUtlStockTrfDtl.BranchCode = BranchCode;
                        oSpUtlStockTrfDtl.DealerCode = oSpUtlStockTrfHdr.DealerCode;
                        oSpUtlStockTrfDtl.LampiranNo = oSpUtlStockTrfHdr.LampiranNo;
                        oSpUtlStockTrfDtl.OrderNo = lines[i].Substring(1, 15);
                        oSpUtlStockTrfDtl.PartNo = lines[i].Substring(22, 15);

                        oSpUtlStockTrfDtl.SalesNo = lines[i].Substring(16, 6);
                        oSpUtlStockTrfDtl.PartNoShip = lines[i].Substring(37, 15);
                        oSpUtlStockTrfDtl.QtyShipped = Convert.ToDecimal(lines[i].Substring(52, 9));
                        oSpUtlStockTrfDtl.SalesUnit = Convert.ToDecimal(lines[i].Substring(61, 3));
                        oSpUtlStockTrfDtl.PurchasePrice = Convert.ToDecimal(lines[i].Substring(64, 10));
                        oSpUtlStockTrfDtl.CostPrice = Convert.ToDecimal(lines[i].Substring(74, 10)); ;
                        oSpUtlStockTrfDtl.ProductType = ProductType;

                        oSpUtlStockTrfDtl.CreatedBy = CurrentUser.UserId;
                        oSpUtlStockTrfDtl.CreatedDate = DateTime.Now;
                        oSpUtlStockTrfDtl.LastUpdateBy = CurrentUser.UserId;
                        oSpUtlStockTrfDtl.LastUpdateDate = DateTime.Now;
                        ctx.SpUtlStockTrfDtls.Add(oSpUtlStockTrfDtl);
                        result = ctx.SaveChanges() > 0;
                    }
                }
            }
            catch
            {
                msg = "Upload Gagal! File ini sudah terdaftar pada list penerimaan transfer stock";
            }

            return result;
        }

        /// <summary>
        /// Upload Data Part Price Domestic
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        /// 
        private bool UploadDataPPRCD(string[] lines)
        {
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ada data";
                //return false;
            }

            string headerText = lines[0];
            // Jika text header < 165 karakter, return false
            if (headerText.Length < 165)
            {
                msg = string.Format("flat file text header : {0}, kurang dari 165 karakter");
                //return false;
            }

            // Jika text detail < 165 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 165 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}: {1}, kurang dari 165 karakter", i + 1, lines[i].Length);
                    //return false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            decimal num = Convert.ToInt32(headerText.Substring(13, 6));
            if (num != lines.Length - 1)
            {
                msg = string.Format("informasi jumlah detail di header: {0}, jumlah detail aktual: {1}", num.ToString("n2"), lines.Length - 1);
                //return false;
            }

            bool result = false;

            try
            {
                string companyCode = CompanyCode;
                string branchCode = BranchCode;
                string productType = ProductType;

                string SupplierCD = headerText.Substring(6, 7);
                var oGnMstSupplier = SelectByStandardCodeSupplier(SupplierCD);
                string SupplierCode = oGnMstSupplier == null ? string.Empty : oGnMstSupplier.SupplierCode;

                if (SupplierCode.Equals(string.Empty))
                {
                    msg = string.Format("Kode Supplier Belum diregistrasi di Kode ISI Standard : [ {0} ]", SupplierCD);
                    return false;
                }

                SupplierProfitCenter oGnMstSupp = ctx.SupplierProfitCenters.Find(companyCode, branchCode, SupplierCode, ProfitCenter);
                if (oGnMstSupp == null)
                {
                    msg = string.Format("Data profit center supplier({0}) ini tidak diketemukan !", SupplierCode);
                    return false;
                }
                string TaxCode = oGnMstSupp.TaxCode;
                string userId = CurrentUser.UserId;
                DateTime time = DateTime.Now;
                decimal taxPct = ctx.Taxes.Find(companyCode, TaxCode).TaxPct.Value;
                string sql = string.Empty;
                int counter = 0;
                decimal retailPriceInclTax = 0;
                decimal retailPrice = 0;
                decimal realpurchasePriceInclTax = 0;
                decimal realpurchasePrice = 0;
                decimal OrigrealpurchasePrice = 0;
                bool isRetailPriceIncPPN = ctx.GnMstCoProfileSpares.Find(companyCode, branchCode).isRetailPriceIncPPN;
                bool isPurchasePriceIncPPN = ctx.GnMstCoProfileSpares.Find(companyCode, branchCode).isPurchasePriceIncPPN;

                // For spHstItemPrice
                decimal discount = 0;
                decimal costPrice = 0;
                decimal oldRetailPrice = 0;
                decimal oldPurchasePrice = 0;
                decimal oldCostPrice = 0;

                var dtItemInfo = ctx.SpMstItemInfos.Where(a => a.CompanyCode == CompanyCode);
                var dtItems = ctx.spMstItems.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode);
                var dtItemPrice = ctx.spMstItemPrices.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode);

                var queryHstPrice = string.Format(@"SELECT
                                        a.PartNo,
                                        a.UpdateDate,
                                        ISNULL(RetailPrice,0) AS RetailPrice, 
                                        ISNULL(RetailPriceInclTax,0) AS RetailPriceInclTax, 
                                        ISNULL(PurchasePrice,0) AS PurchasePrice,
                                        ISNULL(CostPrice,0) AS CostPrice,
                                        ISNULL(Discount,0) AS Discount,
                                        ISNULL(OldRetailPrice,0) AS OldRetailPrice, 
                                        ISNULL(OldPurchasePrice,0) AS OldPurchasePrice,
                                        ISNULL(OldCostPirce,0) AS OldCostPirce,
                                        ISNULL(OldDiscount,0) AS OldDiscount,
                                        LastPurchaseUpdate,
                                        LastRetailPriceUpdate
                                    FROM
                                        spHstItemPrice a
                                        INNER JOIN (SELECT 
                                                        PartNo, MAX(UpdateDate) UpdateDate
                                                    FROM
                                                        spHstItemPrice b
                                                    WHERE
                                                        b.CompanyCode = {0}
                                                        AND b.BranchCode = {1}
                                                    GROUP BY
                                                        PartNo
                                        ) LastUpdate ON LastUpdate.PartNo = a.PartNo AND LastUpdate.UpdateDate = a.UpdateDate
                                    WHERE
                                        a.CompanyCode = {0}
                                        AND a.BranchCode = {1}
                                    ", CompanyCode, BranchCode);

                var dtHstPrice = ctx.Database.SqlQuery<spHstItemPrice>(queryHstPrice);

                //dtItemInfo.PrimaryKey = new DataColumn[] { dtItemInfo.Columns["PartNo"] };
                //dtItems.PrimaryKey = new DataColumn[] { dtItems.Columns["PartNo"] };
                //dtItemPrice.PrimaryKey = new DataColumn[] { dtItemPrice.Columns["PartNo"] };
                //dtHstPrice.PrimaryKey = new DataColumn[] { dtHstPrice.Columns["PartNo"] };

                // Looping data yang ada di flat file, dimulai line ke-2
                for (int i = 1; i < lines.Length; i++)
                {
                    string partNo = lines[i].Substring(1, 15);
                    string partName = lines[i].Substring(16, 50).Replace("¿", " ").Replace("�", " ").Replace("'", "’");
                    string partCategory = lines[i].Substring(69, 3);
                    decimal salesUnit = Convert.ToDecimal(lines[i].Substring(72, 5));
                    decimal orderUnit = Convert.ToDecimal(lines[i].Substring(77, 5));

                    if (isPurchasePriceIncPPN)
                    {
                        OrigrealpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePriceInclTax = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePrice = Math.Truncate(realpurchasePriceInclTax / (1 + (taxPct / 100)));
                    }
                    else
                    {
                        OrigrealpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        realpurchasePriceInclTax = Math.Truncate(realpurchasePrice * (1 + (taxPct / 100)));
                    }

                    if (isRetailPriceIncPPN)
                    {
                        retailPriceInclTax = Convert.ToDecimal(lines[i].Substring(82, 9));
                        retailPrice = Math.Truncate(retailPriceInclTax / (1 + (taxPct / 100)));
                    }
                    else
                    {
                        retailPrice = Convert.ToDecimal(lines[i].Substring(82, 9));
                        retailPriceInclTax = Math.Truncate(retailPrice * (1 + (taxPct / 100)));
                    }

                    // Insert/Update spMstItemInfo
                    var drInfo = dtItemInfo.FirstOrDefault(a => a.PartNo == partNo);
                    counter++;

                    if (drInfo != null)
                    {
                        sql += string.Format("UPDATE spMstItemInfo SET SalesUnit = '{0}', OrderUnit = '{1}', " +
"PurchasePrice = '{2}', LastUpdateBy = '{3}', LastUpdateDate = '{4}', PartName = '{7}' WHERE CompanyCode = '{5}' AND PartNo = '{6}'",
salesUnit, orderUnit, OrigrealpurchasePrice, userId, time, companyCode, partNo, partName);
                    }
                    else
                    {
                        sql += string.Format("INSERT INTO spMstItemInfo VALUES('{0}', '{1}', '{2}', '{3}', '{4}'," +
" '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}'," +
" '{19}')", companyCode, partNo, SupplierCode, partName, true, 0, salesUnit, orderUnit, OrigrealpurchasePrice, "PCS", "1",
productType, partCategory, userId, time, userId, time, false, null, null);

                    }

                    var drItems = dtItems.FirstOrDefault(a => a.PartNo == partNo);
                    if (drItems != null)
                    {
                        // Insert/Update spMstItemPrice
                        var drPrice = dtItemPrice.FirstOrDefault(a => a.PartNo == partNo);

                        if (drPrice != null)
                        {
                            sql += string.Format("UPDATE spMstItemPrice SET RetailPrice = '{0}'," +
" RetailPriceInclTax = '{1}', PurchasePrice = '{2}', OldRetailPrice = '{3}', OldPurchasePrice = '{4}'," +
" LastUpdateBy = '{5}', LastUpdateDate = '{6}' WHERE CompanyCode = '{7}'AND BranchCode = '{8}' AND PartNo = '{9}'",
retailPrice, retailPriceInclTax, realpurchasePrice, drPrice.RetailPrice, drPrice.PurchasePrice,
userId, time, companyCode, branchCode, partNo);

                        }


                        // Insert spHstItemPrice
                        var drHst = dtHstPrice.FirstOrDefault(a => a.PartNo == partNo);
                        if (drHst != null)
                        {
                            discount = drHst.Discount.Value;
                            oldRetailPrice = drHst.RetailPrice.Value;
                            oldPurchasePrice = drHst.PurchasePrice.Value;
                            oldCostPrice = drHst.CostPrice.Value;
                            costPrice = oldCostPrice;
                            DateTime lastPurchasePriceUpdate = drHst.LastPurchaseUpdate.Value == null ? time : drHst.LastPurchaseUpdate.Value;
                            DateTime lastRetailPriceUpdate = drHst.LastRetailPriceUpdate.Value == null ? time : drHst.LastRetailPriceUpdate.Value;

                            sql += string.Format("INSERT INTO spHstItemPrice VALUES('{0}', '{1}', '{2}'," +
    " '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '0')",
    companyCode, branchCode, partNo, time, retailPrice, retailPriceInclTax, realpurchasePrice, costPrice, discount,
    oldRetailPrice, oldPurchasePrice, oldCostPrice, discount, lastPurchasePriceUpdate, lastRetailPriceUpdate,
    userId, time, userId, time);

                        }
                        else
                        {
                            discount = 0;
                            oldRetailPrice = 0;
                            oldPurchasePrice = 0;
                            oldCostPrice = 0;
                            costPrice = realpurchasePrice;

                            sql += string.Format("INSERT INTO spHstItemPrice VALUES('{0}', '{1}', '{2}'," +
    " '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '0')",
    companyCode, branchCode, partNo, time, retailPrice, retailPriceInclTax, realpurchasePrice, costPrice, discount,
    oldRetailPrice, oldPurchasePrice, oldCostPrice, discount, null, null, userId, time, userId, time);

                        }
                    }

                    if (counter != 0 && (counter % 100 == 0) && !string.IsNullOrEmpty(sql))
                    {
                        if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                        {
                            msg = "Proses upload part price gagal";
                            result = false;
                        }
                        sql = string.Empty;
                    }
                }


                if (!string.IsNullOrEmpty(sql))
                {
                    if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                    {
                        msg = "Proses upload part price gagal";
                        result = false;
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Upload Data Part Modification
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataPMODP(string[] lines)
        {
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg = "flat file tidak ditemukan";
                return false;
            }

            string headerText = lines[0];
            if (headerText.Length < 80)
            {
                msg = string.Format("flat file text header: {0}, kurang dari 80 karakter", headerText.Length);
                return false;
            }

            // Jika text detail < 80 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 80 && lines[i].Length > 0)
                {
                    msg = string.Format("flat file text detail ke-{0}:{1}, kurang dari 80 karakter", i, lines[i].Length);
                    return false;
                }
            }

            bool result = false;
            int counter = 0;
            decimal num = 0;

            try
            {
                // Create Temporary DataTable to Store Upload Price 
                DataTable dtUploadPrice = new DataTable();
                dtUploadPrice.Columns.Add("OldPartNo", typeof(string));
                dtUploadPrice.Columns.Add("SeqNo", typeof(string));
                dtUploadPrice.Columns.Add("EndMark", typeof(string));
                dtUploadPrice.Columns.Add("NewPartNo", typeof(string));
                dtUploadPrice.Columns.Add("UnitConvertion", typeof(decimal));
                dtUploadPrice.Columns.Add("ReasonCode", typeof(string));
                dtUploadPrice.Columns.Add("InterChangeCode", typeof(string));

                // Looping data yang ada di flat file, dimulai line ke-2
                num = lines.Length - 1;
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    DataRow row = dtUploadPrice.NewRow();
                    row["OldPartNo"] = lines[i].Substring(1, 15);
                    row["SeqNo"] = lines[i].Substring(16, 2);
                    row["EndMark"] = lines[i].Substring(18, 1);
                    row["NewPartNo"] = lines[i].Substring(19, 15);
                    row["UnitConvertion"] = lines[i].Substring(34, 2);
                    row["ReasonCode"] = lines[i].Substring(36, 2);
                    row["InterChangeCode"] = lines[i].Substring(38, 2);
                    dtUploadPrice.Rows.Add(row);

                    DataRow row2 = dtUploadPrice.NewRow();
                    row2["OldPartNo"] = lines[i].Substring(40, 15);
                    row2["SeqNo"] = lines[i].Substring(55, 2);
                    row2["EndMark"] = lines[i].Substring(57, 1);
                    row2["NewPartNo"] = lines[i].Substring(58, 15);
                    row2["UnitConvertion"] = lines[i].Substring(73, 2);
                    row2["ReasonCode"] = lines[i].Substring(75, 2);
                    row2["InterChangeCode"] = lines[i].Substring(77, 2);
                    dtUploadPrice.Rows.Add(row2);

                }

                string companyCode = CompanyCode;
                string userId = CurrentUser.UserId;
                string productType = ProductType;
                DateTime time = DateTime.Now;
                string sql = string.Empty;

                var dtPartMod = (from a in ctx.SpMstItemInfos
                                 join b in ctx.spMstItemMods
                                 on new { a.CompanyCode, a.PartNo }
                                 equals new { b.CompanyCode, b.PartNo }
                                 where a.CompanyCode == CompanyCode && a.ProductType == ProductType
                                 select a).FirstOrDefault();

                var dtPartCategory = ctx.SpMstItemInfos.FirstOrDefault(a => a.CompanyCode == CompanyCode);


                counter = 0;
                num = dtUploadPrice.Rows.Count;
                foreach (DataRow row in dtUploadPrice.Rows)
                {
                    if (dtPartMod != null)
                    {
                        if ((string)row["OldPartNo"] == dtPartMod.PartNo)
                        {
                            counter++;
                            sql += string.Format("UPDATE spMstItemMod SET NewPartNo = '{0}', UnitConversion = '{1}', InterChangeCode = '{2}', " +
"EndMark = '{3}', LastUpdateBy = '{4}', LastUpdateDate = '{5}' WHERE CompanyCode = '{6}' AND PartNo = '{7}'",
(string)row["NewPartNo"], (decimal)(row["UnitConvertion"]), (string)row["InterChangeCode"], (string)row["EndMark"],
userId, time, companyCode, (string)row["OldPartNo"]);
                        }
                    }

                    else
                    {
                        if (dtPartCategory != null)
                        {
                            counter++;
                            string partCategory = dtPartCategory.PartCategory;

                            sql += string.Format("INSERT INTO spMstItemMod VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}')",
companyCode, (string)row["OldPartNo"], (string)row["NewPartNo"], (decimal)(row["UnitConvertion"]), (string)row["InterChangeCode"],
productType, partCategory, (string)row["EndMark"], userId, time, userId, time, false, null, null);
                        }
                    }

                    if (counter != 0 && (counter % 100 == 0) && !string.IsNullOrEmpty(sql))
                    {
                        if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                        {
                            msg = "Proses upload part modifikasi gagal";
                            result = false;
                        }
                        sql = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    if (ctx.Database.ExecuteSqlCommand(sql) == 0)
                    {
                        msg = "Proses upload part modifikasi gagal";
                        result = false;
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// Upload Data PMDLM
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool UploadDataPMDLM(string[] lines)
        {
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg ="flat file tidak ditemukan";
                return false;
            }

            string headerText = lines[0];
            // Jika text header < 134 karakter, return false
            if (headerText.Length < 134)
            {
                msg =string.Format("flat file text header: {0}, kurang dari 134 karakter", headerText.Length);
                return false;
            }

            // Jika text detail < 134 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 93 && lines[i].Length > 0)
                {
                    msg =string.Format("flat file text detail ke-{0}:{1}, kurang dari 134 karakter", i, lines[i].Length);
                    return false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            int counter = 0;
            decimal num = Convert.ToInt32(headerText.Substring(6, 6));
            if (num != lines.Length - 1)
            {
                msg =string.Format("informasi jumlah detail di header: {0}, jumlah detail aktual: {1}", num, lines.Length - 1);
                return false;
            }

            bool result = false;

            try
            {
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;
                string ccode = CompanyCode;
                string bcode = BranchCode;

                // Looping data yang ada di flat file, dimulai line ke-2
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    string partno = lines[i].Substring(1, 15);
                    string modelno = lines[i].Substring(16, 30);

                    bool isnew = false;
                    SpMstItemInfo oMstItemInfo = ctx.SpMstItemInfos.Find(ccode, partno);
                    SpMstItemModel oMstItemModel = ctx.SpMstItemModels.Find(ccode, partno, modelno);

                    if (oMstItemInfo != null)
                    {
                        if (oMstItemModel == null)
                        {
                            oMstItemModel = new SpMstItemModel();
                            oMstItemModel.CompanyCode = ccode;
                            oMstItemModel.PartNo = partno;
                            oMstItemModel.ModelCode = modelno;
                            oMstItemModel.CreatedBy = uid;
                            oMstItemModel.CreatedDate = cdate;
                            isnew = true;
                        }
                        oMstItemModel.PartCategory = oMstItemInfo.PartCategory;
                        oMstItemModel.LastUpdateBy = uid;
                        oMstItemModel.LastUpdateDate = cdate;
                        if (isnew) ctx.SpMstItemModels.Add(oMstItemModel);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        msg = "Invalid Part No, Tidak terdapat informasinya di spMstItemInfo";
                        result = false;
                    }

                   
                }
                result = true;
            }
            catch (Exception ex)
            {
            }
            
            return result;
        }

        private bool UploadDataMSMDL(string[] lines)
        {
            // Jika lines tidak ada data, return false
            if (lines.Length <= 0)
            {
                msg ="flat file tidak ditemukan";
                return false;
            }

            string headerText = lines[0];
            // Jika text header < 117 karakter, return false
            if (headerText.Length < 117)
            {
                msg =string.Format("flat file text header: {0}, kurang dari 117 karakter", headerText.Length);
                return false;
            }

            // Jika text detail < 117 karakter, return false
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length < 117 && lines[i].Length > 0)
                {
                    msg =string.Format("flat file text detail ke-{0}:{1}, kurang dari 117 karakter", i, lines[i].Length);
                    return false;
                }
            }

            // Check jumlah detail berdasarkan informasi dari header
            int counter = 0;
            decimal num = Convert.ToInt32(headerText.Substring(7, 6));
            if (num != lines.Length - 1)
            {
                msg =string.Format("informasi jumlah detail di header: {0}, jumlah detail aktual: {1}", num, lines.Length - 1);
                return false;
            }

            bool result = false;

            try
            {
                DateTime cdate = DateTime.Now;
                string uid = CurrentUser.UserId;
                string ccode = CompanyCode;
                string codeId = GnMstLookUpHdr.ModelVehicle;
                var dtSeq = ctx.LookUpDtls.Where(a=>a.CompanyCode == ccode && a.CodeID == codeId);
                int seq = dtSeq.Count();

                // Looping data yang ada di flat file, dimulai line ke-2
                for (int i = 1; i < lines.Length; i++)
                {
                    counter++;
                    string lookupValue = lines[i].Substring(1, 15);
                    var oGnMstLookUpHdr = ctx.LookUpHdrs.Find(ccode, codeId);
                    var oGnMstLookUpDtl = ctx.LookUpDtls.Find(ccode, codeId, lookupValue);

                    if (oGnMstLookUpHdr != null)
                    {
                        if (oGnMstLookUpDtl == null)
                        {
                            LookUpDtl recTemp = new LookUpDtl();
                            recTemp.CompanyCode = ccode;
                            recTemp.CodeID = codeId;
                            recTemp.SeqNo = seq + i;
                            recTemp.LookUpValue = lines[i].Substring(1, 15);
                            recTemp.ParaValue = lines[i].Substring(1, 15);
                            recTemp.LookUpValueName = lines[i].Substring(16, 100);
                            recTemp.CreatedBy = CurrentUser.UserId;
                            recTemp.CreatedDate = cdate;
                            recTemp.LastUpdateBy = CurrentUser.UserId;
                            recTemp.LastUpdateDate = cdate;
                            ctx.LookUpDtls.Add(recTemp);
                            result = ctx.SaveChanges() > 0;
                        }
                        else
                        {
                            oGnMstLookUpDtl.ParaValue = lines[i].Substring(1, 15);
                            oGnMstLookUpDtl.LookUpValueName = lines[i].Substring(16, 100);
                            oGnMstLookUpDtl.LastUpdateBy = CurrentUser.UserId;
                            oGnMstLookUpDtl.LastUpdateDate = cdate;

                            result = ctx.SaveChanges() > 0;
                        }
                    }
                    else
                    {
                        msg ="Invalid Code ID, Tidak terdapat informasinya di gnMstLookUpHdr";
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        #region *** Support Receiving PINVD/PINVS ***
        private void GeneratePINVPerType(string dealerCode, string deliveryNo)
        {
            var query = string.Format(@"exec uspfn_GeneratePINVS '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, dealerCode, deliveryNo);
            ctx.Database.ExecuteSqlCommand(query);
        }
        #endregion

        private GnMstCustomer SelectByStandardCodeCustomer(string standardCode)
        {
            var data = ctx.GnMstCustomers.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.StandardCode == standardCode);
            
            return data;
        }

        private Supplier SelectByStandardCodeSupplier(string standardCode)
        {
            var data = ctx.GnMstSuppliers.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.StandardCode == standardCode);

            return data;
        }
       
    }
}
