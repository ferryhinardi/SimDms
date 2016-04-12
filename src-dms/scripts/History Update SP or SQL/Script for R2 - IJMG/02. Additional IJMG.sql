CREATE VIEW [dbo].[SvStallView]
AS
SELECT        CompanyCode, BranchCode, ProductType, StallCode, Description, HaveLift, IsActive, CASE HaveLift WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS HaveLiftString, 
                         CASE IsActive WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END AS IsActiveString
FROM            dbo.svMstStall
GO

create view [dbo].[VW_ROLEMENUPERMISSION]
AS
select b.MenuCaption, b.MenuHeader, b.MenuLevel, b.MenuIndex,   a.*
from SysRoleMenuAccess a
inner join vw_treemenus b on (a.menuid = b.menuid)
GO

if object_id('uspfn_HrInqEmployeeSubOrdinates') is not null
	drop procedure uspfn_HrInqEmployeeSubOrdinates
GO
create procedure [dbo].[uspfn_HrInqEmployeeSubOrdinates]  
--declare  
 @CompanyCode varchar(20),  
 @EmployeeID varchar(20),
 @Status varchar(10)
as  
 
--select @CompanyCode = '6006406', @EmployeeID = '341'
select a.CompanyCode, a.EmployeeID, a.EmployeeName
     , a.JoinDate
     , LastPosition = upper(a.Department)
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)
         + upper(case isnull(c.LookUpValueName, '') when '' then '' else ' - ' + c.LookUpValueName end)
		 , Status = d.LookUpValueName
  from HrEmployee a
  left join GnMstPosition b
    on b.CompanyCode = a.CompanyCode
   and b.DeptCode = a.Department
   and b.PosCode = a.Position
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'ITSG'
   and c.LookUpValue = a.Grade
   left join GnMstLookUpDtl d
   on a.CompanyCode = d.CompanyCode
   and d.CodeID='PERS' and d.LookUpValue=a.PersonnelStatus
 where a.CompanyCode = @CompanyCode
   and a.TeamLeader = @EmployeeID 
   and a.PersonnelStatus = case when @status='' then a.PersonnelStatus else @status end
 order by a.EmployeeID
GO



if object_id('sfm_GetLatestEmployeeData') is not null
	drop procedure sfm_GetLatestEmployeeData
GO
create procedure [dbo].[sfm_GetLatestEmployeeData] (
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@CurrentCompanyCode varchar(25) output,
	@CurrentBranchCode varchar(25) output,
	@CurrentEmployeeID varchar(25) output
)
as
begin
--	declare @CompanyCode varchar(25);
--	declare @BranchCode varchar(25);
--	DECLARE @EmployeeID varchar(25);
	declare @ActiveEmployeeID varchar(25);
	declare @LastActiveBranch varchar(25);
	DECLARE @IsEmployeeActive bit;
	declare @TempEmployeeDataTable table (
		CompanyCode varchar(15),
		BranchCode varchar(15),
		EmployeeID varchar(25)
	);

