CREATE procedure sp_SpSOSelectforLookup (  @CompanyCode varchar(10) ,@BranchCode varchar(10))  
 as  
select distinct(a.STNo)  
  , b.WarehouseCode  
  , b.STHdrNo  
  --, a.PartNo  
  from spTrnStockTakingTemp a with(nolock, nowait)  
       left join spTrnStockTakingHdr b with(nolock, nowait) on a.CompanyCode = b.CompanyCode  
   and a.Branchcode = b.Branchcode   
   and a.STHdrNo = b.STHdrNo   
 where a.status IN ('0', '1')  
   and b.status < 2  
   and a.CompanyCode = @CompanyCode  
   and a.Branchcode = @BranchCode



