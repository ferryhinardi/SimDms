create proc [dbo].[uspfn_spLiveStockPart] --'%', '%'  
(  
	@area varchar(100) = '',  
	@partno varchar(20) = ''  
)  
as  
begin  
	select row_number() over (ORDER BY PartName) AS [No], PartNo, PartName   
	from  
		(select distinct b.PartNo, c.PartName from gnMstDealerMapping a  
		join spMstItems b  
		on a.DealerCode = b.CompanyCode  
		join spMstItemInfo c  
		on b.CompanyCode = c.CompanyCode and b.PartNo = c.PartNo  
		where Area like @area and b.PartNo like @partno) t1  
	order by PartName  
end  
  
  
  