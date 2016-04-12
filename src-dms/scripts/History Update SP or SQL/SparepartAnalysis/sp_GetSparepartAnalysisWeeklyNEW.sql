-- =============================================
-- Author:		David
-- Create date: 15 January 2016
-- Description:	Get Sparepart Analysis Weekly
-- =============================================
IF (OBJECT_ID('sp_GetSparepartAnalysisWeekly') IS NOT NULL)
  DROP PROCEDURE sp_GetSparepartAnalysisWeekly
GO
CREATE PROCEDURE sp_GetSparepartAnalysisWeekly 
	    @CompanyCode varchar(15),
		@BranchCode varchar(15),
		@PeriodYear numeric(4,0),
	    @PeriodMonth numeric(2,0),
		@TypeOfGoods varchar(15)
AS
BEGIN

declare
@startDate DATETIME,
@endDate DATETIME,
@countWeek int,
@first int 

SET @startDate = CONVERT(datetime, (CONVERT(varchar,@PeriodYear) +'-'+ CONVERT(varchar,@PeriodMonth)+'-01'))
    
set @endDate = (select DATEADD(MONTH, DATEDIFF(MONTH, -1, @startDate), -1)) --Last Day of previous month

set @countWeek = case when DAY(@endDate) <= 7 then 1 
     when DAY(@endDate) between 8 and 15 then 2 
	 when DAY(@endDate) between 16 and 22 then 3 
	 when DAY(@endDate) >= 23 then 4 end

declare @tblTemp as table  
(  
  PeriodWeek numeric(1,0)  
) 

set @first = 1

BEGIN TRANSACTION
WHILE(@first <= @countWeek) 
BEGIN INSERT INTO @tblTemp VALUES(@first) SET @first += 1 END
COMMIT TRANSACTION

select * into #t_spAWeekly from(
SELECT PeriodWeek, ISNULL(SUM(Netto_WS),0)Netto_WS, ISNULL(SUM(HPP_WS),0) HPP_WS, ISNULL(SUM(Netto_PS),0)Netto_PS, ISNULL(SUM(HPP_PS),0)HPP_PS
,ISNULL(SUM(Netto_C),0) Netto_C, ISNULL(SUM(HPP_C),0) HPP_C, ISNULL(SUM(Netto_U),0) Netto_U, ISNULL(SUM(HPP_U),0) HPP_U,ISNULL(SUM(NilaiStock),0) NilaiStock from spHstSparepartWeekly
where CompanyCode = @CompanyCode and PeriodYear = @PeriodYear and PeriodMonth = @PeriodMonth
and BranchCode = CASE WHEN @BranchCode != '' THEN @BranchCode ELSE BranchCode END
and TypeOfGoods = CASE WHEN @TypeOfGoods != '' THEN @TypeOfGoods ELSE TypeOfGoods END
GROUP BY PeriodWeek) #t_spAWeekly

select a.PeriodWeek, ISNULL(Netto_WS,0) Netto_WS, ISNULL(HPP_WS,0) HPP_WS, ISNULL(Netto_PS,0) Netto_PS, ISNULL(HPP_PS,0) HPP_PS
,ISNULL(Netto_C,0) Netto_C, ISNULL(HPP_C,0) HPP_C,ISNULL(Netto_U,0) Netto_U, ISNULL(HPP_U,0) HPP_U, ISNULL(NilaiStock,0) NilaiStock from @tblTemp a left join 
#t_spAWeekly b on a.PeriodWeek = b.PeriodWeek

drop table #t_spAWeekly

END
GO
