create proc [dbo].[uspfn_spLiveStockPartParam] 
as  
begin  
 select distinct a.PartNo, b.PartName      
 from spMstItems a       
 join spMstItemInfo b      
 on a.CompanyCode = b.CompanyCode and a.PartNo = b.PartNo  
 order by a.PartNo   
end