using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrInventQtyVehicleBLL : BaseBLL
    {
        #region -- Initiate --
        public OmTrInventQtyVehicleBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public OmTrInventQtyVehicle GetRecord(Decimal year, Decimal month, string salesModelCode, Decimal salesModelYear, string colourCode, string warehouseCode)
        {
            var record = ctx.OmTrInventQtyVehicles.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.Year == year && p.Month == month && p.SalesModelCode == salesModelCode && p.SalesModelYear == salesModelYear 
                && p.ColourCode == colourCode && p.WarehouseCode == warehouseCode).FirstOrDefault();
       
            return record;
        }

        public OmTrInventQtyVehicle BeginningQtyVehicle(Decimal year, Decimal month, string salesModelCode, Decimal salesModelYear, string colourCode, string warehouseCode)
        {
            var record = ctx.OmTrInventQtyVehicles.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && (p.Year == year && p.Month < month || p.Year < year) && p.SalesModelCode == salesModelCode
                && p.SalesModelYear == salesModelYear && p.ColourCode == colourCode && p.WarehouseCode == warehouseCode)
                .OrderByDescending(p => p.Year).OrderByDescending(p => p.Month).FirstOrDefault();

            return record;
        }

        #endregion
    }
}