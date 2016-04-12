using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class StockTakingPrintController : BaseController
    {
        
        public JsonResult FisrtLoad()
        {
            
            var query = string.Format(@"
            SELECT TOP 1 * FROM spTrnStockTakingHdr 
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND (Status = 1 OR Status = 0)", CompanyCode, BranchCode);

            var recordHeader = ctx.Database.SqlQuery<SpTrnStockTakingHdr>(query).FirstOrDefault();
            if (recordHeader != null)
            {
                var record = ctx.LookUpDtls.Find(CompanyCode, "WRCD", recordHeader.WarehouseCode);
                return Json(new
                {
                    success = true,
                    data = record
                });
            }
            else {
                return Json(new
                {
                   success = false
                });
            }
            
        }

        public JsonResult Proses() 
        {
            int result = 0;
            var pesan = "";
            var query = string.Format(@"
            SELECT TOP 1 * FROM spTrnStockTakingHdr 
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND (Status = 1 OR Status = 0)", CompanyCode, BranchCode);
            var recordHeader = ctx.Database.SqlQuery<SpTrnStockTakingHdr>(query).FirstOrDefault();

            var q1 = string.Format(@"
            EXEC sp_IsValidSTAnalyze '{0}', '{1}', '{2}'", CompanyCode, BranchCode, recordHeader.STHdrNo);

            var dt = ctx.Database.SqlQuery<IsValidSTAnalyze>(q1).ToList();
            if (dt.Count > 0)
            {
                return Json(new { success = false, message = "Proses Cetak Analisa Stok Taking gagal !!!" });
            }

            try
            {
                result = StockTakingAnalyze(recordHeader);
                switch (result)
                {
                    case 1:
                        pesan = "Cetak Analisa Stock Taking Berhasil";
                        return Json(new { success = true, message = pesan, data = string.Empty });
                        
                    case 2:
                        pesan ="Terdapat Duplikasi Part No sebagai Lokasi Utama";
                        return Json(new { success = false, message = pesan, data = string.Empty });
                        
                    case 3:
                        pesan = "Terdapat part no yang lokasi utama belum dilakukan entry inventory";
                        return Json(new { success = false, message = pesan, data = string.Empty });
                        
                    case 4:
                        pesan = "Terdapat Data Location Part Melebihi Batas";
                        var dat = IsOutOfLocation(recordHeader.STHdrNo);
                        return Json(new { success = false, message = pesan, data = dat });
                        
                    case 5:
                        pesan = "Terdapat Data Part No yang belum dicetak";
                        return Json(new { success = false, message = pesan, data = string.Empty });
                        
                    default:
                        pesan = "Update Gagal";
                        return Json(new { success = false, message = pesan, data = string.Empty });
                        
                }
            }
            catch (Exception e) {
                return Json(new { success = false, message = e.Message });
            }
        }

        private List<IsValidSTAnalyze> IsOutOfLocation(string STHdrNo)
        {
            var query = string.Format(@"
            SELECT a.PartNo
	, a.STNo
	, a.SeqNo
	, 'Jumlah Lokasi : ' +  Convert(VARCHAR,b.couError) Status  
FROM SpTrnStockTakingDtl a 
LEFT JOIN 
(
	SELECT CompanyCode, BranchCode, STHdrNo, PartNo, COUNT(IsMainLocation) couError 
	FROM			
		spTrnStockTakingDtl 
    WHERE 
        CompanyCode = '{0}'
        AND BranchCode = '{1}'
        AND STHdrNo = '{2}'    
    GROUP BY CompanyCode, BranchCode, STHdrNo, PartNo
) b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.STHdrNo = b.STHdrNo
		AND a.PartNo = b.PartNo
WHERE
    a.CompanyCode = '{0}'
    AND a.BranchCode = '{1}'
    AND a.STHdrNo = '{2}'
    AND b.couError > 7 ORDER BY a.PartNo, a.STNO ASC
            ", CompanyCode, BranchCode, STHdrNo);
            return ctx.Database.SqlQuery<IsValidSTAnalyze>(query).ToList();
        }

        private int StockTakingAnalyze(SpTrnStockTakingHdr recordHeader)
        {
            int result = 0;
            bool upSuccess = false;
            try
            {
                result = StockTakingAnalyzeDtl(recordHeader);
                if (result == 1)
                {
                    upSuccess = StockAnalSuccess(recordHeader);
                    if (!upSuccess)
                        result = 0;
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        private bool StockAnalSuccess(SpTrnStockTakingHdr recordHeader)
        {
            bool result = false;
            var dtDetail = ctx.SpTrnStockTakingDtls.Where(a=>a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.STHdrNo == recordHeader.STHdrNo).ToList();
            decimal counterRows = dtDetail.Count;
            int counterIndex = 0;

            foreach (var dtRow in dtDetail)
            {
                counterIndex++;
                var recordDetail = ctx.SpTrnStockTakingDtls.Find(CompanyCode, BranchCode, recordHeader.STHdrNo, dtRow.STNo.ToString(), decimal.Parse(dtRow.SeqNo.ToString()));
                if (recordDetail == null) {
                    recordDetail = new SpTrnStockTakingDtl() { 
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        STHdrNo = recordHeader.STHdrNo,
                        STNo = dtRow.STNo.ToString(),
                        SeqNo = decimal.Parse(dtRow.SeqNo.ToString())
                    };
                    ctx.SpTrnStockTakingDtls.Add(recordDetail);
                }

                recordDetail.LastUpdatedBy = CurrentUser.UserId;
                recordDetail.LastUpdatedDate = DateTime.Now;
                recordDetail.Status = "1";
                recordDetail.PrintSeq += 1;

                try
                {
                    ctx.SaveChanges();
                    result = true;
                }
                catch {
                    result = false;
                }
            }
            return result;
        }

        private int StockTakingAnalyzeDtl(SpTrnStockTakingHdr recordHeader)
        {
            int result = 0;
            var partSelection = ctx.SpTrnStockTakingDtls.Where(a=>a.CompanyCode == CompanyCode&&a.BranchCode == BranchCode && a.STHdrNo == recordHeader.STHdrNo).ToList();
            decimal counterRows = partSelection.Count;
            int counterIndex = 0;
            foreach (var partRow in partSelection)
            {
                counterIndex++;
                if (recordHeader.Condition != "3")
                {
                    // NOTES : Check Part Main Location Error
                    var partCheck = ctx.SpTrnStockTakingDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.STHdrNo == recordHeader.STHdrNo && a.PartNo == partRow.PartNo).ToList();
                    if (partCheck.Count == 0)
                    {
                        result = 3;
                        break;
                    }
                    else if (partCheck.Count > 1)
                    {
                        result = 2;
                        break;
                    }
                    else result = 1;
                }

                // NOTES : Check Part Quantity Location
                var partCheckLoc = ctx.SpTrnStockTakingDtls.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.STHdrNo == recordHeader.STHdrNo && a.PartNo == partRow.PartNo).ToList();
                if (partCheckLoc.Count > 7)
                {
                    result = 4;
                    break;
                }
                else
                    result = 1;
            }
            return result;
        }

        

    }
}
