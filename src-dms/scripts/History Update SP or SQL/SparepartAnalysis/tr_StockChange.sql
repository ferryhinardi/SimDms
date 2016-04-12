-- =============================================
-- Author:		David
-- Create date: 12 January 2016
-- Description:	get stock for analysis weekly
-- =============================================
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
set @PeriodWeek = (SELECT DATEPART(WEEK, getdate())  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,getdate()), 0))+ 1)

select @CompanyCode = i.CompanyCode, @BranchCode = i.BranchCode, @PartNo = i.PartNo from inserted i

select CompanyCode,BranchCode,PeriodYear,PeriodMonth,PeriodWeek, TypeofGoods, isnull(sum(NilaiStock),0) NilaiStock into #t_items from(
select  i.CompanyCode,i.BranchCode, year(getdate()) PeriodYear, month(getdate()) PeriodMonth,  
(SELECT DATEPART(WEEK, getdate())  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,getdate()), 0))+ 1) AS PeriodWeek, i.TypeOfGoods, (i.OnHand * ip.CostPrice) NilaiStock from spmstitems i
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
