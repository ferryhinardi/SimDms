IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqLostCaseWeb]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqLostCaseWeb]
GO

-- =============================================
-- Author:		<Rijal AL>
-- Create date: <26 Mar 2015>
-- Description:	<Analisis Lost Case For Web>
-- =============================================

CREATE PROCEDURE [dbo].[usprpt_PmRpInqLostCaseWeb] 
@CompanyCode		VARCHAR(15) , --= '6159401000',
	@BranchCode			VARCHAR(15) , --= '6159401001',
	@PeriodBegin		VARCHAR(15) , --= '20140101',
	@PeriodEnd			VARCHAR(15) , --= '20140330',
	@BranchManager		VARCHAR(15) , --= '3-BIT',
	@SalesHead			VARCHAR(15) , --= '028',
	@SalesCoordinator	VARCHAR(15) , --= '028',
	@Salesman			VARCHAR(15) --= ''


AS
BEGIN
	SET NOCOUNT ON;
-- Get EmployeeID
--=======================================================================
DECLARE @SalesmanID		VARCHAR(MAX);

if @SalesHead = '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader in(
	select EmployeeID from HrEmployee where TeamLeader in (
		select EmployeeID from HrEmployee where TeamLeader = '''+@BranchManager+'''))
	union
	select EmployeeID from HrEmployee where TeamLeader in (
			select EmployeeID from HrEmployee where TeamLeader = '''+@BranchManager+''')'
end
else if @SalesHead <> '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader in(
	select EmployeeID from HrEmployee where TeamLeader = '''+@SalesHead+''')
	union
	select EmployeeID from HrEmployee where TeamLeader  = '''+@SalesHead+''''
end
else
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where EmployeeID  = '''+@Salesman+''''
end
--=======================================================================

-- Group By Tipe Kendaraan
--=======================================================================
DECLARE @ByTipeKendaraan		VARCHAR(MAX);
DECLARE @Query1		VARCHAR(MAX);

set @ByTipeKendaraan = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.TipeKendaraan, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+convert(varchar(30),@PeriodBegin)+''' AND '''+convert(varchar(30),@PeriodEnd)+''')
	 and EmployeeID in ('+@SalesmanID+')
	 and a.TipeKendaraan <> '''''

set @Query1 = 'SELECT 
	DISTINCT(TipeKendaraan),
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress <> ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) Active, 
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress = ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) NonActive
	FROM ('+@ByTipeKendaraan+') a'
	
exec (@Query1)

-- Group By Perolehan Data
--======================================================================
DECLARE @ByPerolehanData		VARCHAR(MAX);
DECLARE @Query2		VARCHAR(MAX);

set @ByPerolehanData = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.PerolehanData, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and EmployeeID in ('+@SalesmanID+')'

set @Query2 = 'SELECT 
	DISTINCT(PerolehanData),
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress <> ''LOST'' AND a.PerolehanData = b.PerolehanData) Active, 
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress = ''LOST'' AND a.PerolehanData = b.PerolehanData) NonActive
	FROM ('+@ByPerolehanData+') a'
	
exec (@Query2)

-- Group By Salesman
--=====================================================================
DECLARE @BySalesman		VARCHAR(MAX);
DECLARE @Query3		VARCHAR(MAX);

set @BySalesman = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, b.EmployeeName, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a
	inner join HrEmployee b
		ON b.CompanyCode = a.CompanyCode and b.EmployeeID = a.EmployeeID
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (EmployeeName) Karyawan,
	(select count(*) from ('+@BySalesman+') b where lastprogress <> ''LOST'' AND a.EmployeeID = b.EmployeeID) Active, 
	(select count(*) from ('+@BySalesman+') b where lastprogress = ''LOST'' AND a.EmployeeID = b.EmployeeID) NonActive
	FROM ('+@BySalesman+') a'
	
exec (@Query3)

-- Group By Sales Coordinator
--=====================================================================
DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query4		VARCHAR(MAX);
set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query4 = 'SELECT 
	DISTINCT (BranchName) Supervisor,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query4)

-- Group By Sales Head
--=====================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query5		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query5 = 'SELECT 
	DISTINCT (BranchName) SalesHead,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query5)

-- Group By Branch Name
--=======================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query6		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (BranchName),
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query3)

-- Query Utama
--=======================================================================================
DECLARE @Utama		VARCHAR(MAX);

