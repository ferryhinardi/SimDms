create proc [dbo].[uspfn_spLiveStockArea]    
as    
begin    
 select distinct Area as value, Area as text, max(GroupNo) as groupNo     
 from GnMstDealerMapping    
 group by Area    
 order by max(GroupNo), Area    
end