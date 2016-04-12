alter table cssettings
alter column SettingLink3 nvarchar(400)
go

IF NOT EXISTS(select top 1 * from CsSettings where SettingCode = 'REMDELIVERY')
BEGIN
	INSERT INTO CsSettings (CompanyCode,SettingCode,SettingDesc,SettingParam1,SettingParam3,SettingLink1,SettingLink2,SettingLink3,CreatedDate,UpdatedDate)
		VALUES ('6006400001' ,'REMDELIVERY', 'REMINDER DELIVERY OUTSTANDING', '2015-10-01', 'CUTOFF', 'DeliveryOutstanding', 'Delivery Outstanding', 'Delivery Outstanding Monitoring', getdate(), getdate())
END
GO

UPDATE CsSettings Set CompanyCode = (SELECT TOP 1 CompanyCode from CsSettings where SettingCode = 'REM3DAYSCALL')
WHERE CompanyCode = '6006400001'
GO

IF not exists(select top 1 * from information_schema.columns where column_name = 'Reason' and table_name = 'CsStnkExt')
begin
	alter table CsStnkExt
	add Reason nvarchar(50)
end
go

IF not exists(select top 1 * from information_schema.columns where column_name = 'StnkDate' and table_name = 'CsCustomerVehicleView')
begin
alter table CsCustomerVehicleView
add StnkDate datetime,
BpkbInDate datetime,
BpkbOutDate datetime
end
go

IF NOT EXISTS(select top 1 * from CsSettings where SettingCode = 'SYNC3DAYSCALL')
BEGIN
	INSERT INTO CsSettings (CompanyCode,SettingCode,SettingDesc,SettingParam1,SettingParam3,CreatedDate,UpdatedDate)
		VALUES ('6006400001' ,'SYNC3DAYSCALL', 'SETTING DELIVERY DATE', '2016-01-01', 'CUTOFF', getdate(), getdate())
END
GO

UPDATE CsSettings Set CompanyCode = (SELECT TOP 1 CompanyCode from CsSettings where SettingCode = 'REM3DAYSCALL')
WHERE CompanyCode = '6006400001'
GO

ALTER procedure [dbo].[uspfn_SyncCsCustomerVehicleView]
as

declare @CurrDate datetime, @Param1 as varchar(20)
declare @t_rem as table
(
	RemCode varchar(20),
	RemDate datetime,
	RemValue int
)

set @CurrDate = getdate()

