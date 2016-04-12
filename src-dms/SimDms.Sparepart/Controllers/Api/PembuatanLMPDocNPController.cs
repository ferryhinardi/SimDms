using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using System.Transactions;
using SimDms.Common;
using System.IO;

namespace SimDms.Sparepart.Controllers.Api
{
    public class PembuatanLMPDocNPController : BaseController
    {

        private string GetCompanyCodeExternal()
        {
            var sql = string.Format("select CompanyCodeTo from spMstCompanyAccount where CompanyCode='{0}' and BranchCodeTo='{1}'", CompanyCode, BranchCode);
            var CompanyCodeExt = ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
            if (string.IsNullOrEmpty(CompanyCodeExt) == null)
            {
                return string.Empty;
            }
            else
            {
                return CompanyCodeExt;
            }
        }

        [HttpPost]
        public JsonResult Save(GetLMPHdr model, string salesType)
        {
            //var result = false;
            using (TransactionScope dbTrans = new TransactionScope())
            {
                try
                {
                    string errMsg = DateTransValidation(model.LmpDate.Value.Date);
                    if (errMsg != "")
                    {
                        return Json(new { success = false, message = errMsg }); ;
                    }

                    var sql = string.Format("exec uspfn_GenerateLampiranNPNew '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'",
                    CompanyCode, BranchCode, model.PickingSlipNo, model.LmpDate.Value.ToString("yyyyMMdd"), CurrentUser.CoProfile.ProductType, CurrentUser.UserId, CurrentUser.TypeOfGoods);
                    var command = ctx.Database.ExecuteSqlCommand(sql);

                    var lmpHdr = ctx.SpTrnSLmpHdrs.Find(CompanyCode, BranchCode, model.LmpNo);
                    if (lmpHdr != null)
                    {
                        if (lmpHdr.TransType == "10" && lmpHdr.isLocked == true)
                        {
                            string CompanyCodeExt = GetCompanyCodeExternal();
                            var compAcc = ctx.spMstCompanyAccounts.Find(CompanyCode, BranchCode);

                            //Move code logic from WCF to here
                            //InsertUtlStockTrfHdr
                            SpUtlStockTrfHdr oSpUtlStockTrfHdr = new SpUtlStockTrfHdr();
                            oSpUtlStockTrfHdr.CompanyCode = CompanyCode;
                            oSpUtlStockTrfHdr.BranchCode = BranchCode;
                            oSpUtlStockTrfHdr.DealerCode = model.CustomerCode;
                            oSpUtlStockTrfHdr.LampiranNo = model.LmpNo;
                            oSpUtlStockTrfHdr.RcvDealerCode = model.CustomerCode;
                            oSpUtlStockTrfHdr.InvoiceNo = "";
                            oSpUtlStockTrfHdr.BinningNo = "";
                            oSpUtlStockTrfHdr.BinningDate = new DateTime(1900, 1, 1);
                            oSpUtlStockTrfHdr.Status = "0";
                            oSpUtlStockTrfHdr.CreatedBy = CurrentUser.UserId;
                            oSpUtlStockTrfHdr.CreatedDate = DateTime.Now;
                            oSpUtlStockTrfHdr.LastUpdateBy = CurrentUser.UserId;
                            oSpUtlStockTrfHdr.LastUpdateDate = DateTime.Now;
                            oSpUtlStockTrfHdr.TypeOfGoods = CurrentUser.TypeOfGoods;

                            //Insert UtlStockTrfDtl
                            ctx.SpUtlStockTrfHdrs.Add(oSpUtlStockTrfHdr);
                            Helpers.ReplaceNullable(oSpUtlStockTrfHdr);

                            SpUtlStockTrfDtl oSpUtlStockTrfDtl = null;
                            var DataLampirans = ctx.SpTrnSLmpDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.LmpNo == model.LmpNo).ToList();
                            foreach (var lampiran in DataLampirans)
                            {
                                //insert into SpUtlStockTrfDtl
                                oSpUtlStockTrfDtl = new SpUtlStockTrfDtl();
                                oSpUtlStockTrfDtl.CompanyCode = CompanyCode;
                                oSpUtlStockTrfDtl.BranchCode = BranchCode;
                                oSpUtlStockTrfDtl.DealerCode = BranchCode;
                                oSpUtlStockTrfDtl.LampiranNo = model.LmpNo;
                                oSpUtlStockTrfDtl.OrderNo = lampiran.DocNo;
                                oSpUtlStockTrfDtl.PartNo = lampiran.PartNo;
                                oSpUtlStockTrfDtl.SalesNo = model.BPSFNo;
                                oSpUtlStockTrfDtl.QtyShipped = lampiran.QtyBill;
                                oSpUtlStockTrfDtl.PartNoShip = lampiran.PartNo;
                                oSpUtlStockTrfDtl.SalesUnit = 1;
                                oSpUtlStockTrfDtl.PurchasePrice = lampiran.CostPrice;
                                oSpUtlStockTrfDtl.CostPrice = lampiran.CostPrice;
                                oSpUtlStockTrfDtl.ProcessDate = new DateTime(1900, 1, 1);
                                oSpUtlStockTrfDtl.ProductType = "0";
                                oSpUtlStockTrfDtl.PartCategory = "";
                                oSpUtlStockTrfDtl.CreatedBy = CurrentUser.UserId;
                                oSpUtlStockTrfDtl.CreatedDate = DateTime.Now;
                                oSpUtlStockTrfDtl.LastUpdateBy = CurrentUser.UserId;
                                oSpUtlStockTrfDtl.LastUpdateDate = DateTime.Now;
                                ctx.SpUtlStockTrfDtls.Add(oSpUtlStockTrfDtl);
                                Helpers.ReplaceNullable(oSpUtlStockTrfDtl);
                            }
                            ctx.SaveChanges();
                        }
                    }

                    string CodeID = "";
                    switch (salesType)
                    {
                        case "1":
                            CodeID = "TTNP";
                            break;
                        case "2":
                            CodeID = "TTSR";
                            break;
                        case "3":
                            CodeID = "TTSL";
                            break;
                        default:
                            break;
                    }
                    var coProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                    sql = string.Format("exec uspfn_GetLmpDoc '{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}'", CompanyCode, BranchCode, TypeOfGoods, salesType+"%", CodeID, coProfileSpare.PeriodBeg.Date.ToString("yyyy/MM/dd"), coProfileSpare.PeriodEnd.Date.ToString("yyyy/MM/dd"));
                    var queryable = ctx.Database.SqlQuery<GetLMPHdr>(sql).AsQueryable().FirstOrDefault();

                    dbTrans.Complete();
                    return Json(new { success = true, message = "Success", data = queryable });
                }
                catch (Exception ex)
                {
                    dbTrans.Dispose();
                    return Json(new { success = false, message = ex.Message.ToString() });
                }
            }
        }

