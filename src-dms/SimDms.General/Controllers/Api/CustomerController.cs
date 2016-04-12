using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace SimDms.General.Controllers.Api
{
    public class CustomerController : BaseController
    {

        public JsonResult Default(string custCode)
        {

            return Json(custCode);
        }

       

        public JsonResult ZipCodes(CustomerModel model)
        {
            var record = ctx.GnMstZipCode.Where(p => p.CompanyCode == CompanyCode && p.ZipCode ==  model.ZipNo).FirstOrDefault();
          
            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult CategoryCode(CustomerModel model)
        {
            var record = ctx.LookUpDtls.Find(CompanyCode, "CSCT", model.CategoryCode);
            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult DefaultPKP() 
        {
            var me = ctx.LookUpDtls.Where(p => p.CodeID == "FPJSTD").OrderBy(p => p.SeqNo);
            return Json(me);
        }

        public JsonResult AccessCustomer()
        {
            var RoleId = ctx.SysRoleUsers.Where(p => p.UserId == User.Identity.Name).FirstOrDefault().RoleId;
            var me = ctx.SysRoleMenus.Where(p => p.RoleId == RoleId && p.MenuId == "mnuGnMSCustomerCR").OrderBy(p => p.MenuId).FirstOrDefault();
            if (me != null)
            {
                return Json(new { success = true, data = me }); 
            }
            else {
                return Json(new { success = false}); 
            }
        }

        public JsonResult Inquiry(bool AllowPeriod, DateTime StartDate, DateTime EndDate, string Branch)
        {

            try
            {

                string dtFirstDate, dtLastDate;
                string flag1 = AllowPeriod ? "1" : "0";

                dtFirstDate = StartDate.ToString("yyyy-MM-dd");
                dtLastDate = EndDate.ToString("yyyy-MM-dd");

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "usprpt_QueryCustomerDealer2 '" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','','','" + Branch + "'";

                MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var records = dt.Tables[0];
                var rows = records.Rows.Count;

                return Json(new { success = true, data = records, total = rows }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            string s = "";
            var f = ctx.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;
            var u = ctx.SysUsers.Find(User.Identity.Name);
            var g = ctx.CoProfiles.Find(u.CompanyCode, u.BranchCode);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                CompanyGovName = g.CompanyGovName,
                BranchCode = u.BranchCode,
                CompanyName = g.CompanyName,
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                ProfitCenter = s,
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,

            };

            return info;
        }

        public JsonResult Get()
        {
            ResultModel result = InitializeResultModel();
            string customerCode = Request["CustomerCode"] ?? "";
            var data = ctx.GnMstCustomers.Where(x => x.CompanyCode == CompanyCode && x.CustomerCode == customerCode).FirstOrDefault();

            if (data != null)
            {
                result.status = true;
                data.ProvinceCode = ctx.GnMstZipCode.Where(x => x.CompanyCode == CompanyCode && x.ZipCode == data.ZipNo).FirstOrDefault().IbuKota;
                result.data = data;
            }
            else
            {
                result.message = "Tidak bisa mengambil data Pelanggan.";
            }

            return Json(result);
        }

        public JsonResult CustBrowse(int cols)
        {
            var field = "";
            var value = "";

            string dynamicFilter = "";

            for (int i = 0; i < cols; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? "' AND " + field + " LIKE ''%" + value + "%''" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec uspfn_getCustomerBrowse {0}", dynamicFilter);
            var queryable = ctx.Database.SqlQuery<Customerbrowse>(query).AsQueryable();
            return Json(queryable.toKG());
        }

        [HttpPost]
        public JsonResult GetCustomerCategory()
        {
            string categoryCode = Request["CategoryCode"] ?? "";
            var data = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "CSCT");
            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                data = data.Where(x => x.LookUpValue == categoryCode);
            }

            var rec = data.FirstOrDefault();

            if (rec != null)
            {
                return Json(new
                {
                    CategoryName = rec.LookUpValueName
                });
            }

            return Json(new { CategoryName = "" });
        }

        [HttpPost]
        public JsonResult GetPostalCode()
        {
            string zipCode = Request["ZipNo"] ?? "";
            var data = ctx.GnMstZipCode.Where(x => x.ZipCode == zipCode).FirstOrDefault();

            if (data != null)
            {
                return Json(new
                {
                    ZipNo = data.ZipCode,
                    PosName = data.KelurahanDesa,
                    ProvinceCode = data.IbuKota,
                    CityCode = data.KotaKabupaten,
                    AreaCode = data.KecamatanDistrik
                });
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult SetCustomerCode()
        {
            ResultModel result = InitializeResultModel();
            bool constrainGenerate = false;

            var dtHQ = (from p in ctx.GnMstOrganizationDtls
                        where p.CompanyCode == CompanyCode && p.IsBranch == false
                        select new
                        {
                            CompanyCode = p.CompanyCode,
                            BranchCode = p.BranchCode
                        }).ToList();
            if (dtHQ.Count > 1)
            {
                result.message = "Periksa setting-an organisasi, jumlah holding lebih dari satu";
                return Json(result);
            }

            var recordUtl = ctx.GnMstCustomerUtilities.Find(CompanyCode, dtHQ[0].BranchCode);
            if (recordUtl != null)
            {
                if (recordUtl.IsAutoGenerate.Value)
                {
                    constrainGenerate = true;
                }
                else
                {
                    constrainGenerate = false;
                }
            }

            result.data = constrainGenerate;
            return Json(result);
        }

        [HttpPost]
        public JsonResult Save(CustomerModel model)
        {
            //var KodeProvinsi = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "PROV" && a.LookUpValueName == model.IbuKota).FirstOrDefault();
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;
            Helpers.ReplaceNullable(model);
            if (model.isPKP == true)
            {
                if (string.IsNullOrWhiteSpace(model.NPWPNo) == true)
                {
                    result.message = "Jika Faktur Pajak Standard diaktifkan, maka No. NPWP harus diisi.";
                    return Json(result);
                }

                if (string.IsNullOrWhiteSpace(model.SKPNo))
                {
                    result.message = "Jika Faktur Pajak Standar diaktifkan, maka nomor SKP harus diisi.";
                    return Json(result);
                }

                if (string.IsNullOrWhiteSpace(model.CustomerGovName))
                {
                    result.message = "Anda mengaktifkan Faktur Pajak Standar, maka nama pajak harus diisi.";
                    return Json(result);
                }
            }

            if (model.isPKP == false)
            {
                if (!string.IsNullOrWhiteSpace(model.NPWPNo))
                {
                    result.message = "Anda menonaktifkan Faktur Pajax Standar dengan nomor NPWP terisi.";
                    return Json(result);
                }

                if (!string.IsNullOrWhiteSpace(model.SKPNo))
                {
                    result.message = "Anda menonaktifkan Faktur Pajax Standar dengan nomor SKP terisi.";
                    return Json(result);
                }
            }

            if (model.CustomerType == "C" && string.IsNullOrWhiteSpace(model.NPWPNo) == true)
            {
                result.message = "Jika Tipe Customer adalah Perusahaan, maka No. NPWP harus diisi.";
                return Json(result);
            }


            var customer = ctx.GnMstCustomers.Find(companyCode, model.CustomerCode);
            var oGnMstCustomerDealer = ctx.GnMstCustomerDealers.Find(CompanyCode, model.CustomerCode);
            
            string newCustomerCode = "";
            if (customer == null)
            {                
                var dtHQ = (from p in ctx.GnMstOrganizationDtls
                            where p.CompanyCode == companyCode && p.IsBranch == false
                            select new
                            {
                                CompanyCode = p.CompanyCode,
                                BranchCode = p.BranchCode
                            }).ToList();
                if (dtHQ.Count > 1)
                {
                    result.message = "Periksa setting-an organisasi, jumlah holding lebih dari satu";
                    return Json(result);
                }

                //Get New Customer Code
                //var customerUtil = ctx.GnMstCustomerUtilities.Find(companyCode, dtHQ[0].BranchCode);
                string branchcode = dtHQ[0].BranchCode;
                var customerUtil = ctx.GnMstCustomerUtilities.Where(a => a.CompanyCode == companyCode && a.BranchCode == branchcode && a.IsAutoGenerate == true).FirstOrDefault();
                if (customerUtil != null)
                {
                    newCustomerCode = (customerUtil.Sequence + 1).ToString();
                    newCustomerCode = newCustomerCode.PadLeft(7, '0');

                    customerUtil.Sequence = customerUtil.Sequence + 1;

                    customer = ctx.GnMstCustomers.Find(companyCode, newCustomerCode);
                    if (customer != null)
                    {
                        result.message = "Harap periksa settingan no sequence utility customer";
                        return Json(result);
                    }

                    customer = new GnMstCustomer();
                    customer.CompanyCode = companyCode;
                    customer.CustomerCode = newCustomerCode;
                    customer.StandardCode = newCustomerCode;

                    customer.CreatedBy = userID;
                    customer.CreatedDate = currentTime;

                    ctx.GnMstCustomers.Add(customer);

                    if (oGnMstCustomerDealer == null)
                    {
                        oGnMstCustomerDealer = new GnMstCustomerDealer();
                        oGnMstCustomerDealer.CustomerCode = customer.CustomerCode;
                        if (CurrentUserInfo().ProductType == "4W")
                            oGnMstCustomerDealer.SuzukiCode = customer.StandardCode;
                        else
                            oGnMstCustomerDealer.Suzuki2Code = customer.StandardCode;

                        oGnMstCustomerDealer.CompanyCode = CompanyCode;
                        oGnMstCustomerDealer.CreatedBy = userID;
                        oGnMstCustomerDealer.CreatedDate = currentTime;
                        ctx.GnMstCustomerDealers.Add(oGnMstCustomerDealer);
                    }
                }
                else
                {
                    customer = new GnMstCustomer();
                    customer.CompanyCode = companyCode;
                    customer.CustomerCode = model.CustomerCode;
                    customer.StandardCode = model.StandardCode;
                    customer.CreatedBy = userID;
                    customer.CreatedDate = currentTime;

                    ctx.GnMstCustomers.Add(customer);
                    if (oGnMstCustomerDealer == null)
                    {
                        oGnMstCustomerDealer = new GnMstCustomerDealer();
                        oGnMstCustomerDealer.CustomerCode = model.CustomerCode;
                        if (CurrentUserInfo().ProductType == "4W")
                            oGnMstCustomerDealer.SuzukiCode = model.StandardCode;
                        else
                            oGnMstCustomerDealer.Suzuki2Code = model.StandardCode;

                        oGnMstCustomerDealer.CompanyCode = CompanyCode;
                        oGnMstCustomerDealer.CreatedBy = userID;
                        oGnMstCustomerDealer.CreatedDate = DateTime.Now;
                        ctx.GnMstCustomerDealers.Add(oGnMstCustomerDealer);
                    }
                }
            }
            else
            {
                customer.StandardCode = model.StandardCode;
            }
            customer.CityCode = model.CityCode;
            customer.AreaCode = model.AreaCode;
            customer.Address1 = model.Address1;
            customer.Address2 = model.Address2;
            customer.Address3 = model.Address3;
            customer.Address4 = model.Address4;
            customer.BirthDate = model.BirthDate;
            customer.CategoryCode = model.CategoryCode;
            customer.CustomerAbbrName = model.CustomerAbbrName;
            customer.CustomerGovName = model.CustomerGovName;
            customer.CustomerName = model.CustomerName;
            customer.CustomerStatus = model.CustomerStatus;
            customer.CustomerType = model.CustomerType;
            customer.Email = model.Email;
            customer.FaxNo = model.FaxNo;
            customer.Gender = model.Gender;
            customer.HPNo = model.HPNo;
            customer.IbuKota = model.ProvinceCode;
            customer.isLocked = model.isLocked;
            customer.isPKP = model.isPKP;
            customer.KecamatanDistrik = model.KecamatanDistrik;
            customer.KelurahanDesa = model.KelurahanDesa;
            customer.KotaKabupaten = model.KotaKabupaten;
            customer.IbuKota = model.IbuKota;
            customer.LastUpdateBy = userID;
            customer.LastUpdateDate = currentTime;
            //customer.LockingBy = null;
            //customer.LockingDate = null;
            customer.NPWPDate = model.NPWPDate;
            customer.NPWPNo = model.NPWPNo;
            customer.OfficePhoneNo = model.OfficePhoneNo;
            customer.PhoneNo = model.PhoneNo;
            //customer.ProvinceCode = model.ProvinceCode;
            customer.SKPDate = model.SKPDate;
            customer.SKPNo = model.SKPNo;
            customer.Spare01 = model.Spare01;
            customer.Spare02 = model.Spare02;
            customer.Spare03 = model.Spare03;
            customer.Spare04 = model.Spare04;
            customer.Spare05 = model.Spare05;
            //customer.StandardCode = customer.CustomerCode;
            customer.StandardCode = customer.StandardCode;
            customer.Status = model.Status;
            customer.ZipNo = model.ZipNo;
            customer.CustomerAbbrName = model.CustomerAbbrName;

            
            oGnMstCustomerDealer.CustomerName = model.CustomerName;
            oGnMstCustomerDealer.CustomerGovName = model.CustomerGovName;
            oGnMstCustomerDealer.Address1 = model.Address1;
            oGnMstCustomerDealer.Address2 = model.Address2;
            oGnMstCustomerDealer.Address3 = model.Address3;
            oGnMstCustomerDealer.Address4 = model.Address4;
            //oGnMstCustomerDealer.ProvinceCode = KodeProvinsi == null ? "" : KodeProvinsi.LookUpValue;
            oGnMstCustomerDealer.CityCode = model.CityCode;
            oGnMstCustomerDealer.ZipNo = model.ZipNo;
            oGnMstCustomerDealer.KelurahanDesa = model.KelurahanDesa;
            oGnMstCustomerDealer.KecamatanDistrik = model.KecamatanDistrik;
            oGnMstCustomerDealer.KotaKabupaten = model.KotaKabupaten;
            oGnMstCustomerDealer.IbuKota = model.IbuKota;
            oGnMstCustomerDealer.PhoneNo = model.PhoneNo;
            oGnMstCustomerDealer.HPNo = model.HPNo;
            oGnMstCustomerDealer.FaxNo = model.FaxNo;
            oGnMstCustomerDealer.OfficePhoneNo = model.OfficePhoneNo;
            oGnMstCustomerDealer.Email = model.Email;
            oGnMstCustomerDealer.BirthDate = model.BirthDate;
            oGnMstCustomerDealer.isPKP = model.isPKP;
            oGnMstCustomerDealer.NPWPNo = model.NPWPNo;
            oGnMstCustomerDealer.NPWPDate = model.NPWPDate;
            oGnMstCustomerDealer.SKPNo = model.SKPNo;
            oGnMstCustomerDealer.SKPDate = model.SKPDate;
            oGnMstCustomerDealer.CustomerType = model.CustomerType;
            oGnMstCustomerDealer.CategoryCode = model.CategoryCode;
            oGnMstCustomerDealer.LastUpdateDate = DateTime.Now;
            oGnMstCustomerDealer.LastUpdateBy = userID;

            try
            {
                Helpers.ReplaceNullable(customer);
                Helpers.ReplaceNullable(oGnMstCustomerDealer);
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data customer berhasil disimpan.";
                result.data = new
                {
                    CustomerCode = customer.CustomerCode,
                    StandardCode = customer.StandardCode
                };
            }
            catch (Exception Ex) {
                result.message = "Data customer tidak bisa disimpan.";
                MyLogger.Info("Error on customer saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(CustomerModel model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
                    if (me != null)
                    {
                        ctx.GnMstCustomers.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Customer GL berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Customer , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Customer , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
        [HttpGet]
        public IQueryable<GnMstCustomerDealerProfitCenter> _GnMstCustomerDealerProfitCenters(string CustomerCode)
        {

            var queryable = ctx.Database.SqlQuery<GnMstCustomerDealerProfitCenter>("select * from GnMstCustomerDealerProfitCenter where CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and CustomerCode='" + CustomerCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GnMstCustomerProfitCenterView> _GnMstCustomerProfitCenters(string CustomerCode,string CustomerGovName)
        {

            var queryable = ctx.Database.SqlQuery<GnMstCustomerProfitCenterView>("select  a.*,b.LookUpValueName ProfitCenterName,'"+ CustomerGovName + "' as CustomerGovName from GnMstCustomerProfitCenter a inner join gnMstLookUpDtl b on a.ProfitCenterCode=b.LookUpValue where a.CompanyCode='" + CompanyCode + "' and a.BranchCode='" + BranchCode  + "' and a.CustomerCode='" + CustomerCode + "' and  b.CodeID='PFCN'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public ProfitCenter GnMstCustomerProfitCentersbyCode(string CustomerCode, string ProfitCenterCode)
        {

            var queryable = ctx.Database.SqlQuery<ProfitCenter>("select * from GnMstCustomerProfitCenter where CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and CustomerCode='" + CustomerCode + "' and ProfitCenterCode='"+ ProfitCenterCode + "'").FirstOrDefault();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<GnMstCustomerDiscView> _GnMstCustomerDiscs(string CustomerCode)
        {

            var queryable = ctx.Database.SqlQuery<GnMstCustomerDiscView>("select a.*, b.LookUpValueName ProfitCenterName, c.LookUpValueName TypeOfGoodsName from GnMstCustomerDisc a inner join gnMstLookUpDtl b  on a.ProfitCenterCode=b.LookUpValue inner join gnMstLookUpDtl c on a.TypeOfGoods=c.LookUpValue where a.CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode + "' and CustomerCode='" + CustomerCode + "' and  b.CodeID='PFCN' and  c.CodeID='TPGO'").AsQueryable();
            return (queryable);

        }

  

        [HttpGet]
        public IQueryable<GnMstCustomerBank> _GnMstCustomerBanks(string CustomerCode)
        {
      
            var queryable = ctx.Database.SqlQuery<GnMstCustomerBank>("select * from GnMstCustomerBank where CompanyCode='" + CompanyCode + "' and CustomerCode='"+ CustomerCode +"'").AsQueryable();
            return (queryable);

        }


        public JsonResult CheckCustomer(string CustomerCode)
        {
            //var record = ctx.GnMstCustomers.Find(CompanyCode, CustomerCode);
            var query = string.Format(@"
                           select a.*,c.LookUpValueName as CategoryName, b.KelurahanDesa as PosName,
                            a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as AddressGab
                            from gnMstCustomer a
                            left join gnMstLookUpDtl c
                            on a.CompanyCode = c.CompanyCode and a.CategoryCode = c.LookUpValue
                            left join gnMstZipCode b
                            on a.zipno =b.zipcode and a.KelurahanDesa = b.KelurahanDesa 
	                            and a.KecamatanDistrik =b.KecamatanDistrik and a.KotaKabupaten = b.KotaKabupaten 
	                            and a.IbuKota =b.IbuKota
                            where c.CodeID ='CSCT' and a.CustomerCode='" + CustomerCode + "'");
            var record = ctx.Database.SqlQuery<Customerbrowse>(query).First(); 
            //var categoryName = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == "CSCT" && a.ParaValue == record.CategoryCode).LookUpValueName;
           // var posName = ctx.GnMstZipCode.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.ZipCode == record.ZipNo).KelurahanDesa; 
            if (record != null)
            {
                return Json(new
                {
                    success = true,
                    data = record,
                    CustomerProfitCenters = _GnMstCustomerProfitCenters(CustomerCode, record.CustomerGovName),
                    CustomerDiscs = _GnMstCustomerDiscs(CustomerCode),
                    CustomerBanks = _GnMstCustomerBanks(CustomerCode)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public Object ProfitCenterInfo(string CustomerCode, string ProfitCenter)
        {
            if (!string.IsNullOrEmpty(CustomerCode) && !string.IsNullOrEmpty(ProfitCenter))
            {
                string s = "";
                var profitCenter = Request["ProfitCenter"];
                var customerCode = Request["CustomerCode"];
                var u = CurrentUserInfo();

                var pc = GnMstCustomerProfitCentersbyCode(customerCode, profitCenter);
                var omrf=ctx.OmMstRefferences.Where(p => p.RefferenceType == "GRPR" && p.RefferenceCode == pc.GroupPriceCode).FirstOrDefault();

                return Json( new 
                {
                    ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, pc.ProfitCenterCode),
                    CustomerClassName = CustomerClassName(pc.ProfitCenterCode, pc.CustomerClass),
                    TaxDesc = TaxesName(pc.TaxCode),
                    CollectorName = CollectorsName(pc.ProfitCenterCode, pc.CollectorCode),
                    TaxTransDesc = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TransKdPjk, pc.TaxTransCode),
                    SalesmanName = SalesmansName(pc.Salesman),
                    KelAR= pc.SalesCode,
                    KelARDesc = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.GroupAR, pc.SalesCode),
                    CustomerGradeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.CustomerGrade, pc.CustomerGrade),
                    GroupPriceDesc = omrf == null ? "" : omrf.RefferenceDesc1,
                    GroupPriceCode=pc.GroupPriceCode
                });

                //return Json(new  info;
            }

            return null;
        }

        [HttpPost]
        public string CustomerClassName(string profitCenterCode, string CustomerClass)
        {
           if (!string.IsNullOrEmpty(profitCenterCode) && !string.IsNullOrEmpty(CustomerClass))
            {   
            var Uid = CurrentUserInfo();

            var queryable = ctx.Database.SqlQuery<LookupNameview>("select  CustomerClassName  as LookupName from GnMstCustomerClass where CompanyCode ='" + Uid.CompanyCode + "' and BranchCode='" + Uid.BranchCode + "' and ProfitCenterCode='" + profitCenterCode + "' and CustomerClass='" + CustomerClass + "'").FirstOrDefault();
            return (queryable.LookupName);
            }
           return "";
        }


        [HttpPost]
        public string TaxesName(string TaxCode)
        {
            if (!string.IsNullOrEmpty(TaxCode))
            {
            var Uid = CurrentUserInfo();
            var queryable = ctx.Database.SqlQuery<LookupNameview>("select  Description as LookupName   from gnMstTax  where CompanyCode ='" + Uid.CompanyCode + "' and TaxCode='" + TaxCode + "'").FirstOrDefault();
            return (queryable.LookupName);
            }
            return "";
        }

        [HttpPost]
        public string CollectorsName(string profitCenterCode, string CollectorCode)
        {
            if (!string.IsNullOrEmpty(profitCenterCode) && !string.IsNullOrEmpty(CollectorCode))
            {
                var Uid = CurrentUserInfo();
                var queryable = ctx.Database.SqlQuery<LookupNameview>("select  CollectorName  as LookupName  from gnMstCollector  where CompanyCode ='" + Uid.CompanyCode + "' and ProfitCenterCode='" + profitCenterCode + "' and CollectorCode='" + CollectorCode + "'").FirstOrDefault();
                return (queryable.LookupName);
            }
            return "";
        }

        [HttpPost]
        public string SalesmansName(string EmployeeID)
        {
            if (!string.IsNullOrEmpty(EmployeeID))
            {
                var Uid = CurrentUserInfo();
                var queryable = ctx.Database.SqlQuery<LookupNameview>("select  EmployeeName as LookupName from gnMstEmployee where CompanyCode='" + Uid.CompanyCode + "' and EmployeeID='" + EmployeeID + "'").FirstOrDefault();
                return (queryable.LookupName);
            }
            return "";

        }


        public class LookupNameview
        {
            public string LookupName { get; set; }

        }

    }
}
