using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class InquiryPaketServiceController : BaseController
    {
        public JsonResult DetailPackage(string packageCode, string BasicModel)
        {
            var query = string.Format(@"SELECT svMstPackageTask.OperationNo
                , svMstPackageTask.DiscPct DiscTask
                , svMstPackagePart.PartNo
                , spMstItemInfo.PartName
                , svMstPackagePart.DiscPct DiscPart    
            FROM svMstPackageTask
            LEFT JOIN svMstPackagePart ON
                svMstPackageTask.CompanyCode = svMstPackagePart.CompanyCode
                AND svMstPackageTask.PackageCode = svMstPackagePart.PackageCode
                AND svMstPackageTask.BasicModel = svMstPackagePart.BasicModel
                --AND svMstPackageTask.OperationNo = svMstPackagePart.OperationNo
            LEFT JOIN spMstItemInfo ON
                spMstItemInfo.CompanyCode = svMstPackageTask.CompanyCode 
                AND spMstItemInfo.PartNo = svMstPackagePart.PartNo
            WHERE svMstPackageTask.CompanyCode = '{0}'
                AND svMstPackageTask.PackageCode = '{1}'
                AND svMstPackageTask.BasicModel = '{2}'
            ", CompanyCode, packageCode, BasicModel);

            var query2 = string.Format(@"SELECT svMstPackageContract.CustomerCode
                , gnMstCustomer.CustomerName
                , svMstPackageContract.PoliceRegNo
                , svMstCustomerVehicle.ServiceBookNo
                , svMstPackageContract.ChassisCode
                , svMstPackageContract.ChassisNo    
                , svMstPackageContract.BeginDate
                , svMstPackageContract.EndDate
                , svMstPackageContract.VirtualAccount
            FROM svMstPackage
            LEFT JOIN svMstPackageContract ON
                svMstPackageContract.CompanyCode = svMstPackage.CompanyCode
                AND svMstPackageContract.PackageCode = svMstPackage.PackageCode
            LEFT JOIN gnMstCustomer ON
                svMstPackageContract.CompanyCode = gnMstCustomer.CompanyCode
                AND svMstPackageContract.CustomerCode = gnMstCustomer.CustomerCode
            INNER JOIN svMstCustomerVehicle ON
	            svMstPackageContract.CompanyCode = svMstCustomerVehicle.CompanyCode
	            AND svMstPackageContract.ChassisCode = svMstCustomerVehicle.ChassisCode
	            AND svMstPackageContract.ChassisNo = svMstCustomerVehicle.ChassisNo
            WHERE svMstPackage.CompanyCode = '{0}'
                AND svMstPackage.PackageCode = '{1}'
            ", CompanyCode, packageCode);

            var sqlstr = ctx.Database.SqlQuery<DetailPackage>(query).AsQueryable();
            var sqlstr2 = ctx.Database.SqlQuery<DetailPackage>(query2).AsQueryable();

            return Json(new { success = true, data = sqlstr, data2 = sqlstr2 });
        }

    }
}