--	SET @EmployeeID = '2 200 726';
--	set @EmployeeID = '0.000.020';
--	set @CompanyCode = '6015401';
	SET @IsEmployeeActive = 0;

	set @LastActiveBranch = (
		select top 1
			a.MutationTo
		from
			gnMstEmployeeMutation a
		where
			a.CompanyCode = @CompanyCode
			and
			a.EmployeeID = @EmployeeID
			and
			a.IsValid = 1
		order by
			a.MutationDate desc
	);

	if @LastActiveBranch is not null
	begin	
		set @ActiveEmployeeID = (
			select top 1
				a.EmployeeID
			from
				gnMstEmployee a
			inner join
				gnMstEmployeeData b
			on 
				b.CompanyCode = a.CompanyCode
				and
				b.BranchCode = a.BranchCode
				and
				b.EmployeeID = a.EmployeeID
			where
				a.CompanyCode = @CompanyCode
				and
				a.BranchCode = @LastActiveBranch
				and
				a.EmployeeID = @EmployeeID
		);

		set @CurrentCompanyCode = @CompanyCode;
		set @CurrentBranchCode = @LastActiveBranch;
		set @CurrentEmployeeID = @EmployeeID;
	end
	else
	begin
		set @ActiveEmployeeID = (
			select top 1
				a.EmployeeID
			from 
				gnMstEmployee a
			where
				a.CompanyCode = @CompanyCode
				and
				a.EmployeeID = @EmployeeID
				and
				a.PersonnelStatus = '1'
				and
				exists (
					select top 1
						x.EmployeeID
					from
						gnMstEmployeeData x
					where
						x.CompanyCode = a.CompanyCode
						and
						x.BranchCode = a.BranchCode
						and
						x.EmployeeID = a.EmployeeID
				)
		);

		if @ActiveEmployeeID is not null 
			begin
				set @IsEmployeeActive = 1;
			end
		else
			begin
				set @IsEmployeeActive = 0;
			end
	
		delete FROM @TempEmployeeDataTable;	

		IF @IsEmployeeActive = 1 
			BEGIN
				insert into @TempEmployeeDataTable
				select TOP 1
					a.CompanyCode,
					a.BranchCode,
					a.EmployeeID
				from 
					gnMstEmployee a
				inner join 
					gnMstEmployeeData b
				on
					b.CompanyCode = a.CompanyCode
					and
					b.BranchCode = a.BranchCode
					and
					b.EmployeeID = a.EmployeeID
				where
					a.CompanyCode = @CompanyCode
					and
					a.EmployeeID = @EmployeeID
					and
					a.PersonnelStatus = '1'
			end
		else
			BEGIN
				declare @CreatedDate datetime;
				declare @UpdatedDate datetime;
				declare @LatestDate datetime;

				set @CreatedDate = getdate();
				set @UpdatedDate = getdate();
				set @LatestDate = getdate();

				select @CreatedDate = (
						SELECT
							max(a.CreatedDate)
						from
							gnMstEmployee a
						inner join 
							gnMstEmployeeData b
						on
							a.CompanyCode = b.BranchCode
							and
							a.BranchCode = b.BranchCode
							and
							a.EmployeeID = b.EmployeeID
						WHERE 
							a.CompanyCode = @CompanyCode
							and
							a.EmployeeID = @EmployeeID
							and
							b.CreatedDate IS not null
					);
			
					set @UpdatedDate = (
							SELECT
								max(a.LastupdateDate)
							from
								gnMstEmployee a
							inner join 
								gnMstEmployeeData b
							on
								a.CompanyCode = b.CompanyCode
								and
								a.BranchCode = b.BranchCode
								and
								a.EmployeeID = b.EmployeeID
							WHERE 
								a.CompanyCode = @CompanyCode
								and
								a.EmployeeID = @EmployeeID
								and
								b.UpdatedDate IS not null
						);

					set @LatestDate = (
							SELECT 
								max(val)
							from
								(
									SELECT @CreatedDate as val
									union all
									SELECT @UpdatedDate as val
								) as datetimes
						)

					insert INTO @TempEmployeeDataTable
						select TOP 1
							a.CompanyCode,
							a.BranchCode,
							a.EmployeeID
						from
							gnMstEmployee a
						where
							a.CompanyCode = @CompanyCode
							and
							a.EmployeeID = @EmployeeID
							and
							(
								a.CreatedDate = @LatestDate
								OR
								a.LastupdateDate = @LatestDate
							)	
							and
							exists (
								select 
									x.EmployeeID
								from
									gnMstEmployeeData x
								where
									x.CompanyCode = a.CompanyCode
									and
									x.BranchCode = a.BranchCode
									and
									x.EmployeeID = a.EmployeeID
							)
						end

			--	select * from @TempEmployeeDataTable;
				set @CurrentCompanyCode = (select TOP 1 CompanyCode from @TempEmployeeDataTable);
				set @CurrentBranchCode = (select TOP 1 BranchCode from @TempEmployeeDataTable);
				set @CurrentEmployeeID = (select TOP 1 EmployeeID from @TempEmployeeDataTable);
	end;
end;
GO
if object_id('uspfn_OmInquiryChassisDO') is not null
	drop procedure uspfn_OmInquiryChassisDO
