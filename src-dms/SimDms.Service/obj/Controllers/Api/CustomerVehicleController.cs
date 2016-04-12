using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Service.Controllers.Api
{
    public class CustomerVehicleController : BaseController
    {
        public JsonResult BasicModel(ModelCodeOpen model)
        {
            var query = string.Format(@"Select a.RefferenceCode AS BasicModel, a.DescriptionEng AS TechnicalModelCode,
            a.Description AS ModelDescription, CASE a.IsActive WHEN '1' THEN 'Aktif' ELSE 'Tidak Aktif' END AS Status from svMstrefferenceService a
            WHERE a.CompanyCode = '{0}'AND a.ProductType = '{1}' AND a.RefferenceType = 'BASMODEL'AND a.RefferenceCode = '{2}'
            ", CompanyCode, ProductType, model.BasicModel);

            var sqlstr = ctx.Database.SqlQuery<ModelCodeOpen>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult ColourCode(ColourCodeOpen model)
        {
            var query = string.Format(@"select RefferenceCode, RefferenceDesc1, RefferenceDesc2 
                from omMstRefference
                where CompanyCode = '{0}' and RefferenceType = 'COLO' and RefferenceCode = '{1}'
                order by RefferenceDesc1
            ", CompanyCode, model.ColourCode);

            var sqlstr = ctx.Database.SqlQuery<ColourCodeOpen>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult CustomerCode(CustomerCodeOpen model)
        {
            var query = string.Format(@"SELECT CustomerCode, CustomerName, Address1, Address2, Address3, Address4, 
                Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 AS Address,
                CASE Status WHEN 1 THEN 'Aktif' ELSE 'TIdak Aktif' END AS Status
                  FROM gnMstCustomer with(nolock, nowait)
                WHERE  CompanyCode = '{0}' and CustomerCode = '{1}'
                ORDER BY CustomerName ASC
            ", CompanyCode, model.CustomerCode);

            var sqlstr = ctx.Database.SqlQuery<CustomerCodeOpen>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult DealerCode(CustomerCodeOpen model)
        {
            var query = string.Format(@"SELECT CustomerCode, CustomerName, Address1, Address2, Address3, Address4, 
                Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 AS Address,
                CASE Status WHEN 1 THEN 'Aktif' ELSE 'TIdak Aktif' END AS Status
                  FROM gnMstCustomer with(nolock, nowait)
                WHERE  CompanyCode = '{0}' and CustomerCode = '{1}'
                ORDER BY CustomerName ASC
            ", CompanyCode, model.DealerCode);

            var sqlstr = ctx.Database.SqlQuery<CustomerCodeOpen>(query).AsQueryable();
            return Json(sqlstr);
        }

        
        public JsonResult SavePacth( string isSPK, List<GetBookNoNewAndOld> model)
        {
            string msg = "Data Tidak Berhasil Di Simpan";
            try
            {
                string ChassisCode = ""; decimal? ChassisNo = 0; string newServiceBookNo = "";
                {
                    if (model.Count() > 0)
                    {
                        foreach (GetBookNoNewAndOld recDtl in model)
                        {
                            ChassisCode = recDtl.ChassisCode;
                            ChassisNo = recDtl.ChassisNo;
                            newServiceBookNo = recDtl.ServiceBookNoNew;
                        }

                        ctx.Database.ExecuteSqlCommand("uspfn_SvSetServiceBookNo'" + CompanyCode + "','" + BranchCode + "','" + ChassisCode + "','" + ChassisNo + "','" + newServiceBookNo + "','" + isSPK + "'");
                        msg = "Data Berhasil Di Simpan";
                    }

                    return Json(new { success = true, message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = msg, error_log = ex.Message });
            }
        }

        public JsonResult GetBookNoNewAndOld(string ChassisCode, string ChassisNo, string isSPK)
        {
            var query = string.Format(@"uspfn_SvGetServiceBookNo '{0}','{1}','{2}','{3}','{4}'
            ", CompanyCode, BranchCode, ChassisCode, ChassisNo, isSPK);

            var sqlstr = ctx.Database.SqlQuery<GetBookNoNewAndOld>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult CustVehicle(string ChassisCode, string ChassisNo)
        {
            var query = string.Format(@"SELECT a.*, b.RefferenceDesc1 as ColourName, c.CustomerName as DealerName, d.CustomerName as CustomerName
            FROM svMstCustomerVehicle a
            LEFT JOIN omMstRefference b
				ON a.ColourCode = b.RefferenceCode AND b.RefferenceType='COLO'
			LEFT JOIN gnMstCustomer c
				ON a.DealerCode = c.CustomerCode
			LEFT JOIN gnMstCustomer d
				ON a.CustomerCode = d.CustomerCode
            WHERE a.CompanyCode = '{0}' and a.ChassisCode = '{1}' and a.ChassisNo = '{2}'
            ", CompanyCode, ChassisCode, (ChassisNo != null) ? Decimal.Parse(ChassisNo) : 0);

            var sqlstr = ctx.Database.SqlQuery<MstCustomerVehicleView>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult HistCustomerVehicle(string ChassisCode, string ChassisNo)
        {
            var query = string.Format(@"select SeqNo, PreviousData, ChangeCode, LastUpdateBy, LastUpdateDate 
                from svMstCustomerVehicleHist a
                where 
                a.companycode = '{0}' and
                a.chassiscode = '{1}' and
                a.chassisno = '{2}'
            ", CompanyCode, ChassisCode, ChassisNo);

            var sqlstr = ctx.Database.SqlQuery<CustomerVehicleHistGrid>(query).AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult Default()
        {
            return Json(new
            {

                FakturPolisiDate = DateTime.Now,
                
            });
        }

      /*  public string getData()
        {
            var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            return GetNewDocumentNo("KTK", transdate.Value);
        }
        */
        public int GetSeqNo(string ChassisCode, long ChassisNo, string ChangeCode)
        {
            var query = string.Format(@"SELECT ISNULL((max(SeqNo)), 0) AS SeqNo
                FROM svMstCustomerVehicleHist 
                WHERE CompanyCode = '{0}' AND 
                    ChassisCode = '{1}' AND 
                    ChassisNo = '{2}'
            ", CompanyCode, ChassisCode, ChassisNo, ChangeCode);


            //var data = ctx.svMstCustomerVehicleHists.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.ChassisCode == ChassisCode && a.ChassisNo == ChassisNo && a.ChangeCode == ChangeCode).SeqNo;

            //return data;

            var sqlstr = ctx.Database.SqlQuery<int>(query).FirstOrDefault();
            return sqlstr;
        }

        public JsonResult CustomerVehicleHistoryData(string ChassisCode, long ChassisNo, string ChangeCode, string PreviousData)
        {
            var data = ctx.svMstCustomerVehicleHists.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.ChassisCode == ChassisCode && a.ChassisNo == ChassisNo && a.ChangeCode == ChangeCode && a.PreviousData == PreviousData);
            if (data == null)
            {
                var seqNo = GetSeqNo(ChassisCode, ChassisNo, ChangeCode);
                data = new svMstCustomerVehicleHist()
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = ChassisCode,
                    ChassisNo = ChassisNo,
                    ChangeCode = ChangeCode,
                    SeqNo = seqNo+1,
                    PreviousData = PreviousData == "null" ? "" : PreviousData == null ? "" : PreviousData,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now

                };

                ctx.svMstCustomerVehicleHists.Add(data);
            }
            data.PreviousData = PreviousData;
            data.LastUpdateBy = CurrentUser.UserId;
            data.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            //return Json(new { success = false });
        }

        public JsonResult Save(MstCustomerVehicleView model)
        {
            var StatusDoc = "";
            var record = ctx.CustomerVehicles.Find(CompanyCode, model.ChassisCode, model.ChassisNo);
            if (record == null)
            {
                record = new CustomerVehicle
                {
                    CompanyCode = CompanyCode,
                    ChassisCode = model.ChassisCode,
                    ChassisNo = model.ChassisNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    EngineCode = model.EngineCode,
                    EngineNo = model.EngineNo,
                    BasicModel = model.BasicModel,
                    TechnicalModelCode = model.TechnicalModelCode,
                    ServiceBookNo = model.ServiceBookNo,
                    ColourCode = model.ColourCode,
                    PoliceRegNo = model.PoliceRegNo,
                    CustomerCode = model.CustomerCode,
                    DealerCode = model.DealerCode,
                    TransmissionType = model.TransmissionType,
                    FakturPolisiDate = model.FakturPolisiDate,
                    ContractNo = "",
                    IsContractStatus = false,
                    ProductionYear = Convert.ToDecimal(model.ProductionYear),
                    IsActive = model.IsActive,
                    LastupdateBy = CurrentUser.UserId,
                    LastupdateDate = DateTime.Now,
                    ClubCode = "",
                    ClubNo = "",
                    IsClubStatus = false,
                    RemainderDescription = "",
                    LastJobType = "",
                    ContactName = model.ContactName,
                    ContactAddress = model.ContactAddress,
                    ContactPhone = model.ContactPhone,

                };
                ctx.CustomerVehicles.Add(record);
                StatusDoc = "new";
            }
            else
            {
                record.EngineCode = model.EngineCode;
                record.EngineNo = model.EngineNo;
                record.BasicModel = model.BasicModel;
                record.TechnicalModelCode = model.TechnicalModelCode;
                record.ServiceBookNo = model.ServiceBookNo;
                record.PoliceRegNo = model.PoliceRegNo;
                record.CustomerCode = model.CustomerCode;
                record.DealerCode = model.DealerCode;
                record.ColourCode = model.ColourCode;
                record.TransmissionType = model.TransmissionType;
                record.FakturPolisiDate = model.FakturPolisiDate;
                record.ContractNo = "";
                record.IsContractStatus = false;
                record.ProductionYear = Convert.ToDecimal(model.ProductionYear);
                record.IsActive = model.IsActive;
                record.LastupdateBy = CurrentUser.UserId;
                record.LastupdateDate = DateTime.Now;
                record.ClubCode = "";
                record.ClubNo = "";
                record.IsClubStatus = false;
                record.RemainderDescription = "";
                record.LastJobType = "";
                record.ContactName = model.ContactName;
                record.ContactAddress = model.ContactAddress;
                record.ContactPhone = model.ContactPhone;

                StatusDoc = "old";
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, StatusDoc = StatusDoc });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(CustomerVehicle model)
        {
            var data = ctx.Services.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProductType == ProductType && a.PoliceRegNo == model.PoliceRegNo);
            if (data == null)
            {
                var record = ctx.CustomerVehicles.Find(CompanyCode, model.ChassisCode, model.ChassisNo);
                if (record != null)
                {
                    ctx.CustomerVehicles.Remove(record);
                }

                try
                {
                    ctx.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                var msg = "Data tidak dapat dihapus karena sudah digunakan di SPK";
                return Json(new { success = false, message = msg });
            }


        }

        public JsonResult getDataTable(SvGetTableChassis model, string ChassisCode, string ChassisNo)
        {

            var record = ctx.SvGetTableChassiss.Where(x => x.CompanyCode == CompanyCode && x.ChassisCode == ChassisCode && x.ChassisNo == ChassisNo);
            return Json(record);
        }

        public JsonResult getPelangganDetail(SvCustomerDetailView model, string CustomerCode)
        {
            var record = ctx.SvCustomerDetailViews.Find(CompanyCode, CustomerCode);
            return Json(record);
        }

        public JsonResult BassmodAction(SvCBasmodView model, string BasicModel)
        {
            var record = ctx.SvCBasmodViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.RefferenceCode == BasicModel);
            return Json(record);
        }

        public JsonResult ChassisAction(MstCustomerVehicleView model) //, string ChassisCode, decimal? ChassisNo)
        {
            var record = (from e in ctx.CustomerVehicles
                         where e.CompanyCode == CompanyCode && e.ChassisCode == model.ChassisCode && e.ChassisNo == model.ChassisNo
                select new MstCustomerVehicleView()
                {
                    ChassisCode = e.ChassisCode,
                    ChassisNo = e.ChassisNo,
                    CustomerCode = e.CustomerCode,
                    CustomerName = ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == e.CustomerCode).FirstOrDefault().CustomerName,
                    DealerCode = e.DealerCode,
                    ColourCode = e.ColourCode,
                    ColourName = ctx.SvColorViews.Where(p => p.CompanyCode == CompanyCode && p.ColourCode == e.ColourCode).FirstOrDefault().RefferenceDesc1,
                    BasicModel = e.BasicModel,
                    TechnicalModelCode = e.TechnicalModelCode,
                    EngineCode = e.EngineCode,
                    EngineNo = e.EngineNo,
                    PoliceRegNo = e.PoliceRegNo,
                    ServiceBookNo = e.ServiceBookNo,
                    TransmissionType = e.TransmissionType,
                    ProductionYear = e.ProductionYear,
                    ContactName = e.ContactName == null ? ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == e.CustomerCode).FirstOrDefault().CustomerName : e.ContactName,
                    ContactAddress = e.ContactAddress == null ? ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == e.CustomerCode).FirstOrDefault().Address1 : e.ContactAddress,
                    ContactPhone = e.ContactPhone == null ? ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == e.CustomerCode).FirstOrDefault().PhoneNo : e.ContactPhone,
                    DealerName = e.DealerCode != null ? ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == e.DealerCode).FirstOrDefault().CustomerName : "",
                    FakturPolisiDate = e.FakturPolisiDate
                }).FirstOrDefault();
            return Json(new {success = true, data= record });
        }

        public JsonResult BrowseAction(SvKendaraanPel model, string ChassisCode, string ChassisNo)
        {
            var record = ctx.SvKendaraanPels.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.ChassisCode == ChassisCode && x.ChassisNo == ChassisNo);
            return Json(record);
        }

        public JsonResult ColorAction(SvColorView model, string ColourCode)
        {
            var record = ctx.SvColorViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.RefferenceCode == ColourCode);
            return Json(record);
        }

        public JsonResult LookUpCustomer()
        {
            var query = string.Format(@"SELECT a.CustomerCode, a.CustomerName, a.Address1, a.Address2, a.Address3, a.Address4, 
                a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 AS Address, 
                CASE a.Status WHEN 1 THEN 'Aktif' ELSE 'TIdak Aktif' END AS Status,
                a.HPNo AS ContactPhone, a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 AS ContactAddress, a.CustomerName ContactName
                  FROM gnMstCustomer a with(nolock, nowait)
                  INNER JOIN gnMstCustomerProfitCenter b
                    ON a.CustomerCode = b.CustomerCode
                WHERE  a.CompanyCode = '{0}' AND b.BranchCode = '{1}' AND b.ProfitCenterCode = '200'
				GROUP BY a.CustomerCode, a.CustomerName, a.Address1, a.Address2, a.Address3, a.Address4, a.Status, a.HPNo
                ORDER BY a.CustomerName ASC
            ", CompanyCode, BranchCode);

            var sqlstr = ctx.Database.SqlQuery<CustomerCodeOpen>(query).AsQueryable();

            return Json(sqlstr.toKG());
        }

        public JsonResult LoadBrowse(string BasicModel, string ColourCode, string DealerCode)
        {
            var DescriptionEng = ctx.svMstRefferenceServices.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.RefferenceType == "BASMODEL" && x.RefferenceCode == BasicModel).DescriptionEng;
            var ColourName = ctx.SvColorViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.RefferenceCode == ColourCode).RefferenceDesc1;
            var DealerName = "";
            if (DealerCode != "")
            {
                if (ctx.Customers.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CustomerCode == DealerCode).CustomerName != null)
                {
                    DealerName = ctx.Customers.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CustomerCode == DealerCode).CustomerName;
                }
            }
            return Json(new { DescriptionEng = DescriptionEng, ColourName = ColourName, DealerName = DealerName });
        }

    }

}
