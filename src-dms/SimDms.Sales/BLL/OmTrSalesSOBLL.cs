using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimDms.Common.Models;
using SimDms.Sales.Models;
using System.Transactions;
using SimDms.Common;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesSOBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrSalesSOBLL (DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public OmTRSalesSO GetRecord(string SONo) 
        {
            var record = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, SONo);
            return record;
        }
        
        public void SelectGroupAR(string customerCode, string profitCenter, out string salesCode)
        {
            var record = ctx.CustomerProfitCenters.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.CustomerCode == customerCode && p.ProfitCenterCode == profitCenter).FirstOrDefault();

            if (record != null)
            {
                salesCode = record.SalesCode;
            }
            else
            {
                salesCode = string.Empty;
            }
        }

        public List<Tax> Select4Tax(string customerCode)
        {
            var records = (from a in ctx.Taxs
                          join b in ctx.CustomerProfitCenters on new { a.CompanyCode, a.TaxCode, CustomerCode = customerCode, ProfitCenterCode = "100" }
                          equals new { b.CompanyCode, b.TaxCode, b.CustomerCode, b.ProfitCenterCode }
                          where a.CompanyCode == CompanyCode
                          select a).ToList();

            return records;
        }


       
        #endregion
        #region -- Approve --
        private decimal GetSalesAmt(string companyCode, string branchCode, string SONo)
        {
            decimal unitAmt, accAmt, spareAmt;
            unitAmt = accAmt = spareAmt = 0;
            var query = string.Format(@"
                 select sum(a.QuantitySO * a.AfterDiscTotal) as TotSO from OmTrSalesSOModel a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj = ctx.Database.SqlQuery<decimal>(query).AsQueryable();
            if (obj != null)
            {
                unitAmt = Convert.ToDecimal(obj.ToString());
            }
            var query1 = string.Format(@"
                 select isnull(sum(a.AfterDiscTotal * b.QuantitySO), 0) from OmTrSalesSOModelOthers a 
                inner join OmTrSalesSOModel b 
                    on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.SONo = b.SONo
                         and a.salesModelCode = b.salesModelCode and a.SalesModelYear = b.salesModelYear
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj1 = ctx.Database.SqlQuery<decimal>(query1).AsQueryable();

            if (obj1 != null)
            {
                accAmt = Convert.ToDecimal(obj1.ToString());
            }

            var query2 = string.Format(@"
                 select isnull(sum(a.DemandQty * a.RetailPrice), 0) from OmTrSalesSoAccs a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj2 = ctx.Database.SqlQuery<decimal>(query2).AsQueryable();

            if (obj2 != null)
            {
                spareAmt = Convert.ToDecimal(obj2.ToString());
            }
            return unitAmt + accAmt + spareAmt;
        }
        private bool UpdateBankBook(string SONo, string customerCode, string profitCenterCode, bool isReject)
        {
            bool result = false;
            bool isNew = false;
            var bankBook = ctx.GnTrnBankBooks.Find(CompanyCode, BranchCode, customerCode, profitCenterCode);
            if (bankBook == null)
            {
                isNew = true;
                bankBook = new GnTrnBankBook();
                bankBook.CompanyCode = CompanyCode;
                bankBook.BranchCode = BranchCode;
                bankBook.CustomerCode = customerCode;
                bankBook.ProfitCenterCode = profitCenterCode;
                bankBook.ReceivedAmt = 0;
            }
            if (!isReject)
                bankBook.SalesAmt = bankBook.SalesAmt + GetSalesAmt(CompanyCode, BranchCode, SONo);
            else bankBook.SalesAmt = bankBook.SalesAmt - GetSalesAmt(CompanyCode, BranchCode, SONo);

            if (isNew)
            {
                ctx.GnTrnBankBooks.Add(bankBook);
                result = true;
            }
            else
            {
                ctx.GnTrnBankBooks.Add(bankBook);
                result = true;
            }

            return result;
        }
        private void UpdateModelOwnerShip(string SONo, string salesModelCode, decimal salesModelYear, string statusVehicle, string othersBrand, string othersType)
        {
            string userID = CurrentUser.UserId;
            OmTrSalesSOModelAdditional oOmTrSalesModelAdditional = new OmTrSalesSOModelAdditional();
            OmTrSalesSOModelAdditional model = ctx.OmTrSalesSOModelAdditionals.Find(CompanyCode, BranchCode, SONo, salesModelCode, salesModelYear);
            //GetRecord(companyCode, branchCode, SONo, salesModelCode, salesModelYear);
            if (model == null)
            {
                model = new OmTrSalesSOModelAdditional();
                model.CompanyCode = CompanyCode;
                model.BranchCode = BranchCode;
                model.CreatedBy = userID;
                model.CreatedDate = DateTime.Now;
                model.LastUpdateBy = userID;
                model.LastUpdateDate = DateTime.Now;
                model.StatusVehicle = statusVehicle;
                model.OthersBrand = othersBrand;
                model.OthersType = othersType;
                model.SalesModelCode = salesModelCode;
                model.SalesModelYear = salesModelYear;
                model.SONo = SONo;
            }
            else
            {
                model.StatusVehicle = statusVehicle;
                model.OthersType = othersType;
                model.OthersBrand = othersBrand;
                model.LastUpdateDate = DateTime.Now;
                model.LastUpdateBy = userID;
            }
            ctx.OmTrSalesSOModelAdditionals.Add(model);
        }

        //public static bool ApproveSO(SysUser user, OmTRSalesSO record, bool islinkITS, List<OWSalesModel> additionalOwnership)
         //{
         //    bool result = false;
         //    string companyCode = record.CompanyCode;
         //    string branchCode = record.BranchCode;
         //    string SONo = record.SONo;
         //    string warehouseCode = record.WareHouseCode;

         //    IDbContext ctx = DbFactory.Configure(true);
         //    OmTrSalesSODao oOmTrSalesSODao = new OmTrSalesSODao(ctx);
         //    OmTrInventQtyVehicleDao oOmTrInventQtyVehicleDao = new OmTrInventQtyVehicleDao(ctx);
         //    OmMstVehicleDao oOmMstVehicleDao = new OmMstVehicleDao(ctx);
         //    List<OmTrSalesSOModelColour> listModelColour = GetListModelColour(ctx, companyCode, branchCode,
         //        SONo);
         //    DataTable dt = OmTrSalesSOVinBLL.Select("CompanyCode= '{0}' and BranchCode= '{1}' and SONo= '{2}' and ChassisNo <> 0", record.CompanyCode, record.BranchCode, record.SONo);
         //    try
         //    {
         //        foreach (DataRow row in dt.Rows)
         //        {
         //            OmMstVehicle oOmMstVehicle = oOmMstVehicleDao.GetRecord(user.CompanyCode, row["ChassisCode"].ToString(), Convert.ToDecimal(row["ChassisNo"]));
         //            if (oOmMstVehicle != null)
         //            {
         //                if (oOmMstVehicle.Status != "0")
         //                    throw new Exception("Untuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
         //                        + " tidak berstatus Ready");

         //                oOmMstVehicle.Status = "3";
         //                oOmMstVehicle.SONo = record.SONo;
         //                oOmMstVehicle.LastUpdateBy = user.UserId;
         //                oOmMstVehicle.LastUpdateDate = DmsTime.Now;

         //                if (oOmMstVehicleDao.Update(oOmMstVehicle) <= 0)
         //                    throw new Exception("Untuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
         //                        + " tidak ada terupdate di Master Kendaraan");
         //                else result = true;
         //            }
         //            else
         //                throw new Exception("Untuk kendaraan ini belum menjadi stock");
         //        }

         //        foreach (OmTrSalesSOModelColour modelColour in listModelColour)
         //        {
         //            OmTrInventQtyVehicle vehicle = GetVehicle(ctx, companyCode, branchCode, modelColour.SalesModelCode,
         //                modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
         //            if (vehicle != null)
         //            {
         //                vehicle.Alocation = vehicle.Alocation + modelColour.Quantity;
         //                vehicle.EndingOH = vehicle.BeginningOH + vehicle.QtyIn - vehicle.QtyOut;
         //                vehicle.EndingAV = vehicle.BeginningAV + vehicle.QtyIn - vehicle.Alocation - vehicle.QtyOut;
         //                //decimal i = CalculateQuantitySO(ctx, companyCode, branchCode, modelColour.SONo, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
         //                if (vehicle.EndingAV < 0)
         //                    throw new Exception("Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
         //                        + " tidak mempunyai cukup available unit");

         //                vehicle.LastUpdateBy = user.UserId;
         //                vehicle.LastUpdateDate = DmsTime.Now;

         //                if (oOmTrInventQtyVehicleDao.Update(vehicle) < 0)
         //                    throw new Exception("Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
         //                        + " tidak ada terupdate di Stock Inventory");
         //                else
         //                    result = true;
         //            }
         //        }

         //        ////Belum Digunakan
         //        //if (result && record.SalesType.Equals("1"))
         //        //{
         //        //    //update pmTrInquiryStatusHistory
         //        //    if (!UpdateStatusHistory(ctx, record, user))
         //        //        result = false;
         //        //}

         //        if (result)
         //        {
         //            if (UpdateBankBook(ctx, companyCode, branchCode, SONo, record.CustomerCode,
         //                user.ProfitCenterCode, false))
         //            {
         //                if (oOmTrSalesSODao.Update(record) > 0)
         //                {
         //                    if (user.CoProfile.ProductType == "4W")
         //                    {
         //                        foreach (DataRow row in additionalOwnership.Rows)
         //                            UpdateModelOwnerShip(ctx, user, record.CompanyCode, record.BranchCode, SONo, row["SalesModelCode"].ToString(), Convert.ToInt32(row["SalesModelYear"].ToString()), row["StatusVehicle"].ToString(), row["BrandCode"].ToString(), row["ModelName"].ToString());
         //                    }

         //                    if (islinkITS)
         //                    {
         //                        if (record.ProspectNo != "")
         //                        {
         //                            //if (!Update4ITSAdditional(ctx, Convert.ToInt32(record.ProspectNo), user.CompanyCode, user.BranchCode, user.UserId, "SPK", additionalOwnership))
         //                            //    throw new Exception("Update ITS Gagal");
         //                        }
         //                    }
         //                }
         //                else throw new Exception("Update SO Gagal");
         //            }
         //            else throw new Exception("Update Bank Book Gagal");
         //        }
         //        else throw new Exception("Approve SO Gagal");
         //    }
         //    catch (Exception ex)
         //    {
         //        result = false;
         //        ctx.RollBackTransaction();
         //        XLogger.Log(ex);
         //    }
         //    finally
         //    {
         //        if (result)
         //            ctx.CommitTransaction();
         //        else
         //            ctx.RollBackTransaction();
         //    }
         //    return result;
        //}
        #endregion
    }
}