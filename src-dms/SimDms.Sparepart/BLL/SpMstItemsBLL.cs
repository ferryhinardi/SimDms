using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class SpMstItemsBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static SpMstItemsBLL _SpMstItemsBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static SpMstItemsBLL Instance(string _username)
        {
            if (_SpMstItemsBLL == null)
            {
                _SpMstItemsBLL = new SpMstItemsBLL();
            }
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _SpMstItemsBLL;
        }
        #endregion


        /// <summary>
        /// /
        /// </summary>
        /// <param name="warehouseCode"></param>
        /// <returns></returns>
        public List<string> GetPartNo4ValidationStockTaking(string warehouseCode)
        {   
            
                var ret = ctx.spMstItems.Join(ctx.SpMstItemLocs,
                    a => new { a.CompanyCode, a.BranchCode, a.PartNo },
                    b => new { b.CompanyCode, b.BranchCode, b.PartNo },
                    (a, b) => new { spMstItem = a, SpMstItemLoc = b })
                    .Where( x=>x.spMstItem.CompanyCode==CompanyCode &&
                                x.spMstItem.BranchCode==BranchCode &&
                                x.spMstItem.ProductType==ProductType&&
                                x.SpMstItemLoc.WarehouseCode==warehouseCode &&
                                (x.SpMstItemLoc.AllocationSP+x.SpMstItemLoc.AllocationSR+x.SpMstItemLoc.AllocationSL
                                +x.SpMstItemLoc.ReservedSP+x.SpMstItemLoc.ReservedSR+x.SpMstItemLoc.ReservedSL
                                +x.spMstItem.InTransit>0)
                                //&& x.SpMstItemLoc.LocationCode=="GEN" 
                                )
                    .Select(x => x.spMstItem.PartNo)
                    .ToList();
                return ret;
                     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseCode"></param>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public List<string> GetPartNo4ValidationStockTaking(string warehouseCode, string locationCode)
        {
            string sql = string.Format(@"
SELECT
	a.PartNo
FROM
	spMstItems a
	LEFT JOIN spMstItemLoc b ON a.CompanyCode = b.CompanyCode AND 
        a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	a.CompanyCode = '{0}'
	AND a.BranchCode = '{1}'
	AND b.WarehouseCode = '{2}'
    AND b.LocationCode Like '{3}'", CompanyCode, BranchCode, warehouseCode, locationCode);

           
            return ctx.Database.SqlQuery<string>(sql).ToList<string>(); 

                //var ret = ctx.spMstItems.Join(ctx.SpMstItemLocs,
                //       a => new { a.CompanyCode, a.BranchCode, a.PartNo },
                //       b => new { b.CompanyCode, b.BranchCode, b.PartNo },
                //       (a, b) => new { spMstItem = a, SpMstItemLoc = b })
                //       .Where(x => x.spMstItem.CompanyCode == CompanyCode &&
                //                   x.spMstItem.BranchCode == BranchCode &&
                //                   x.spMstItem.ProductType == ProductType &&
                //                   x.SpMstItemLoc.WarehouseCode == warehouseCode &&                                   
                //                    x.SpMstItemLoc.LocationCode.Contains(locationCode)
                //                   )                       
                //       .Select(x => x.spMstItem.PartNo)
                //       .ToList();
                //return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mdl"></param>
        /// <returns></returns>
        public bool isMainLocation(SpTrnStockTakingDtl mdl)
        {
            var dtl = ctx.SpTrnStockTakingDtls
                       .Where(x => x.CompanyCode == CompanyCode
                           && x.BranchCode == BranchCode
                           && x.STHdrNo == mdl.STHdrNo
                           && x.STNo==mdl.STNo)
                           .ToList();

            
            
            
            Func<bool> ismnloc = () => dtl.Where(x => x.PartNo == mdl.PartNo 
                                            && x.isMainLocation == mdl.isMainLocation)
                                            .Count() > 0;

            if (dtl.Where(x => x.SeqNo == mdl.SeqNo).Count() == 0)
            {
                return ismnloc();
            }
            else
            {
                if (dtl.Where(x => x.SeqNo == mdl.SeqNo).SingleOrDefault().isMainLocation)
                    return true;

                return ismnloc();
            }
        }

        /// <summary>
        /// Get spMstItem Data Record
        /// </summary>
        /// <param name="newPartNo"></param>
        /// <returns>spMstItem record</returns>
        public spMstItem GetRecord(string newPartNo)
        {
            var query = string.Format(@"select * from {0}..spMstItems where CompanyCode = '{1}' and BranchCode = '{2}' and PartNo = '{3}'", GetDbMD(), CompanyMD, BranchMD, newPartNo);
            var record = ctx.Database.SqlQuery<spMstItem>(query).FirstOrDefault();
            //var record = ctx.spMstItems.Find(CompanyCode, BranchCode, newPartNo);

            return record;
        }

        /// <summary>
        /// Update spMstItem data
        /// </summary>
        /// <returns>Int savechange</returns>
        public int Update()
        {
            return ctx.SaveChanges();
        }


    }
}
