Create procedure [dbo].[uspfn_OmAccsSoList]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@ReturnNo varchar(15)
	,@UserID varchar(30)
as

--declare @CompanyCode as varchar(15)
--,@BranchCode as varchar(15)
--,@ReturnNo as varchar(15)
--,@UserID as varchar(30)

--select @CompanyCode='6006406'
--,@BranchCode='6006410'
--,@ReturnNo='RTS/12/000001'
--,@UserID='amk'

-- get data from Return Vin


DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25),
	@BranchMD AS varchar(25),
	@Otom AS Varchar(5)

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
set @BranchMD = (SELECT TOP 1 UnitBranchMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @Otom = (SELECT ParaValue from gnMstLookUpDtl where CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode=@CompanyCode)
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)


if @Otom = '0' 
begin
		select * into #t1
	from ( 
		select a.*,b.ReturnDate,b.WarehouseCode
		from omTrSalesReturnVin a 
			inner join omTrSalesReturn b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode 
			and b.ReturnNo = a.ReturnNo
		where a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.ReturnNo = @ReturnNo
	) #t1

	-- grouping data from #t1
	select * into #t2
	from (
		select a.CompanyCode,a.BranchCode,Year(a.ReturnDate) [Year],Month(a.ReturnDate) [Month]
			,Year(dateadd(m,-1,a.ReturnDate)) YearBefore, Month(dateadd(m,-1,a.ReturnDate)) MonthBefore
			,a.SalesModelCode,a.SalesModelYear,a.ColourCode,a.WarehouseCode,count(a.SalesModelCode) QtyIn
		from #t1 a
		group by a.CompanyCode,a.BranchCode,Year(a.ReturnDate),Month(a.ReturnDate)
			,Year(dateadd(m,-1,a.ReturnDate)), Month(dateadd(m,-1,a.ReturnDate))
			,a.SalesModelCode,a.SalesModelYear,a.ColourCode,a.WarehouseCode
	) #t2

	-- update omTrInventQtyVehicle when data is already inserted
	update omTrInventQtyVehicle
	set QtyIn=a.QtyIn+b.QtyIn
		,EndingOH = BeginningOH + (a.QtyIn + b.QtyIn) - QtyOut
		,EndingAV = BeginningAV + (a.QtyIn + b.QtyIn) - Alocation - QtyOut
		,LastUpdateBy= @UserID
		,LastUpdateDate= getDate()
	from omTrInventQtyVehicle a
		inner join #t2 b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.[Year]=b.[Year] 
			and a.[Month]=b.[Month] and a.SalesModelCode=b.SalesModelCode and a.SalesModelYear=b.SalesModelYear 
			and a.ColourCode=b.ColourCode and a.WarehouseCode=b.WarehouseCode

	-- insert omTrInventQtyVehicle when data is inserted yet
	if exists 
	(
		select 1 from #t2 a
		where not exists (
			select 1 from omTrInventQtyVehicle
			where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and [Year]=a.[Year] 
				and [Month]=a.[Month] and SalesModelCode=a.SalesModelCode and SalesModelYear=a.SalesModelYear 
				and ColourCode=a.ColourCode and WarehouseCode=a.WarehouseCode
		)
	)
	begin 
		insert omTrInventQtyVehicle (CompanyCode, BranchCode, Year, Month, SalesModelCode, SalesModelYear, ColourCode, WarehouseCode, QtyIn, Alocation, QtyOut, BeginningOH, EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate)
		select a.CompanyCode,isnull(b.BranchCode,a.BranchCode) BranchCode,a.[Year],a.[Month],a.SalesModelCode
			,a.SalesModelYear,a.ColourCode,a.WarehouseCode
			,(isnull(b.QtyIn,0) + a.QtyIn) QtyIn
			,0 Alocation, 0 QtyOut
			,isnull(b.BeginningOH,0) BeginningOH
			,(isnull(b.BeginningOH,0) + (isnull(b.QtyIn,0) + a.QtyIn)) - 0 EndingOH
			, isnull(b.BeginningAV,0) BeginningAV
			,(isnull(b.BeginningAV,0) + (isnull(b.QtyIn,0) + a.QtyIn)) - 0 - 0 EndingAV
			,'' Remark, '0' Status, @UserID CreatedBy, getDate() CreatedDate, @UserID LastUpdateBy, getDate() LastUpdateDate
			,0 IsLocked, '' LockedBy, getDate() LockedDate
		from #t2 a
			left join omTrInventQtyVehicle b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode 
				and a.[YearBefore]=b.[Year] and a.[MonthBefore]=b.[Month] and a.SalesModelCode=b.SalesModelCode 
				and a.SalesModelYear=b.SalesModelYear and a.ColourCode=b.ColourCode and a.WarehouseCode=b.WarehouseCode
		where not exists (
				select 1 from omTrInventQtyVehicle
				where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and [Year]=a.[Year] 
					and [Month]=a.[Month] and SalesModelCode=a.SalesModelCode and SalesModelYear=a.SalesModelYear 
					and ColourCode=a.ColourCode and WarehouseCode=a.WarehouseCode
			)
	end

	-- update omMstVehicle Status
	update omMstVehicle
	set WarehouseCode=b.WarehouseCode,SOReturnNo=b.ReturnNo,Status='0'
		,LastUpdateBy=@UserID,LastUpdateDate= getDate()
	from omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
			and a.ChassisNo=b.ChassisNo

	-- insert omMstVehicleHistory
	insert into omMstVehicleHistory (CompanyCode, ChassisCode, ChassisNo, SeqNo, EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, ServiceBookNo, KeyNo, COGSUnit, COGSOthers, COGSKaroseri, PpnBmBuyPaid, PpnBmBuy, SalesNetAmt, PpnBmSellPaid, PpnBmSell, PONo, POReturnNo, BPUNo, HPPNo, KaroseriSPKNo, KaroseriTerimaNo, SONo, SOReturnNo, DONo, BPKNo, InvoiceNo, ReqOutNo, TransferOutNo, TransferInNo, WarehouseCode, Remark, Status, IsAlreadyPDI, BPKDate, FakturPolisiNo, FakturPolisiDate, PoliceRegistrationNo, PoliceRegistrationDate, IsProfitCenterSales, IsProfitCenterService, IsActive, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate, IsNonRegister, BPUDate, SuzukiDONo, SuzukiDODate, SuzukiSJNo, SuzukiSJDate)
	select a.CompanyCode,a.ChassisCode,a.ChassisNo
		,(select isnull(max(SeqNo),0)+1 from omMstVehicleHistory where ChassisCode=a.ChassisCode and ChassisNo=a.ChassisNo) SeqNo
		,a.EngineCode,a.EngineNo,a.SalesModelCode,a.SalesModelYear,a.ColourCode
		,a.ServiceBookNo,a.KeyNo,a.COGSUnit,a.COGSOthers,a.COGSKaroseri,a.PpnBmBuyPaid,a.PpnBmBuy,a.SalesNetAmt,a.PpnBmSellPaid,a.PpnBmSell
		,a.PONo,a.POReturnNo,a.BPUNo,a.HPPNo,a.KaroseriSPKNo,a.KaroseriTerimaNo,a.SONo,a.SOReturnNo,a.DONo,a.BPKNo,a.InvoiceNo,a.ReqOutNo,a.TransferOutNo
		,a.TransferInNo,a.WarehouseCode,'SALES RETURN' Remark,a.Status,a.IsAlreadyPDI,a.BPKDate,a.FakturPolisiNo,a.FakturPolisiDate,a.PoliceRegistrationNo
		,a.PoliceRegistrationDate,a.IsProfitCenterSales,a.IsProfitCenterService,a.IsActive,a.CreatedBy,a.CreatedDate,a.LastUpdateBy,a.LastUpdateDate
		,a.IsLocked,a.LockedBy,a.LockedDate,a.IsNonRegister,a.BPUDate,a.SuzukiDONo, a.SuzukiDODate, a.SuzukiSJNo, a.SuzukiSJDate
	from omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.SOReturnNo=b.ReturnNo and a.ChassisCode=b.ChassisCode 
			and a.ChassisNo=b.ChassisNo

	-- update omMstVehicle Document
	update omMstVehicle
	set SONo='', SOreturnNo='', DONo='', BPKNo='', InvoiceNo=''
		,LastUpdateBy=@UserID,LastUpdateDate= getDate()
	from omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
			and a.ChassisNo=b.ChassisNo

	select * into #t3 from (
		select b.InvoiceNo, a.ChassisCode, a.ChassisNo 
		from omTrSalesReturnVin a 
			inner join omTrSalesReturn b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode 
			and b.ReturnNo = a.ReturnNo
		where a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.ReturnNo = @ReturnNo
	)#t3

	-- update omHstInquirySales
	update omHstInquirySales
	set Status = 0, DCSStatus = 0, DCSDate = null, LastUpdateBy=@UserID, LastUpdateDate = getdate()
	where CompanyCode = @CompanyCode
		and BranchCode = @BranchCode
		and InvoiceNo + ChassisCode + convert(varchar,ChassisNo) in (select InvoiceNo + ChassisCode + convert(varchar,ChassisNo) from #t3) 

	-- update Sales Return header
	update omTrSalesReturn
	set Status='2'
	where CompanyCode= @CompanyCode and BranchCode=@BranchCode and ReturnNo= @ReturnNo

	drop table #t1
	drop table #t2
	drop table #t3
end
else
begin
	set @QRYTmp = 'select * into #t1
	from ( 
		select a.*,b.ReturnDate,b.WarehouseCode
		from omTrSalesReturnVin a 
			inner join omTrSalesReturn b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode 
			and b.ReturnNo = a.ReturnNo
		where a.CompanyCode = ''' + @CompanyCode + '''
			AND a.BranchCode = ''' + @BranchCode + '''
			AND a.ReturnNo = ''' + @ReturnNo + '''
	) #t1

	-- grouping data from #t1
	select * into #t2
	from (
		select a.CompanyCode,a.BranchCode,Year(a.ReturnDate) [Year],Month(a.ReturnDate) [Month]
			,Year(dateadd(m,-1,a.ReturnDate)) YearBefore, Month(dateadd(m,-1,a.ReturnDate)) MonthBefore
			,a.SalesModelCode,a.SalesModelYear,a.ColourCode,a.WarehouseCode,count(a.SalesModelCode) QtyIn
		from #t1 a
		group by a.CompanyCode,a.BranchCode,Year(a.ReturnDate),Month(a.ReturnDate)
			,Year(dateadd(m,-1,a.ReturnDate)), Month(dateadd(m,-1,a.ReturnDate))
			,a.SalesModelCode,a.SalesModelYear,a.ColourCode,a.WarehouseCode
	) #t2

	-- update omTrInventQtyVehicle when data is already inserted
	update ' + @DBMD + '.dbo.omTrInventQtyVehicle
	set QtyIn=a.QtyIn+b.QtyIn
		,EndingOH = BeginningOH + (a.QtyIn + b.QtyIn) - QtyOut
		,EndingAV = BeginningAV + (a.QtyIn + b.QtyIn) - Alocation - QtyOut
		,LastUpdateBy= ''' + @UserID + '''
		,LastUpdateDate= getDate()
	from omTrInventQtyVehicle a
		inner join #t2 b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.[Year]=b.[Year] 
			and a.[Month]=b.[Month] and a.SalesModelCode=b.SalesModelCode and a.SalesModelYear=b.SalesModelYear 
			and a.ColourCode=b.ColourCode and a.WarehouseCode=b.WarehouseCode

	-- insert omTrInventQtyVehicle when data is inserted yet
	if exists 
	(
		select 1 from #t2 a
		where not exists (
			select 1 from omTrInventQtyVehicle
			where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and [Year]=a.[Year] 
				and [Month]=a.[Month] and SalesModelCode=a.SalesModelCode and SalesModelYear=a.SalesModelYear 
				and ColourCode=a.ColourCode and WarehouseCode=a.WarehouseCode
		)
	)
	begin 
		insert ' + @DBMD + '.dbo.omTrInventQtyVehicle (CompanyCode, BranchCode, Year, Month, SalesModelCode, SalesModelYear, ColourCode, WarehouseCode, QtyIn, Alocation, QtyOut, BeginningOH, EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate)
		select a.CompanyCode,isnull(b.BranchCode,a.BranchCode) BranchCode,a.[Year],a.[Month],a.SalesModelCode
			,a.SalesModelYear,a.ColourCode,a.WarehouseCode
			,(isnull(b.QtyIn,0) + a.QtyIn) QtyIn
			,0 Alocation, 0 QtyOut
			,isnull(b.BeginningOH,0) BeginningOH
			,(isnull(b.BeginningOH,0) + (isnull(b.QtyIn,0) + a.QtyIn)) - 0 EndingOH
			, isnull(b.BeginningAV,0) BeginningAV
			,(isnull(b.BeginningAV,0) + (isnull(b.QtyIn,0) + a.QtyIn)) - 0 - 0 EndingAV
			,'''' Remark, ''0'' Status, ''' + @UserID + ''' CreatedBy, getDate() CreatedDate, ''' + @UserID + ''' LastUpdateBy, getDate() LastUpdateDate
			,0 IsLocked, '''' LockedBy, getDate() LockedDate
		from #t2 a
			left join ' + @DBMD + '.dbo.omTrInventQtyVehicle b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode 
				and a.[YearBefore]=b.[Year] and a.[MonthBefore]=b.[Month] and a.SalesModelCode=b.SalesModelCode 
				and a.SalesModelYear=b.SalesModelYear and a.ColourCode=b.ColourCode and a.WarehouseCode=b.WarehouseCode
		where not exists (
				select 1 from ' + @DBMD + '.dbo.omTrInventQtyVehicle
				where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and [Year]=a.[Year] 
					and [Month]=a.[Month] and SalesModelCode=a.SalesModelCode and SalesModelYear=a.SalesModelYear 
					and ColourCode=a.ColourCode and WarehouseCode=a.WarehouseCode
			)
	end

	-- update omMstVehicle Status
	update ' + @DBMD + '.dbo.omMstVehicle
	set WarehouseCode=b.WarehouseCode,SOReturnNo=b.ReturnNo,Status=''0''
		,LastUpdateBy=''' + @UserID + ''',LastUpdateDate= getDate()
	from ' + @DBMD + '.dbo.omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
			and a.ChassisNo=b.ChassisNo

	-- insert omMstVehicleHistory
	insert into omMstVehicleHistory (CompanyCode, ChassisCode, ChassisNo, SeqNo, EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, ServiceBookNo, KeyNo, COGSUnit, COGSOthers, COGSKaroseri, PpnBmBuyPaid, PpnBmBuy, SalesNetAmt, PpnBmSellPaid, PpnBmSell, PONo, POReturnNo, BPUNo, HPPNo, KaroseriSPKNo, KaroseriTerimaNo, SONo, SOReturnNo, DONo, BPKNo, InvoiceNo, ReqOutNo, TransferOutNo, TransferInNo, WarehouseCode, Remark, Status, IsAlreadyPDI, BPKDate, FakturPolisiNo, FakturPolisiDate, PoliceRegistrationNo, PoliceRegistrationDate, IsProfitCenterSales, IsProfitCenterService, IsActive, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate, IsNonRegister, BPUDate, SuzukiDONo, SuzukiDODate, SuzukiSJNo, SuzukiSJDate)
	select a.CompanyCode,a.ChassisCode,a.ChassisNo
		,(select isnull(max(SeqNo),0)+1 from omMstVehicleHistory where ChassisCode=a.ChassisCode and ChassisNo=a.ChassisNo) SeqNo
		,a.EngineCode,a.EngineNo,a.SalesModelCode,a.SalesModelYear,a.ColourCode
		,a.ServiceBookNo,a.KeyNo,a.COGSUnit,a.COGSOthers,a.COGSKaroseri,a.PpnBmBuyPaid,a.PpnBmBuy,a.SalesNetAmt,a.PpnBmSellPaid,a.PpnBmSell
		,a.PONo,a.POReturnNo,a.BPUNo,a.HPPNo,a.KaroseriSPKNo,a.KaroseriTerimaNo,a.SONo,a.SOReturnNo,a.DONo,a.BPKNo,a.InvoiceNo,a.ReqOutNo,a.TransferOutNo
		,a.TransferInNo,a.WarehouseCode,'SALES RETURN' Remark,a.Status,a.IsAlreadyPDI,a.BPKDate,a.FakturPolisiNo,a.FakturPolisiDate,a.PoliceRegistrationNo
		,a.PoliceRegistrationDate,a.IsProfitCenterSales,a.IsProfitCenterService,a.IsActive,a.CreatedBy,a.CreatedDate,a.LastUpdateBy,a.LastUpdateDate
		,a.IsLocked,a.LockedBy,a.LockedDate,a.IsNonRegister,a.BPUDate,a.SuzukiDONo, a.SuzukiDODate, a.SuzukiSJNo, a.SuzukiSJDate
	from omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.SOReturnNo=b.ReturnNo and a.ChassisCode=b.ChassisCode 
			and a.ChassisNo=b.ChassisNo

	-- update omMstVehicle Document
	update omMstVehicle
	set SONo='', SOreturnNo='', DONo='', BPKNo='', InvoiceNo=''
		,LastUpdateBy=@UserID,LastUpdateDate= getDate()
	from omMstVehicle a
		inner join #t1 b on a.CompanyCode=b.CompanyCode and a.ChassisCode=b.ChassisCode
			and a.ChassisNo=b.ChassisNo

	select * into #t3 from (
		select b.InvoiceNo, a.ChassisCode, a.ChassisNo 
		from omTrSalesReturnVin a 
			inner join omTrSalesReturn b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode 
			and b.ReturnNo = a.ReturnNo
		where a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.ReturnNo = @ReturnNo
	)#t3

	-- update omHstInquirySales
	update omHstInquirySales
	set Status = 0, DCSStatus = 0, DCSDate = null, LastUpdateBy=@UserID, LastUpdateDate = getdate()
	where CompanyCode = @CompanyCode
		and BranchCode = @BranchCode
		and InvoiceNo + ChassisCode + convert(varchar,ChassisNo) in (select InvoiceNo + ChassisCode + convert(varchar,ChassisNo) from #t3) 

	-- update Sales Return header
	update omTrSalesReturn
	set Status='2'
	where CompanyCode= @CompanyCode and BranchCode=@BranchCode and ReturnNo= @ReturnNo

	drop table #t1
	drop table #t2
	drop table #t3
end