-- SYNC3DAYSCALL
set @Param1 = isnull((select top 1 SettingParam1 from CsSettings where SettingCode = 'SYNC3DAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('SYNC3DAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

;with x as (
select ROW_NUMBER() OVER (PARTITION BY g.CompanyCode, g.ChassisCode, g.ChassisNo ORDER BY g.LastUpdateDate Desc) as row_num
, g.CompanyCode, g.ChassisCode, g.ChassisNo, g.PoliceRegistrationDate, g.BPKBInDate, h.BPKBOutDate
	 from omTrSalesSPKDetail g
left join omTrSalesSPKSubDetail h
	   on g.CompanyCode = h.CompanyCode
	  and g.BranchCode = h.BranchCode
	  and g.SPKNo = h.SPKNo
	  and g.ChassisCode = h.ChassisCode
	  and g.ChassisNo = h.ChassisNo
) select * into #tSPKdtl from x where row_num = 1

;with r as (
select a.CompanyCode
     , a.ChassisCode
	 , a.ChassisNo
	 , BranchCode = (select top 1 x.BranchCode from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoNo = (select top 1 x.DONo from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoSeq = (select top 1 x.DOSeq from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
  from omTrSalesDODetail a
 where 1 = 1
   and (year(a.LastUpdateDate) = year(getdate()) or (year(a.LastUpdateDate) + 1) = year(getdate()))
 group by a.CompanyCode, a.ChassisCode, a.ChassisNo
),
s as (
select r.CompanyCode
	 , r.BranchCode
	 , c.CustomerCode
	 , r.ChassisCode + convert(varchar, r.ChassisNo) as Chassis
	 , b.EngineCode + convert(varchar, b.EngineNo) as Engine
	 , c.SONo
	 , c.DONo
	 , c.DODate
	 , BpkNo = isnull((select top 1 BPKNo from OmTrSalesBpk
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and DONo = r.DoNo
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
	 , SalesmanCode = isnull((select top 1 Salesman from omTrSalesSO
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
     , b.SalesModelCode
     , b.SalesModelYear
     , b.ColourCode
	 , r.ChassisCode
	 , r.ChassisNo
  from r
  join omTrSalesDODetail b
    on b.CompanyCode = r.CompanyCode
   and b.BranchCode = r.BranchCode
   and b.DONo = r.DoNo
   and b.DOSeq = r.DoSeq
  join omTrSalesDO c
    on c.CompanyCode = b.CompanyCode
   and c.BranchCode = b.BranchCode
   and c.DONo = b.DONo
 where b.StatusBPK != '3'
),
t as (
select s.CompanyCode
     , s.BranchCode 
	 , s.CustomerCode
	 , s.Chassis
	 , s.Engine
	 , s.SONo
	 , s.DoNo
	 , s.DoDate
	 , s.BpkNo
     , s.SalesModelCode as CarType
     , s.ColourCode as Color
	 , s.SalesmanCode
	 , b.EmployeeName as SalesmanName
	 , c.PoliceRegNo
	 --, (CASE WHEN (CONVERT(varchar, d.LockingDate, 112) = '19000101' OR d.LockingDate IS NULL OR d.LockingDate < d.BPKDate) THEN '' ELSE d.LockingDate END) as DeliveryDate
	 , (
		CASE 
			WHEN ISNULL(CONVERT(varchar, d.LockingDate, 112), '19000101') = '19000101' 
			  AND d.BpkDate < (SELECT SettingParam1 FROM CsSettings WHERE SettingCode = 'SYNC3DAYSCALL') 
			THEN
				d.BpkDate
			ELSE
				d.LockingDate
		END
	   ) as DeliveryDate
     , s.SalesModelCode
     , s.SalesModelYear
     , s.ColourCode
	 , d.BpkDate
	 , e.isLeasing
	 , e.LeasingCo
	 , f.CustomerName as LeasingName
	 , e.Installment
	 , StnkDate = case when convert(varchar, g.PoliceRegistrationDate, 112) = '19000101' then null else g.PoliceRegistrationDate end
	 , BPKBInDate = case when convert(varchar, g.BPKBInDate, 112) = '19000101' then null else g.BPKBInDate end
	 , BPKBOutDate = case when convert(varchar, g.BPKBOutDate, 112) = '19000101' then null else g.BPKBOutDate end
	 , LastUpdatedDate = getdate()
  from s with (nolock, nowait)
  left join HrEmployee b
    on b.CompanyCode = s.CompanyCode
   and b.EmployeeID = s.SalesmanCode
  left join svMstCustomerVehicle c
    on c.CompanyCode = s.CompanyCode
   and c.ChassisCode = s.ChassisCode
   and c.ChassisNo = s.ChassisNo
  join omTrSalesBPK d
    on d.CompanyCode = s.CompanyCode
   and d.BranchCode = s.BranchCode
   and d.BpkNo = s.BpkNo
  join omTrSalesSO e
    on e.CompanyCode = s.CompanyCode
   and e.BranchCode = s.BranchCode
   and e.SONo = s.SONo
  left join gnMstCustomer f
    on f.CompanyCode = e.CompanyCode
   and f.CustomerCode = e.LeasingCo
  left join #tSPKdtl g
    on g.CompanyCode = s.CompanyCode
--   and g.BranchCode = s.BranchCode
   and g.ChassisCode = s.ChassisCode
   and g.ChassisNo = s.ChassisNo
  -- and g.SPKNo = 
 where isnull(d.Status, 3) != '3'
)
select * into #t1 from (select * from t)#

delete CsCustomerVehicleView
 where exists (
	select top 1 1 from #t1
	 where #t1.CompanyCode = CsCustomerVehicleView.CompanyCode
	   and #t1.BranchCode = CsCustomerVehicleView.BranchCode
	   and #t1.Chassis = CsCustomerVehicleView.Chassis
 )
insert into CsCustomerVehicleView (CompanyCode, BranchCode, CustomerCode, Chassis, Engine, SONo, DONo, DoDate, BpkNo, CarType, Color, SalesmanCode, SalesmanName, PoliceRegNo, DeliveryDate, SalesModelCode, SalesModelYear, ColourCode, BpkDate, IsLeasing, LeasingCo, LeasingName, Installment, StnkDate, BPKBInDate, BPKBOutDate,LastUpdatedDate)
select * from #t1


declare @ccode varchar(20), @bday datetime, @companyCode varchar(20)

declare UpdateBirthDay cursor for
select a.CustomerCode, b.skpkbirthday, b.CompanyCode from #t1 a
inner join omTrSalesReqDetail b on (a.bpkno=b.bpkno and a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.sono=b.sono)

OPEN UpdateBirthDay
FETCH NEXT FROM UpdateBirthDay
INTO @ccode, @bday, @companyCode

WHILE @@FETCH_STATUS = 0
BEGIN

	update gnMstCustomer
	set BirthDate=@bday,
		LastUpdateDate=getdate()
	where CompanyCode=@companyCode and CustomerCode=@ccode

	update CsCustomerView
	set BirthDate=@bday
	where CompanyCode=@companyCode and CustomerCode=@ccode and BirthDate <> @bday

	FETCH NEXT FROM UpdateBirthDay
	INTO @ccode, @bday, @companyCode

END

CLOSE UpdateBirthDay
DEALLOCATE UpdateBirthDay

drop table #t1

GO