        public JsonResult GetLMP(string SalesType)
        {
            var parameters = SalesType.Split('?');
            var transType = parameters[0];
            string CodeID = "";
            switch (transType)
            {
                case "1":
                    CodeID = "TTNP";
                    break;
                case "2":
                    CodeID = "TTSR";
                    break;
                case "3":
                    CodeID = "TTSL";
                    break;
                default:
                    break;
            }
            transType += "%";
            var coProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            string sql = string.Format("exec uspfn_GetLmpDoc '{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}'", CompanyCode, BranchCode, TypeOfGoods, transType, CodeID, coProfileSpare.PeriodBeg.Date.ToString("yyyy/MM/dd"), coProfileSpare.PeriodEnd.Date.ToString("yyyy/MM/dd"));
            var queryable = ctx.Database.SqlQuery<GetLMPHdr>(sql).AsQueryable();
            return Json(new { success = true, data = queryable });
        }

        public JsonResult GetSpTrnSBPSFDtl(string BPSFNo)
        {
            string sql = string.Format("exec uspfn_spGetSpTrnSBPSFDtl '{0}', '{1}', '{2}'", CompanyCode, BranchCode, BPSFNo);
            var queryable = ctx.Database.SqlQuery<DetailsPesananNP>(sql).AsQueryable();
            return Json(queryable);
        }

