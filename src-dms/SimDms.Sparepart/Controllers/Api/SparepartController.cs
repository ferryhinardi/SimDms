using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.Models;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{

    public class SpInquiryController : BaseController
    {
        public JsonResult Default()
        {
            var curDate = DateTime.Now.Date.ToString("yyyy-MM-dd hh:mm");
            var curDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            return Json(new { currentDate = curDate, currentDateTime = curDateTime });
        }
        
        public JsonResult ISpSupplierKontrak(string spCode)
        {

            var result = ctx.Database.SqlQuery<System.Decimal>("exec [uspfn_SpMstItemCheckPartCount]  @CompanyCode=@p0, @BranchCode=@p1, @PartNo=@p2", CompanyCode, BranchCode, spCode);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //Load Default Report -> Reports -> Kartu Stock
        public JsonResult DefaultReportKartuStock()
        {
            var now = DateTime.Now;
            return Json(new { Month = now.Month, Year = now.Year });
        }

        public JsonResult UpdateRangeLampiran(string firstLmp, string lastLmp, string salesType)
        {
            try
            {
                string query = string.Format(
                    @"UPDATE SpTrnSLmpHdr 
                    SET Status = 1 
                        , PrintSeq = PrintSeq + 1
                        , LastUpdateBy = '{6}'
                        , LastUpdateDate = GetDate()
                    WHERE
                        CompanyCode = '{0}'
                        AND BranchCode = '{1}'
                        AND LmpNo BETWEEN '{2}' AND '{3}' 
                        AND SUBSTRING(TransType,1,1) = '{4}'
                        AND TypeOfGoods = '{5}'
                        ", CompanyCode, BranchCode, firstLmp, lastLmp, salesType, TypeOfGoods, CurrentUser.UserId);

               if(ctx.Database.ExecuteSqlCommand(query) > 0){
                   return Json(new { success = true });
               }
               else{
                   return Json(new { success = false, message = "Daftar Lampiran Dokumen Nonpenjualan tidak ditemukan" });
               }
            }
            catch(Exception ex){
                   return Json(new { success = false, message = ex.Message });

            }
        }

        public JsonResult checkAOS()
        {
            string sQry = "";
            if (!IsBranch)
            {
                sQry = @"select Branch, BranchName, POSNo, POSDate, PORDS
                            from ( select Branch=d.OutletCode, BranchName=d.OutletAbbreviation, 
                                    POSNo=(select top 1 p.POSNo from spTrnPPOSHdr p
                                            where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc),
                                    POSDate=convert(varchar, (select top 1 p.POSDate from spTrnPPOSHdr p
                                               where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc), 106),
                                    PORDS=(select top 1 p.isGenPORDD from spTrnPPOSHdr p
                                            where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc)
                                              from gnMstDealerOutletMapping d ) a
                            where POSDate is not null 
                            order by Branch";
            }
            else
            {
                sQry = @"select Branch, BranchName, POSNo, POSDate, PORDS
                            from ( select Branch=d.OutletCode, BranchName=d.OutletAbbreviation, 
                                    POSNo=(select top 1 p.POSNo from spTrnPPOSHdr p
                                            where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc),
                                    POSDate=convert(varchar, (select top 1 p.POSDate from spTrnPPOSHdr p
                                               where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc), 106),
                                    PORDS=(select top 1 p.isGenPORDD from spTrnPPOSHdr p
                                            where p.CompanyCode=d.DealerCode and p.BranchCode=d.OutletCode
                                                    and p.OrderType='8' and p.isDeleted=0
                                               order by p.POSDate desc)
                                              from gnMstDealerOutletMapping d ) a
                            where POSDate is not null and Branch = '" + BranchCode + "' order by Branch";
            }
            var aos = ctx.Database.SqlQuery<mdlAOS>(sQry).AsQueryable();
            if (aos.Count() > 0)
            {
                return Json(new { success = true, data = aos });
            }
            return Json(new { success = false });
        }

        private class mdlAOS
        {
            public string Branch { get; set; }
            public string BranchName { get; set; }
            public string POSNo { get; set; }
            public string POSDate { get; set; }
            public Boolean PORDS { get; set; }

        }
    }
}