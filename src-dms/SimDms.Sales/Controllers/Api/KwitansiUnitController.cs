using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class KwitansiUnitController : BaseController
    {
        public JsonResult SetNoKwitansi()
        {
            var msg = "";
            var DocNo = "";
            var record = ctx.GnMstDocuments.Find( CompanyCode, BranchCode , "KUN");
            if (record == null)
            {
                msg = "Belum tersedia document type untuk module ini";
            }
            else
            {
                GnMstDocument doc = ctx.GnMstDocuments.Find(CompanyCode, BranchCode, "KUN");
                if ((int)doc.DocumentSequence == 0)
                {
                    DocNo = "KUN/" + DateTime.Now.Year.ToString().Remove(0, 2) + "/000001";
                }
                else
                {
                    decimal row = doc.DocumentSequence + 1;
                    DocNo = "KUN/" + DateTime.Now.Year.ToString().Remove(0, 2) + "/" + row.ToString().PadLeft(6, '0');
                }

                //DocNo = GetNewDocumentNo("KUN", DateTime.Now);
            }
            return Json(new { success = true, msg = msg, DocNo = DocNo });
        }



       public JsonResult Save(OmTrSalesReceipt model)
       {
            string msg = "";
            var record = ctx.OmTrSalesReceipt.Find(CompanyCode, BranchCode, model.ReceiptNo, model.ChassisCode, model.ChassisNo, model.EngineCode, model.EngineNo);

            var query = string.Format(@"
               select CustomerCode
                    from OmMstVehicle a
                    left join OmTrSalesSO b on
	                    a.CompanyCode = b.CompanyCode
	                    and a.SONo = b.SONo
                    where a.CompanyCode = '{0}'
                    and a.ChassisNo = '{1}'
                    and a.ChassisCode = '{2}'
                    and a.EngineNo = '{3}'
                    and a.EngineCode = '{4}'
				", CompanyCode, model.ChassisNo, model.ChassisCode, model.EngineNo, model.EngineCode);

            var queryable = ctx.Database.SqlQuery<OmTrSalesReceiptView>(query).AsQueryable();

            if (record == null)
            {
                record = new OmTrSalesReceipt
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReceiptNo = GetNewDocumentNo("KUN", DateTime.Now),
                    ChassisCode = model.ChassisCode,
                    ChassisNo = model.ChassisNo,
                    EngineCode = model.EngineCode,
                    EngineNo = model.EngineNo,
                    CustomerCode = model.CustomerCode,
                    CustomerName = model.CustomerName,
                    Description = model.Description,
                    Amount = model.Amount,
                    FakturPolisiNo = model.FakturPolisiNo,
                    ColourCode = model.ColourCode,
                    ColourDescription = model.ColourDescription,
                    PrintSeq = '0',
                    ReceiptStatus = "0",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    UpdateBy = CurrentUser.UserId,
                    UpdateDate = ctx.CurrentTime
                };
                ctx.OmTrSalesReceipt.Add(record);
            }
            else
            {
                    record.CustomerCode = queryable.FirstOrDefault().CustomerCode;
                    record.CustomerName = model.CustomerName;
                    record.Description = model.Description;
                    record.Amount = model.Amount;
                    record.FakturPolisiNo = model.FakturPolisiNo;
                    record.ColourCode = model.ColourCode;
                    record.ColourDescription = model.ColourDescription;
                    record.UpdateBy = CurrentUser.UserId;
                    record.UpdateDate = ctx.CurrentTime;
                //ctx.MstRefferences.Add(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

       public JsonResult closed(OmTrSalesReceipt model)
       {

           ctx.Database.ExecuteSqlCommand(@"Update OmTrSalesReceipt 
                                            set ReceiptStatus = '2' 
                                          WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and ReceiptNo='" + model.ReceiptNo +
                                             "' and ChassisCode='" + model.ChassisCode + "' and ChassisNo='" + model.ChassisNo +
                                             "' and EngineCode='" + model.EngineCode + "' and EngineNo='" + model.EngineNo + "'");

           try
           {
               ctx.SaveChanges();

               return Json(new { success = true});
           }
           catch (Exception ex)
           {
               return Json(new { success = false, message = ex.Message });
           }
       }

    }
}