        public JsonResult GetSpTrnSLmpDtl(string LmpNo)
        {
            string sql = string.Format("exec uspfn_spGetSpTrnSLmpDtl '{0}', '{1}', '{2}'", CompanyCode, BranchCode, LmpNo);
            var queryable = ctx.Database.SqlQuery<DetailsPesananNP>(sql).AsQueryable();
            return Json(queryable);
        }

        [HttpPost]
        public JsonResult UpdateLampiranDoc(string LmpNo)
        {
            try
            {
                var oSpTrnLmpHdr = ctx.SpTrnSLmpHdrs.Find(CompanyCode, BranchCode, LmpNo);
                if (oSpTrnLmpHdr != null)
                {
                    if (oSpTrnLmpHdr.Status.ToString().Equals("0"))
                    {
                        oSpTrnLmpHdr.Status = "1";
                        oSpTrnLmpHdr.CreatedBy = CurrentUser.UserId;
                        oSpTrnLmpHdr.CreatedDate = DateTime.Now;

                        Helpers.ReplaceNullable(oSpTrnLmpHdr);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }
            return Json(new { success = true, message = "" });
        }

        public ActionResult GenerateTrsfStockFile()
        {
            string LmpNo = (Request["LmpNo"] != null) ? Request["LmpNo"].ToString() : "";
            string BPSFNo = (Request["BPSFNo"] != null) ? Request["BPSFNo"].ToString() : "";
            string fileName = "TSTKD.txt";
            //StreamAppendr sw = new StreamAppendr(fileName);
            StringBuilder sw = new StringBuilder();

            string sql = string.Format("exec uspfn_SelectLMPDtl '{0}', '{1}', '{2}'", CompanyCode, BranchCode, LmpNo);
            var recLmpDtls = ctx.Database.SqlQuery<SelectLMPDtl>(sql).ToList();
            var recordLmpHdr = ctx.SpTrnSLmpHdrs.Find(CompanyCode, BranchCode, LmpNo);

            // Create an instance of StreamAppendr to Append text to a file.
            // The using statement also closes the StreamAppendr.
            string dealerCode = BranchCode.Substring(0, 7);
            //using (sw)
            //{
            // Add some text to the file.
            // Record ID.
            sw.Append(p_GetCharacter("H", 1, false));
            // Data ID.
            sw.Append(p_GetCharacter("TSTKD", 5, false));
            // Dealer Code
            sw.Append(p_GetCharacter(BranchCode, 10, false));
            // Receiving Dealer Code
            sw.Append(p_GetCharacter(recordLmpHdr.CustomerCode, 10, false));

            GnMstCoProfile recordProfile = ctx.GnMstCoProfiles.Find(CompanyCode, BranchCode);
            // Dealer Name
            sw.Append(p_GetCharacter(recordProfile.CompanyName, 50, false));
            // Total Number of Item
            sw.Append(p_GetCharacter(Convert.ToString(recLmpDtls.Count), 6, true));
            // Lampiran No
            sw.Append(p_GetCharacter(recordLmpHdr.LmpNo, 15, false));
            // Lampiran Date                
            sw.Append(p_GetCharacter(p_FormatDateTime(recordLmpHdr.LmpDate.Value.Year.ToString(),
                recordLmpHdr.LmpDate.Value.Month.ToString(), recordLmpHdr.LmpDate.Value.Day.ToString()), 8, false));
            // Picking No
            sw.Append(p_GetCharacter(recordLmpHdr.PickingSlipNo, 15, false));
            // Picking Date
            sw.Append(p_GetCharacter(p_FormatDateTime(recordLmpHdr.PickingSlipDate.Value.Year.ToString(),
                recordLmpHdr.PickingSlipDate.Value.Month.ToString(), recordLmpHdr.PickingSlipDate.Value.Day.ToString()), 8, false));
            // Invoice No
            sw.Append(p_GetCharacter(" ", 15, false));
            // Invoice Date
            sw.Append(p_GetCharacter(" ", 8, false));
            foreach (SelectLMPDtl rec in recLmpDtls)
            {
                sw.AppendLine();
                // Record ID
                sw.Append(p_GetCharacter("1", 1, true));
                // Order No
                sw.Append(p_GetCharacter(rec.DocNo, 15, false));
                // Sales No
                sw.Append(p_GetCharacter(BPSFNo, 6, false));
                // Part Number Order
                sw.Append(p_GetCharacter(rec.PartNoOriginal, 15, false));
                // Part Number to be shipped
                sw.Append(p_GetCharacter(rec.PartNo, 15, false));
                // Shipped Quantity
                sw.Append(p_GetCharacter(rec.QtyBill.ToString(), 9, true));
                // Sales Unit Quantity
                SpMstItemInfo recordItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, rec.PartNo);
                sw.Append(p_GetCharacter(recordItemInfo.SalesUnit.ToString(), 3, true));
                // Purchase Price
                SpTrnSBPSFDtl recordDtl = ctx.SpTrnSBPSFDtls.Find(CompanyCode, BranchCode,
                    BPSFNo, "00", rec.PartNo, rec.PartNoOriginal,
                    rec.DocNo);
                sw.Append(p_GetCharacter(recordDtl.CostPrice.ToString(), 10, true));
                // Cost Price
                sw.Append(p_GetCharacter(recordDtl.CostPrice.ToString(), 10, true));
                // Process Date
                sw.Append(p_GetCharacter(p_FormatDateTime(recordLmpHdr.LmpDate.Value.Year.ToString(),
                    recordLmpHdr.LmpDate.Value.Month.ToString(), recordLmpHdr.LmpDate.Value.Day.ToString()), 8, false));
                // Blank Filler
                sw.Append(p_GetCharacter(" ", 59, false));
            }
            //}

            var bytesFile = Encoding.UTF8.GetBytes(sw.ToString());
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            using (MemoryStream MyMemoryStream = new MemoryStream(bytesFile))
            {
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                Response.Close();

                return File(Response.OutputStream, Response.ContentType);
            }
        }

        public JsonResult GetCustomerShip(string CustomerCodeShip)
        {
            var data = "";
            var records = ctx.GnMstCustomers.Find(CompanyCode, CustomerCodeShip);

            return Json(new
            {
                data = new
                {
                    CustomerNameShip = records != null ? records.CustomerName : "",
                    Address1 = records != null ? records.Address1 : "",
                    Address2 = records != null ? records.Address2 : "",
                    Address3 = records != null ? records.Address3 : "",
                    Address4 = records != null ? records.Address4 : "",
                }
            });
        }

        public JsonResult GetCustomerBill(string CustomerCodeBill)
        {
            var data = "";
            var records = ctx.GnMstCustomers.Find(CompanyCode, CustomerCodeBill);

            return Json(new
            {
                data = new
                {
                    CustomerNameTagih = records != null ? records.CustomerName : "",
                    Address1Tagih = records != null ? records.Address1 : "",
                    Address2Tagih = records != null ? records.Address2 : "",
                    Address3Tagih = records != null ? records.Address3 : "",
                    Address4Tagih = records != null ? records.Address4 : "",
                }
            });
        }

        #region -- Private Method --
        private string p_GetCharacter(string szCharacter, int iLength, bool bWriteInteger)
        {
            string szEmpty = " ";

            if (bWriteInteger)
                szEmpty = "0";

            string szEmptyCharacter = "";
            if (szCharacter.Length < iLength)
            {
                for (int i = 0; i < iLength - szCharacter.Length; i++)
                {
                    szEmptyCharacter = szEmptyCharacter + szEmpty;
                }

                if (bWriteInteger)
                    szCharacter = szEmptyCharacter + szCharacter;
                else
                    szCharacter = szCharacter + szEmptyCharacter;
            }
            else
                szCharacter = szCharacter.Substring(0, iLength);

            return szCharacter;
        }

        private string p_FormatDateTime(string szYear, string szMonth, string szDate)
        {
            if (szMonth.Length == 1) szMonth = "0" + szMonth;
            if (szDate.Length == 1) szDate = "0" + szDate;

            return szYear + szMonth + szDate;
        }

        #endregion
    }
}
