using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.General.Models.Others;
using GeLang;

namespace SimDms.General.Controllers.Api
{
    public class LookupController : BaseController
    {
        [HttpPost]
        public JsonResult Customers()
        {
            ResultModel result = InitializeResultModel();
            string filterCustomerCode = Request["filterCustomerCode"] ?? "";
            string filterCustomerName = Request["filterCustomerName"] ?? "";

            var data = ctx.Database.SqlQuery<LookupCustomer>("exec uspfn_LookupCustomer @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterCustomerCode))
            {
                data = data.Where(x => x.CustomerCode.Contains(filterCustomerCode));
            }

            if (!string.IsNullOrWhiteSpace(filterCustomerName))
            {
                data = data.Where(x => x.CustomerName.Contains(filterCustomerName));

            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult CustomerCategories()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("CSCT") == true
                        //&&
                        //x.SeqNo < 3
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            CategoryName = x.LookUpValueName.ToUpper(),
                            CategoryCode = x.LookUpValue
                        }).AsQueryable();

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult ZipCodes()
        {
            var data = (from x in ctx.GnMstZipCode
                        where
                        x.CompanyCode == CompanyCode
                        orderby x.ZipCode ascending
                        select new
                        {
                            ProvinceCode = x.IbuKota,
                            CityCode = x.KotaKabupaten,
                            AreaCode = x.KecamatanDistrik,
                            PosCode = x.ZipCode,
                            PosName = x.KelurahanDesa
                        }).AsQueryable();

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult ProfitCenters()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("PFCN") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            ProfitCenterName = x.LookUpValueName.ToUpper(),
                            ProfitCenterCode = x.LookUpValue
                        }).AsQueryable();

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult CustomerClasses()
        {
            string profitCenterCode = Request["ProfitCenterCode"] ?? "";
            var data = (from x in ctx.GnMstCustomerClasses
                        where
                        x.ProfitCenterCode == profitCenterCode
                        select new
                        {
                            x.CustomerClass,
                            x.CustomerClassName
                        }
                       ).Distinct().AsQueryable();

            string customerClass = Request["filterCustomerClass"] ?? "";
            string customerClassName = Request["filterCustomerClassName"] ?? "";

            if (!string.IsNullOrWhiteSpace(customerClass))
            {
                data = data.Where(x => x.CustomerClass.Contains(customerClass));
            }
            if (!string.IsNullOrWhiteSpace(customerClassName))
            {
                data = data.Where(x => x.CustomerClassName.Contains(customerClassName));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult Taxes()
        {
            var data = (from x in ctx.Taxs
                        where
                        x.CompanyCode == CompanyCode
                        select new { 
                            TaxCode = x.TaxCode,
                            TaxDesc = x.Description
                        }
                       ).AsQueryable();

            string taxCode = Request["filterTaxCode"] ?? "";
            string taskDesc = Request["filterTaskDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(taxCode))
            {
                data = data.Where(x => x.TaxCode.Contains(taxCode));
            }
            if (!string.IsNullOrWhiteSpace(taskDesc))
            {
                data = data.Where(x => x.TaxDesc.Contains(taskDesc));
            } 

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult Collectors()
        {
            string profitCenterCode = Request["ProfitCenterCode"] ?? "";
            var data = (from x in ctx.GnMstCollectors
                        where
                        x.CompanyCode == CompanyCode
                        &&
                        x.BranchCode == BranchCode
                        &&
                        x.ProfitCenterCode == profitCenterCode
                        select new
                        {
                            CollectorCode = x.CollectorCode,
                            CollectorName = x.CollectorName
                        }
                       ).AsQueryable();

            string collectorCode = Request["filterCollectorCode"] ?? "";
            string collectorName = Request["filterCollectorName"] ?? "";

            if (!string.IsNullOrWhiteSpace(collectorCode))
            {
                data = data.Where(x => x.CollectorCode.Contains(collectorCode));
            }
            if (!string.IsNullOrWhiteSpace(collectorName))
            {
                data = data.Where(x => x.CollectorName.Contains(collectorName));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult KodeTransaksiPajak()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("TRPJ") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            TaxTransDesc = x.LookUpValueName.ToUpper(),
                            TaxTransCode = x.LookUpValue
                        }).AsQueryable();

            string taxTransCode = Request["filterTaxTransCode"] ?? "";
            string taxTransDesc = Request["filterTaxTransDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(taxTransCode))
            {
                data = data.Where(x => x.TaxTransCode.Contains(taxTransCode));
            }
            if (!string.IsNullOrWhiteSpace(taxTransDesc))
            {
                data = data.Where(x => x.TaxTransDesc.Contains(taxTransDesc));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult Salesmans()
        {
            string department = Request["Department"] ?? "";
            string position = Request["Position"] ?? "";
            string employeeID = Request["filterSalesmanCode"] ?? "";
            string employeeName = Request["filterSalesmanName"] ?? "";

            var data = (from x in ctx.HrEmployeeViews
                        where
                        x.CompanyCode == CompanyCode
                        select x).AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
            {
                data = data.Where(x => x.Department.Contains(department));
            }
            if (!string.IsNullOrWhiteSpace(position))
            {
                data = data.Where(x => x.Position.Contains(position) || x.Position.Contains("CT"));
            }                                                                                
            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                data = data.Where(x => x.EmployeeName.Contains(employeeName));
            }
            if (!string.IsNullOrWhiteSpace(employeeID))
            {
                data = data.Where(x => x.EmployeeID.Contains(employeeID));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult KelompokAR()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("GPAR") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            KelARDesc = x.LookUpValueName.ToUpper(),
                            KelAR = x.LookUpValue
                        }).AsQueryable();

            string kelAR = Request["filterKelAR"] ?? "";
            string kelARDesc = Request["filterKelARDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(kelAR))
            {
                data = data.Where(x => x.KelAR.Contains(kelAR));
            }
            if (!string.IsNullOrWhiteSpace(kelARDesc))
            {
                data = data.Where(x => x.KelARDesc.Contains(kelARDesc));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult CustomerGrades()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("CSGR") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            CustomerGradeName = x.LookUpValueName.ToUpper(),
                            CustomerGrade = x.LookUpValue
                        }).AsQueryable();

            string customerGrade = Request["filterCustomerGradeCode"] ?? "";
            string customerGradeName = Request["filterCustomerGradeName"] ?? "";

            if (!string.IsNullOrWhiteSpace(customerGrade))
            {
                data = data.Where(x => x.CustomerGrade.Contains(customerGrade));
            }
            if (!string.IsNullOrWhiteSpace(customerGradeName))
            {
                data = data.Where(x => x.CustomerGradeName.Contains(customerGradeName));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult GroupPrices()
        {
            var data = (from x in ctx.OmMstRefferences
                        where
                        x.RefferenceType.Equals("GRPR") == true
                        orderby x.RefferenceDesc1 ascending
                        select new
                        {
                            GroupPriceDesc = x.RefferenceDesc1.ToUpper(),
                            GroupPrice = x.RefferenceCode
                        }).AsQueryable();

            string groupPrice = Request["filterGroupPrice"] ?? "";
            string groupPriceDesc = Request["filterGroupPriceDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(groupPrice))
            {
                data = data.Where(x => x.GroupPrice.Contains(groupPrice));
            }
            if (!string.IsNullOrWhiteSpace(groupPriceDesc))
            {
                data = data.Where(x => x.GroupPriceDesc.Contains(groupPriceDesc));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult TypeOfGoods()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("TPGO") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            TypeOfGoodsName = x.LookUpValueName.ToUpper(),
                            TypeOfGoods = x.LookUpValue
                        }).AsQueryable();

            string groupPrice = Request["filterGroupPrice"] ?? "";
            string groupPriceDesc = Request["filterGroupPriceDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(groupPrice))
            {
                data = data.Where(x => x.TypeOfGoods.Contains(groupPrice));
            }
            if (!string.IsNullOrWhiteSpace(groupPriceDesc))
            {
                data = data.Where(x => x.TypeOfGoodsName.Contains(groupPriceDesc));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult Banks()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("BANK") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            BankName = x.LookUpValueName.ToUpper(),
                            BankCode = x.LookUpValue
                        }).AsQueryable();

            string bankCode = Request["filterBankCode"] ?? "";
            string bankName = Request["filterBankName"] ?? "";

            if (!string.IsNullOrWhiteSpace(bankCode))
            {
                data = data.Where(x => x.BankCode.Contains(bankCode));
            }
            if (!string.IsNullOrWhiteSpace(bankName))
            {
                data = data.Where(x => x.BankName.Contains(bankName));
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult Branchs()
        {
            ResultModel result = InitializeResultModel();
            string filterCustomerCode = Request["filterCustomerCode"] ?? "";
            string filterCustomerName = Request["filterCustomerName"] ?? "";

            var data = ctx.Database.SqlQuery<LookupCustomer>("exec uspfn_LookupCoProfile @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterCustomerCode))
            {
                data = data.Where(x => x.CustomerCode.Contains(filterCustomerCode));
            }

            if (!string.IsNullOrWhiteSpace(filterCustomerName))
            {
                data = data.Where(x => x.CustomerName.Contains(filterCustomerName));

            }

            return Json(data.KGrid());
        }

        public JsonResult getLookupName(string CodeID, string Lookupvalue) 
        {
            var titleName = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == CodeID && a.LookUpValue == Lookupvalue).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.LookUpValueName
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getAccountName(string AccountNo)
        {
            var titleName = ctx.GnMstAccounts.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == AccountNo);
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.Description
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult getCompanyCode(string branchCode) 
        {
            //var titleName = "";
            var titleName = ctx.CoProfiles.FirstOrDefault();
            if (branchCode !=""){
                titleName = ctx.CoProfiles.Where(a => a.BranchCode == branchCode).FirstOrDefault();
            }
             
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.CompanyCode,
                    CompanyName = titleName.CompanyName,
                    CompanyGovName = titleName.CompanyGovName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = "",
                    CompanyName="",
                    CompanyGovName =""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult TaxName(string TaxCode)
        {
            var titleName = ctx.Taxs.Where(a => a.CompanyCode == CompanyCode && a.TaxCode == TaxCode).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.Description
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SupplierName(string SupplierClass)
        {
            var titleName = ctx.GnMstSuppliers.Where(a => a.SupplierCode == SupplierClass).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.SupplierName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CustomerName(string CustomerCode) 
        {
            var titleName = ctx.GnMstCustomers.Where(a => a.CustomerCode == CustomerCode).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName.CustomerName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult documenttype(string DocumentType)
        {
            var titleName = ctx.GnMstDocuments.Where(a => a.DocumentType == DocumentType).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult getuser(string UserId)
        {
            var titleName = ctx.SysUsers.Where(a => a.UserId == UserId).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    TitleName = titleName,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}