GO
-- uspfn_OmInquiryChassisDO '6115202','611520200','SOA/11/000287','FU150 SCD',2011,'MH8BG41CABJ','COLO','00'
create procedure [dbo].[uspfn_OmInquiryChassisDO]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@SONo varchar(15)
	,@SalesModelCode varchar(30)
	--,@SalesModelYear int
	,@SalesModelYear varchar(15)
	,@ChassisCode varchar(15)
	,@RefType varchar(15)
	,@WarehouseCode varchar(15)

as

--declare @CompanyCode varchar(15)
--,@BranchCode varchar(15)
--,@SONo varchar(15)
--,@SalesModelCode varchar(15)
--,@SalesModelYear int
--,@ChassisCode varchar(15)
--,@RefType varchar(15)
--,@WarehouseCode varchar(15)
--
--select @CompanyCode='6115202'
--,@BranchCode='611520200'
--,@SONo='SOA/11/000287' 
--,@SalesModelCode='FU150 SCD' 
--,@SalesModelYear=2011
--,@ChassisCode='MH8BG41CABJ' 
--,@RefType='COLO'
--,@WarehouseCode='00'

declare 
@val as int,
@CompanyMD as varchar(15)
,@DBMD as varchar(15)
,@QryTemp as varchar(max)


DECLARE @columnVal TABLE (columnVal int);

set @val=0

set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

