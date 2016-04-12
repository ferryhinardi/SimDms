using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Sales.BLL;

namespace SimDms.Sales.Controllers.Api
{
    public class PerlengkapanOutController : BaseController
    {
        public JsonResult Save(omTrSalesPerlengkapanOut mdl, bool chkReffDate)
        {
            try
            {
                var recordHdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (recordHdr == null)
                {
                    recordHdr = new omTrSalesPerlengkapanOut();
                    recordHdr.CompanyCode = CompanyCode;
                    recordHdr.BranchCode = BranchCode;
                    recordHdr.PerlengkapanDate = mdl.PerlengkapanDate;
                    recordHdr.PerlengkapanNo = GetNewDocumentNo("PLK", (DateTime)mdl.PerlengkapanDate);
                    recordHdr.CreatedBy = CurrentUser.UserId;
                    recordHdr.CreatedDate = ctx.CurrentTime;
                    ctx.omTrSalesPerlengkapanOuts.Add(recordHdr);
                }
                recordHdr.ReferenceNo = mdl.ReferenceNo;
                if (chkReffDate)
                {
                    recordHdr.ReferenceDate = mdl.ReferenceDate;
                }
                else
                {
                    recordHdr.ReferenceDate = Convert.ToDateTime("1900/01/01");
                }
                recordHdr.PerlengkapanType = mdl.PerlengkapanType;
                recordHdr.SourceDoc = mdl.SourceDoc;
                recordHdr.CustomerCode = mdl.CustomerCode;
                recordHdr.Remark = mdl.Remark;
                recordHdr.Status = "0";
                recordHdr.LastUpdateBy = CurrentUser.UserId;
                recordHdr.LastUpdateDate = ctx.CurrentTime;

                ctx.SaveChanges();
                return Json(new { success = true, PerlengkapanNo = recordHdr.PerlengkapanNo, Status = "0", StatusDsc = getStringStatus("0") });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(omTrSalesPerlengkapanOut mdl)
        {
            try
            {
                var recordHdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (recordHdr != null)
                {
                    if (recordHdr.Status != "0" && recordHdr.Status != "1")
                    {
                        return Json(new { success = false, message = "Invalid Status" });
                    }

                    if (OmTrSalesBLL.Instance(CurrentUser.UserId).plkpDeleteAll(recordHdr))
                    {
                        recordHdr.Status = "3";
                        recordHdr.LastUpdateBy = CurrentUser.UserId;
                        recordHdr.LastUpdateDate = ctx.CurrentTime;
                    }
                    ctx.SaveChanges();
                    return Json(new { success = true, PerlengkapanNo = recordHdr.PerlengkapanNo, Status = recordHdr.Status, StatusDsc = getStringStatus(recordHdr.Status) });
                }
                return Json(new { success = false, message = "Gagal Hapus Data!" }); ;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Print(omTrSalesPerlengkapanOut mdl)
        {
            try
            {
                var hdr=ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (hdr == null)
                {
                    return Json(new { success = false, message = "Invalin PerlengkapanOut" });                                   
                }

                if (ctx.OmTrSalesPerlengkapanOutDetails
                           .Count(x => x.CompanyCode == CompanyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.PerlengkapanNo == mdl.PerlengkapanNo) < 1)
                {
                    return Json(new { success = false, message = "Tidak ada data yang dicetak !" });                                   
                }

                hdr.Status = "1";
                hdr.LastUpdateBy = CurrentUser.UserId;
                hdr.LastUpdateDate=ctx.CurrentTime;
                ctx.SaveChanges();
                return Json(new { success = true, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });                  
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Approve(omTrSalesPerlengkapanOut mdl)
        {
            try
            {
                var recordHdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (recordHdr != null)
                {
                    if (OmTrSalesBLL.Instance(CurrentUser.UserId).plkpdtlApprove(recordHdr))
                    {
                        recordHdr.Status = "2";
                        recordHdr.LastUpdateBy = CurrentUser.UserId;
                        recordHdr.LastUpdateDate = ctx.CurrentTime;
                        ctx.SaveChanges();
                    }                    
                    return Json(new { success = true, Status = recordHdr.Status, StatusDsc = getStringStatus(recordHdr.Status) });                  
                }
                else
                {
                    return Json(new { success = false, message = "Invalin PerlengkapanOut" });
                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message,umsg="Proses Approve Gagal"});
            }
        }

        public JsonResult savedetailmodel(OmTrSalesPerlengkapanOutModel mdl)
        {
            try
            {
                var hdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (hdr.Status != "0" && hdr.Status != "1")
                {
                    return Json(new { success = false, message = "Invalid Status" }); ;
                }

                var plkpout = OmTrSalesBLL.Instance(CurrentUser.UserId).Select4LookupModel(hdr);
                plkpout.Where(x => x.SalesModelCode == mdl.SalesModelCode).FirstOrDefault();
                if (plkpout == null)
                {
                    return Json(new { success = false, message = "Invalid SalesModelCode" }); ;
                }

                bool isNew = false;
                var recordModel = ctx.OmTrSalesPerlengkapanOutModels.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo, mdl.SalesModelCode);
                if (recordModel == null)
                {
                    isNew = true;
                    recordModel = new OmTrSalesPerlengkapanOutModel();
                    recordModel.CompanyCode = CompanyCode;
                    recordModel.BranchCode = BranchCode;
                    recordModel.PerlengkapanNo = mdl.PerlengkapanNo;
                    recordModel.SalesModelCode = mdl.SalesModelCode;
                    recordModel.CreatedBy = CurrentUser.UserId;
                    recordModel.CreatedDate = ctx.CurrentTime;
                    ctx.OmTrSalesPerlengkapanOutModels.Add(recordModel);
                }

                recordModel.Quantity = mdl.Quantity;
                recordModel.Remark = mdl.Remark;
                recordModel.LastUpdateBy = CurrentUser.UserId;
                recordModel.LastUpdateDate = ctx.CurrentTime;
                hdr.Status = "0";
                hdr.LastUpdateBy = CurrentUser.UserId;
                hdr.LastUpdateDate = ctx.CurrentTime;
                ctx.SaveChanges();
                return Json(new { success = true, Status=hdr.Status,StatusDsc= getStringStatus(hdr.Status)});

                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }

        }

        public JsonResult deleteDetilmodel(OmTrSalesPerlengkapanOutModel mdl)
        {
            try
            {
                var hdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (hdr.Status != "0" && hdr.Status != "1")
                {
                    return Json(new { success = false, message = "Invalid Status" }); ;
                }               

                var recordModel = ctx.OmTrSalesPerlengkapanOutModels.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo, mdl.SalesModelCode);
                if (recordModel != null)
                {
                    var cekDetail = OmTrSalesBLL.Instance(CurrentUser.UserId).CekPerlengkapan(mdl);
                    if (cekDetail.Count() > 0)
                    {
                        return Json(new { success = false, message = "Hapus terlebih dahulu data detail perlengkapan !" }); ;
                    }
                    else
                    {
                        ctx.OmTrSalesPerlengkapanOutModels.Remove(recordModel);
                        hdr.Status = "0";
                        hdr.LastUpdateBy = CurrentUser.UserId;
                        hdr.LastUpdateDate = ctx.CurrentTime;
                        ctx.SaveChanges();
                    }

                    return Json(new { success = true, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });
                }
                return Json(new { success = false, message = "data not found!" }); ;
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }); ;
            }
        }


        public JsonResult savedetailperlengkapan(OmTrSalesPerlengkapanOutDetail mdl)
        {
            try
            {
                var hdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (hdr.Status != "0" && hdr.Status != "1")
                {
                    return Json(new { success = false, message = "Invalid Status" }); ;
                }

                if (ctx.Database.SqlQuery<omSlsPrlgkpnOutLkpMdlDtl>(string.Format("exec uspfn_omSlsPrlgkpnOutLkpMdlDtl {0},{1},{2}", CompanyCode, BranchCode, mdl.SalesModelCode))
                    .Where(x => x.PerlengkapanCode == mdl.PerlengkapanCode)
                    .FirstOrDefault() == null)
                {
                    return Json(new { success = false, message = "Invalid Perlengkapan Code" }); ;
                }

                if (mdl.Quantity < 1)
                {
                    return Json(new { success = false, message = "Invalid Quantity " }); ;
                }

                var recordDtl = ctx.OmTrSalesPerlengkapanOutDetails
                                .Find(CompanyCode, BranchCode, mdl.PerlengkapanNo, mdl.SalesModelCode, mdl.PerlengkapanCode);

                if (recordDtl == null)
                {
                    
                    recordDtl = new OmTrSalesPerlengkapanOutDetail();
                    recordDtl.CompanyCode = CompanyCode;
                    recordDtl.BranchCode = BranchCode;
                    recordDtl.SalesModelCode = mdl.SalesModelCode;
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = ctx.CurrentTime;
                    ctx.OmTrSalesPerlengkapanOutDetails.Add(recordDtl);
                }
                recordDtl.PerlengkapanCode = mdl.PerlengkapanCode;
                recordDtl.PerlengkapanNo = mdl.PerlengkapanNo;
                recordDtl.QuantityStd = mdl.QuantityStd;
                recordDtl.Quantity = mdl.Quantity;
                recordDtl.Remark = mdl.Remark;
                recordDtl.LastUpdateBy = CurrentUser.UserId;
                recordDtl.LastUpdateDate = ctx.CurrentTime;

                hdr.Status = "0";
                hdr.LastUpdateBy = CurrentUser.UserId;
                hdr.LastUpdateDate = ctx.CurrentTime;

                ctx.SaveChanges();

                return Json(new { success = true, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }); ;
            }
      
        }

        public JsonResult deletedtlperlenglapan(OmTrSalesPerlengkapanOutDetail mdl)
        {
            try
            {
                var hdr = ctx.omTrSalesPerlengkapanOuts.Find(CompanyCode, BranchCode, mdl.PerlengkapanNo);
                if (hdr.Status != "0" && hdr.Status != "1")
                {
                    return Json(new { success = false, message = "Invalid Status" }); ;
                }

                var recordPerlengkapan = ctx.OmTrSalesPerlengkapanOutDetails
                                        .Find(CompanyCode, BranchCode, mdl.PerlengkapanNo, mdl.SalesModelCode, mdl.PerlengkapanCode);
                if (recordPerlengkapan != null)
                {
                    ctx.OmTrSalesPerlengkapanOutDetails.Remove(recordPerlengkapan);
                    hdr.Status = "0";
                    hdr.LastUpdateBy = CurrentUser.UserId;
                    hdr.LastUpdateDate = ctx.CurrentTime;
                    ctx.SaveChanges();
                    return Json(new { success = true, Status = hdr.Status, StatusDsc = getStringStatus(hdr.Status) });
                }
                else
                {
                    return Json(new { success = false, message = "Data Not found" }); ;
                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }); ;
            }
        }
    }
}
