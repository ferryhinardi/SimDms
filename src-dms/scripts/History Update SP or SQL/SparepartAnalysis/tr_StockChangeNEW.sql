-- =============================================
-- Author:		David
-- Create date: 12 January 2016
-- Description:	get stock for analysis weekly
-- =============================================
IF (OBJECT_ID('tr_StockChange') IS NOT NULL)
  DROP TRIGGER tr_StockChange
GO
CREATE TRIGGER tr_StockChange
   ON spMstItems 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN

DECLARE
@CompanyCode varchar(20),
@BranchCode varchar(20),
@PeriodYear numeric(4,0),
@PeriodMonth numeric(2,0),
@PeriodWeek numeric(1,0),
@PartNo varchar(20)

set @PeriodYear = year(getdate())
set @PeriodMonth =  month(getdate())
set @PeriodWeek = case when DAY(getdate()) <= 7 then 1 
     when DAY(getdate()) between 8 and 15 then 2 
	 when DAY(getdate()) between 16 and 22 then 3 
	 when DAY(getdate()) >= 23 then 4 end

select @CompanyCode = i.CompanyCode, @BranchCode = i.BranchCode, @PartNo = i.PartNo from inserted i

select CompanyCode,BranchCode,PeriodYear,PeriodMonth,PeriodWeek, TypeofGoods, isnull(sum(NilaiStock),0) NilaiStock into #t_items from(
select  i.CompanyCode,i.BranchCode, year(getdate()) PeriodYear, month(getdate()) PeriodMonth,  
case when DAY(getdate()) <= 7 then 1 
     when DAY(getdate()) between 8 and 15 then 2 
	 when DAY(getdate()) between 16 and 22 then 3 
	 when DAY(getdate()) >= 23 then 4 end AS PeriodWeek, i.TypeOfGoods, (i.OnHand * ip.CostPrice) NilaiStock from spmstitems i
	join spMstItemPrice ip
	on i.CompanyCode = ip.CompanyCode and i.BranchCode = ip.BranchCode and i.PartNo = ip.PartNo
	where  i.CompanyCode = @CompanyCode and i.BranchCode = @BranchCode and i.PartNo = @PartNo
	) #t_items
	group by CompanyCode, BranchCode,PeriodYear, PeriodMonth, PeriodWeek, TypeOfGoods
	
	declare @check int
	set @check = 	(select count(*) from spHstSparePartWeeklyStock where PeriodYear = @PeriodYear and PeriodMonth = @PeriodMonth 
						and PeriodWeek = @PeriodWeek and TypeOfGoods = (select TypeofGoods from #t_items))

	if @check > 0
	Begin
	update spHstSparePartWeeklyStock
	set NilaiStock = (select NilaiStock from #t_items)
	where CompanyCode = @CompanyCode and BranchCode = @BranchCode and PeriodYear = @PeriodYear and PeriodMonth = @PeriodMonth 
						and PeriodWeek = @PeriodWeek and TypeOfGoods =(select TypeofGoods from #t_items)
	end
	else

	Begin
	insert into spHstSparePartWeeklyStock select * , 'system' as CreatedBy, getdate() as CreatedDate, 'system' as LastUpdateBy, getdate() as LastUpdateDateFrom from #t_items 

	end

	drop table #t_items
END
GO
