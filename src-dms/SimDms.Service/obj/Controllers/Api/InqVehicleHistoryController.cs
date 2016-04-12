using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using System.Text;
using System.IO;

namespace SimDms.Service.Controllers.Api
{
    public class InqVehicleHistoryController : BaseController
    {
        public JsonResult GetVehicleHistory(string PoliceRegNo, string ServiceBookNo, string CustomerCode, string ChassisCode, string ChassisNo, string EngineCode, string EngineNo, string BasicModel, bool isSarviceDate, string SarviceDate, bool isBranch)
        //public JsonResult GetVehicleHistory(VehicleHistory model)    
        {
            var isSarvice = isSarviceDate == false ? 0 : 1;
            var isAllBranch = isBranch == false ? 0 : 1;
            var ServiceDate = SarviceDate == "" ? "1/1/1900" : SarviceDate == "undefined" ? "1/1/1900" : SarviceDate;

            var query = string.Format(@"uspfn_SvInqVehicleHistory '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'
            ", CompanyCode, BranchCode, ProductType, PoliceRegNo, ServiceBookNo, CustomerCode, ChassisCode, ChassisNo
             , EngineCode, EngineNo, BasicModel, isSarvice, ServiceDate, isAllBranch);

            var sqlstr = ctx.Database.SqlQuery<InqVehicleHistory>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetVehicleInfo(string PoliceRegNo, string ChassisCode, string ChassisNo, string BasicMode, string customerCode, bool isBranch)
        {
            var isAllBranch = isBranch == false ? 0 : 1;
            var JobOrderDate = "01011900";

//            var query = string.Format(@"uspfn_GetVehicleInfo '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
//            ", CompanyCode, BranchCode, ProductType, PoliceRegNo, ChassisCode, ChassisNo, BasicMode, JobOrderDate, customerCode, isAllBranch);

            var query = string.Format(@"uspfn_GetVehicleInfo_New '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
            ", CompanyCode, BranchCode, ProductType, PoliceRegNo, ChassisCode, ChassisNo, BasicMode, JobOrderDate, customerCode, isAllBranch);

            var sqlstr = ctx.Database.SqlQuery<GetVehicleInfo>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetSubVehicleInfo(string PoliceRegNo, string ChassisCode, string ChassisNo, string BasicMode, string customerCode, bool isBranch, string JobOrderNo)
        {
            var isAllBranch = isBranch == false ? 0 : 1;
            var JobOrderDate = "01011900";

            var query = string.Format(@"uspfn_GetVehicleInfo_SubNew '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '{10}'
            ", CompanyCode, BranchCode, ProductType, PoliceRegNo, ChassisCode, ChassisNo, BasicMode, JobOrderDate, customerCode, isAllBranch, JobOrderNo);

            var sqlstr = ctx.Database.SqlQuery<GetVehicleInfo>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetVehicleHistoryWSDS(string PoliceRegNo, string BasicModel, string CustomerCode, string CustomerName, string ChassisCode, string ChassisNo, string EngineCode, string EngineNo)
        {

            var query = string.Format(@"uspfn_SvInqVehicleHistoryWSDS '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
            ", CompanyCode, BranchCode, PoliceRegNo, BasicModel, CustomerCode, CustomerName, ChassisCode, ChassisNo, EngineCode, EngineNo);

            var sqlstr = ctx.Database.SqlQuery<InqVehicleHistoryWSDS>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult GetVehicleInfoWSDS(string PoliceRegNo)
        {

            var query = string.Format(@"
        select a.InvoiceNo ,a.InvoiceDate ,a.FPJNo ,a.FPJDate ,a.JobType ,a.Odometer ,c.MechanicID ,c.ChiefMechanicID ,b.OperationNo 
        ,b.OperationHour ,b.OperationCost ,'' Description
          from svHstVehicle a
          left join svHstVhcTask b
            on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.ProductType = a.ProductType and b.InvoiceNo = a.InvoiceNo
          left join svHstVhcMechanic c
            on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.ProductType = a.ProductType and c.InvoiceNo = a.InvoiceNo 
            and c.OperationNo = b.OperationNo
         where 1 = 1 and a.CompanyCode = '{0}' and a.BranchCode = '{1}' and a.ProductType = '{2}' and a.PoliceRegNo = '{3}'

        union all

        select a.InvoiceNo ,a.InvoiceDate ,a.FPJNo  ,a.FPJDate ,a.JobType ,a.Odometer ,'' ,'' ,b.PartNo ,(b.SupplyQty - b.ReturnQty) ,b.RetailPrice  ,''
          from svHstVehicle a
          left join svHstVhcItem b
            on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode  and b.ProductType = a.ProductType and b.InvoiceNo = a.InvoiceNo
         where 1 = 1 and a.CompanyCode = '{0}' and a.BranchCode = '{1}' and a.ProductType = '{2}' and a.PoliceRegNo = '{3}'
            ", CompanyCode, BranchCode, ProductType, PoliceRegNo);

            var sqlstr = ctx.Database.SqlQuery<GetVehicleInfo>(query).AsQueryable();
            return Json(sqlstr);
        }


    }
}