--set @val= (
set @QryTemp = 'SELECT isnull(count (a.ChassisNo),0) jml
FROM omTrSalesSOVin a 
	INNER JOIN ' + @DBMD + '.dbo.omMstVehicle b ON 
		a.ChassisCode = b.ChassisCode 
		AND a.ChassisNo = b.ChassisNo 
WHERE a.CompanyCode = ''' + @CompanyCode + '''
	AND a.BranchCode = ''' + @BranchCode + ''' 
	AND a.SONo = ''' + @SONo + '''
	AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
	AND a.SalesModelYear = ''' + @SalesModelYear + '''
	AND a.ChassisCode = ''' + @ChassisCode + '''
	and b.Status in (0,3)
	AND not exists 
	( 
		SELECT 1 
		FROM omTrSalesDODetail x
			inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
				and x.DONo=y.DONo
		WHERE x.CompanyCode = a.CompanyCode 
		AND x.BranchCode = a.BranchCode 
		AND x.SalesModelCode = a.SalesModelCode 
		AND x.ChassisCode = a.ChassisCode 
		and x.ChassisNo= a.ChassisNo
		and y.Status in (0,1)
	) 
	AND a.ChassisNo != 0 ' 

INSERT INTO @columnVal EXEC (@QryTemp);

set @val= (SELECT * FROM @columnval);

if @val = 0 
begin
	set @QryTemp =
	'SELECT 
		a.ChassisCode , a.ChassisNo , a.EngineCode , a.EngineNo , 
		a.ColourCode , b.RefferenceDesc1 AS ColourName 
	FROM ' + @DBMD + '.dbo.omMstVehicle a 
		LEFT JOIN ' + @DBMD + '.dbo.omMstRefference b ON b.CompanyCode = a.CompanyCode 
			AND b.RefferenceType = ''' + @RefType + ''' 
			AND b.RefferenceCode = a.ColourCode 
	WHERE a.CompanyCode = ''' + @CompanyMD + '''  
		AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
		AND a.ChassisCode = ''' + @ChassisCode + ''' 
		AND a.Status = 0 
		AND isnull (a.SONo,'''') = ''''  
		AND a.WarehouseCode = ''' + @WarehouseCode + '''
		AND exists 
		( 
			SELECT z.colourCode FROM OmTrSalesSOModelColour z 
			WHERE z.companyCode = a.CompanyCode 
			AND z.BranchCode = ''' + @BranchCode + '''
			AND z.SONo= ''' + @SONo + ''' 
			AND z.SalesModelCode = a.SalesModelCode 
			AND z.SalesModelYear = a.SalesModelYear 
			and z.ColourCode= a.ColourCode
		)'
		Exec (@QryTemp);
end
else
begin
	set @QryTemp =
	'SELECT 
		a.ChassisNo , a.EngineCode , a.EngineNo , 
		a.ColourCode , b.RefferenceDesc1 AS ColourName 
	FROM omTrSalesSOVin a 
		LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode 
			AND b.RefferenceType = ''' + @RefType + ''' 
			AND b.RefferenceCode = a.ColourCode 
		INNER JOIN ' + @DBMD + '.dbo.omMstVehicle c ON 
			a.ChassisCode = c.ChassisCode 
			AND a.ChassisNo = c.ChassisNo 
	WHERE a.CompanyCode = ''' + @CompanyCode + ''' 
		AND a.BranchCode = ''' + @BranchCode + '''  
		AND a.SONo = ''' + @SONo + ''' 
		AND a.SalesModelCode = ''' + @SalesModelCode + '''
		AND a.SalesModelYear = ''' + @SalesModelYear + ''' 
		AND a.ChassisCode = ''' + @ChassisCode + ''' 
		AND c.WarehouseCode = ''' + @WarehouseCode + '''
		and c.Status in (0,3)
		AND not exists
		( 
			SELECT 1 
			FROM omTrSalesDODetail x
				inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
					and x.DONo=y.DONo
			WHERE x.CompanyCode = a.CompanyCode 
			AND x.BranchCode = a.BranchCode 
			AND x.SalesModelCode = a.SalesModelCode 
			AND x.ChassisCode = a.ChassisCode 
			and x.ChassisNo= a.ChassisNo
			and y.Status in (0,1)
		)'
		Exec (@QryTemp);
end
GO

if object_id('uspfn_SvTrnInvoiceGet') is not null
	drop procedure uspfn_SvTrnInvoiceGet
GO
create procedure [dbo].[uspfn_SvTrnInvoiceGet]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@InvoiceNo   varchar(20)
as  

select a.CompanyCode
     , a.BranchCode
     , a.ProductType
     , a.InvoiceNo
     , a.JobOrderNo
     , a.JobOrderDate
     , b.ServiceNo
     , a.PoliceRegNo
     , b.ServiceBookNo
     , a.BasicModel
     , b.TransmissionType
     , a.ChassisCode
     , a.ChassisNo
     , a.EngineCode
     , a.EngineNo
     , b.ColorCode
     , rtrim(rtrim(b.ColorCode)
     + case isnull(c.RefferenceDesc2,'') when '' then '' else ' - ' end
     + isnull(c.RefferenceDesc2,'')) as ColorCodeDesc
     , a.Odometer
     , isnull(d.IsContractStatus, 0) as IsContract
     , isnull(d.ContractNo, '') ContractNo
     , e.EndPeriod as ContractEndPeriod
     , isnull(e.IsActive, 0) ContractStatus
	 , case d.IsContractStatus 
		 when 1 then (case e.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		 else ''
	   end ContractStatusDesc
     , f.IsClubStatus IsClub
     , f.ClubCode
     , f.ClubDateFinish ClubEndPeriod
     , isnull(g.IsActive, 0) ClubStatus
	 , case f.IsClubStatus
		 when 1 then (case g.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		 else ''
	   end ClubStatusDesc
     , a.CustomerCode
     , h.CustomerName
     , h.Address1 CustAddr1
     , h.Address2 CustAddr2
     , h.Address3 CustAddr3
     , h.Address4 CustAddr4
     , h.CityCode
     , j.LookupValueName CityName
     , a.CustomerCodeBill
     , i.CustomerName CustomerNameBill
     , i.Address1 CustAddr1Bill
     , i.Address2 CustAddr2Bill
     , i.Address3 CustAddr3Bill
     , i.Address4 CustAddr4Bill
     , i.CityCode CityCodeBill
     , k.LookupValueName CityNameBill
     , i.PhoneNo
     , i.FaxNo
     , i.HPNo
     , a.LaborDiscPct
     , a.PartsDiscPct
     , a.MaterialDiscPct
     , a.ServiceRequestDesc
     , a.JobType
     , l.Description JobTypeDesc
     , b.ForemanID
     , m.EmployeeName ForemanName
     , b.MechanicID
     , n.EmployeeName MechanicName
     , b.ConfirmChangingPart
     , b.EstimateFinishDate
     , a.LaborDppAmt
     , a.PartsDppAmt
     , a.MaterialDppAmt
     , a.TotalDppAmt
     , a.TotalPpnAmt
     , a.TotalSrvAmt
     , b.ServiceStatus
     --, a.ServiceStatusDesc     
  from svTrnInvoice a with (nowait,nolock)
  left join svTrnService b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.JobOrderNo = a.JobOrderNo
  left join omMstRefference c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.RefferenceType = 'COLO'
   and c.RefferenceCode = b.ColorCode
  left join svMstCustomerVehicle d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
   and d.IsContractStatus = 1
  left join svMstContract e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.ContractNo = d.ContractNo
  left join svMstCustomerVehicle f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode = a.ChassisCode
   and f.ChassisNo = a.ChassisNo
   and f.IsClubStatus = 1
  left join svMstClub g with (nowait,nolock)
    on g.CompanyCode = a.CompanyCode
   and g.ClubCode = f.ClubCode
  left join gnMstCustomer h with (nowait,nolock)
    on h.CompanyCode = a.CompanyCode
   and h.CustomerCode = a.CustomerCode
  left join gnMstCustomer i with (nowait,nolock)
    on i.CompanyCode = a.CompanyCode
   and i.CustomerCode = a.CustomerCodeBill
  left join gnMstLookupDtl j with (nowait,nolock)
    on j.CompanyCode = a.CompanyCode
   and j.CodeID = 'CITY'
   and j.LookUpValue = h.CityCode
  left join gnMstLookupDtl k with (nowait,nolock)
    on k.CompanyCode = a.CompanyCode
   and k.CodeID = 'CITY'
   and k.LookUpValue = i.CityCode
  left join SvMstRefferenceService l with (nowait,nolock)
    on l.CompanyCode = a.CompanyCode
   and l.ProductType = a.ProductType
   and l.RefferenceCode = a.JobType
   and l.RefferenceType = 'JOBSTYPE'
  left join gnMstEmployee m with (nowait,nolock)
    on m.CompanyCode = a.CompanyCode
   and m.BranchCode = a.BranchCode
   and m.EmployeeID = b.ForemanID
  left join gnMstEmployee n with (nowait,nolock)
    on n.CompanyCode = a.CompanyCode
   and n.BranchCode = a.BranchCode
   and n.EmployeeID = b.MechanicID
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   and a.InvoiceNo = @InvoiceNo
GO

if object_id('uspfn_SyncPmHstIts') is not null
	drop procedure uspfn_SyncPmHstIts
GO
create procedure [dbo].[uspfn_SyncPmHstIts]
as
declare @CompanyCode varchar(max)
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationDtl)

declare @BranchCode varchar(max)
set @BranchCode = (select top 1 BranchCode from gnMstOrganizationDtl where IsBranch = 0)

declare @DealerCode varchar(max)
set @DealerCode  = isnull((select LockingBy from gnMstCoProfileSales where CompanyCode = @CompanyCode and BranchCode = @BranchCode), @CompanyCode)

declare @DealerName varchar(max)
set @DealerName = isnull((select top 1 CompanyName from gnmstorganizationhdr where companycode=@companyCode),
					(select isnull(BranchName,' ') from gnMstOrganizationDtl where companycode=@companycode and branchcode=@dealercode))

declare @LastDate datetime
declare @NoInquiry int

select * into #t2
from (
select pp.CompanyCode, pp.BranchCode, pp.EmployeeID BranchHeadID, ge.EmployeeName BranchHeadName, pt.TeamID
  from pmPosition pp
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pp.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
  left join pmMstTeamMembers pt
	on pp.CompanyCode = pt.CompanyCode
   and pp.BranchCode = pt.BranchCode
   and pp.EmployeeID = pt.EmployeeID
   and pt.IsSupervisor = 1
 where pp.PositionId = '40'
) #t2

select * into #t3
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName,
	   pt.EmployeeID SalesHeadID, ge.EmployeeName SalesHeadName, pd.TeamID
  from #t2 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t3


select * into #t4
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName, 
	   pt.EmployeeID SalesCoordinatorID, ge.EmployeeName SalesCoordinatorName, pd.TeamID
  from #t3 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t4

select * into #t5
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName, 
	   pp.SalesCoordinatorID, pp.SalesCoordinatorName, pt.EmployeeID SalesmanID, ge.EmployeeName SalesmanName
  from #t4 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t5

insert into PmHstITS
select a.CompanyCode
	 , a.BranchCode
	 , a.InquiryNumber
	 , a.InquiryDate
	 , a.OutletID
	 , LEFT(isnull((select Top 1 BranchHeadname from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) BranchHead
	 , LEFT(isnull((select Top 1 SalesHeadName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) SalesHead
	 , LEFT(isnull((select Top 1 SalesCoordinatorName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) SalesCoordinator
	 , LEFT(isnull((select Top 1 SalesmanName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesmanID = a.EmployeeID),''),50) Wiraniaga
	 , a.StatusProspek
	 , a.PerolehanData
	 , LEFT(replace(replace(isnull(a.NamaProspek,' '),CHAR(13), ' '),CHAR(10), ' '),50) NamaProspek
	 , LEFT(replace(replace(replace(isnull(a.AlamatProspek,' '),';',':'),CHAR(13), ' '),CHAR(10), ' '),200) AlamatProspek
	 , LEFT(a.TelpRumah,15) TelpRumah
	 , LEFT(isnull((select TOP 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'CITY' and LookUpValue = a.CityID),''),50) City
	 , LEFT(replace(replace(isnull(a.NamaPerusahaan,' '),CHAR(13), ' '),CHAR(10), ' '),50) NamaPerusahaan
	 , LEFT(replace(replace(replace(isnull(a.AlamatPerusahaan,' '),';',':'),CHAR(13), ' '),CHAR(10), ' '),200) AlamatPerusahaan
	 , a.Jabatan
	 , a.Handphone
	 , a.Faximile
	 , a.Email
	 , a.TipeKendaraan
	 , a.Variant
	 , a.Transmisi
	 , a.ColourCode
	 , LEFT(isnull((select TOP 1 RefferenceDesc1 from omMstRefference where CompanyCode = a.CompanyCode and RefferenceType = 'COLO' and RefferenceCode = a.ColourCode),''),50) ColourDescription
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMBY' and LookUpValue = a.CaraPembayaran),''),30) CaraPembayaran
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMOP' and LookUpValue = a.TestDrive),''),15) TestDrive
	 , a.QuantityInquiry
	 , LEFT(a.LastProgress,15) LastProgress
	 , a.LastUpdateStatus
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('P','PROSPECT')),'19000101') ProspectDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('HP','HOT')),'19000101') HotDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('SPK')),'19000101') SPKDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('DELIVER','DO','DELIVERY','DEAL')),'19000101') DeliveryDate
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'LSNG' and LookUpValue = a.Leasing),''),30) Leasing
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'DWPM' and LookUpValue = a.DownPayment),''),30) DownPayment
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'TENOR' and LookUpValue = a.Tenor),''),30) Tenor
	 , a.LostCaseDate
	 , LEFT(replace(replace(isnull(a.LostCaseCategory,' '),CHAR(13), ' '),CHAR(10), ' '),30) LostCaseCategory
	 , LEFT(a.LostCaseReasonID,30) LostCaseReasonID
	 , LEFT(replace(replace(isnull(a.LostCaseOtherReason,' '),CHAR(13), ' '),CHAR(10), ' '),100) LostCaseOtherReason
	 , LEFT(replace(replace(isnull(a.LostCaseVoiceOfCustomer,' '),CHAR(13), ' '),CHAR(10), ' '),200) LostCaseVoiceOfCustomer
	 , LEFT(a.MerkLain,50) MerkLain
	 , a.CreatedBy
	 , a.CreationDate CreatedDate
	 , a.LastUpdateBy
	 , a.LastUpdateDate
from pmKDP a
where 1 = 1
and not exists (select * from PmHstITS b
				 where  b.CompanyCode = a.CompanyCode
				   and b.BranchCode = a.BranchCOde
				   and b.InquiryNumber = a.InquiryNumber)
				   order by LastUpdateDate

drop table #t2
drop table #t3
drop table #t4
drop table #t5
GO