set @Utama = 'SELECT
 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.NamaProspek, a.Inquirydate, ISNULL(a.TipeKendaraan,''-'') TipeKendaraan, 
 ISNULL(a.Variant,''-'') Variant, ISNULL(a.Transmisi,''-'') Transmisi,
 b.RefferenceDesc1 Warna, a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor,
 a.LastProgress, e.UpdateDate TglLost, f.LookUpValueName KategoriLost, g.LookUpValueName Reason,
 a.LostCaseVoiceofCustomer VOC, a.SpvEmployeeID
FROM
 PmKDP a
LEFT JOIN OmMstRefference b
ON b.CompanyCode = a.CompanyCode AND b.RefferenceType = ''COLO'' AND b.RefferenceCode = a.ColourCode
LEFT JOIN HrEmployee c
ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
LEFT JOIN HrEmployee d
ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmStatusHistory e
ON e.InquiryNumber = a.InquiryNumber AND e.CompanyCode = a.CompanyCode 
AND e.BranchCode = a.BranchCode AND e.SequenceNo = (SELECT TOP 1 SequenceNo FROM PmStatusHistory
		WHERE InquiryNumber = a.InquiryNumber AND CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode AND LastProgress=''LOST'' ORDER BY SequenceNo DESC)
LEFT JOIN GnMstLookUpDtl f
ON f.CompanyCode = a.CompanyCode AND f.CodeID = ''PLCC'' AND f.LookUpValue = a.LostCaseCategory
LEFT JOIN GnMstLookUpDtl g
ON g.CompanyCode = a.CompanyCode AND g.CodeID = ''ITLR'' AND g.LookUpValue = a.LostCaseReasonID
WHERE
 a.CompanyCode = '''+@CompanyCode+''' 
 AND a.BranchCode = '''+@BranchCode+'''
 AND a.LastProgress = ''LOST'' 
 AND CONVERT(VARCHAR, e.UpdateDate, 112) BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''' 
 AND a.EmployeeID in ('+@SalesmanID+')'

Exec (@Utama)

-- Pivot
--=====================================================================
declare	@columns			VARCHAR(MAX)
declare	@columns2			VARCHAR(MAX)
declare	@Pivot				VARCHAR(MAX)

select	@columns = coalesce(@columns + ',[' + cast(LookUpValue as varchar) + ']',
				'[' + cast(LookUpValue as varchar)+ ']') 
		,@columns2 = coalesce(@columns2 + ',isnull([' + cast(LookUpValue as varchar) + '],0) as '+ LookUpValue +'',
		'isnull([' + cast(LookUpValue as varchar)+ '],0) as '+ LookUpValue +'')
from
(
	select	a.LookUpValue
	from	gnMstLookUpDtl a
	where	CompanyCode=@CompanyCode and CodeID='PLCC'
) as x

set @Pivot='
select 
	p.TipeKendaraan, '+ @columns2 +'
from (
	select 
		a.TipeKendaraan
		,d.LookupValue LostCaseCategory
		,count(d.LookupValue) Quantity
	from 
		pmKDP a	
	left join PmStatusHistory b
	on b.InquiryNumber = a.InquiryNumber AND b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory
			where InquiryNumber = a.InquiryNumber and CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode and LastProgress=''LOST'' order by SequenceNo desc)
	left join gnMstLookUpDtl d on d.CompanyCode=a.CompanyCode and CodeID=''PLCC'' 
		and LookUpValue=a.LostCaseCategory
	where
		a.LastProgress= ''LOST''
		and a.EmployeeID in ('+@SalesmanID+')
		and convert(varchar, b.UpdateDate, 112) between '''+@PeriodBegin+''' and '''+@PeriodEnd+''' 
	group by
		a.CompanyCode,a.BranchCode,d.LookupValue,a.TipeKendaraan
) as b
pivot
(
	sum(Quantity)
	for LostCaseCategory 
	in ('+@columns+')
) as p
order by p.TipeKendaraan'


exec(@Pivot)



-- Get GnMstLookUpDtl (Kategori Lost Case) 
--===========================
SELECT LookupValue+' : '+LookUpValueName AS Kategori, LookupValue FROM gnMstLookupDtl 
WHERE CompanyCode=@CompanyCode AND CodeID='PLCC'

END
GO


