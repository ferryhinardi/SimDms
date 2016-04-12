if object_id('usprpt_QueryCustomerDealer2') is not null
	drop procedure usprpt_QueryCustomerDealer2
GO

-- INQUIRY CUSTOMER
-- ----------------
-- Revision by   : HTO
-- Revision date : 05 January 2015
--
-- usprpt_QueryCustomerDealer2 '0','2014-11-01','2014-11-30','CABANG','6006400001',''
CREATE procedure [dbo].[usprpt_QueryCustomerDealer2]
	@CheckDate	bit,
	@StartDate	Datetime,
	@EndDate	Datetime,
	@Area		varchar(15),
	@Dealer		varchar(15),
	@Outlet		varchar(15)
AS
BEGIN
  --Check status in table sysParameter, then insert/update Customer Status
	declare @CustomerStatus	integer
	declare @CustomerDate	varchar(10)
	set @CustomerStatus = 0
	set @CustomerDate   = (select ParamValue from sysParaMeter where ParamId='CUSTOMER_STATUS')
	set @CustomerStatus = (case when @CustomerDate is null                         then 0
	                            when @CustomerDate<>convert(varchar,getdate(),111) then 1
					    	    else                                                    2
						   end)
	if @CustomerStatus = 0
		insert sysParaMeter values('CUSTOMER_STATUS',convert(varchar,getdate(),111),'Customer Status used for Inquiry Customer')
	else
		if @CustomerStatus = 1
			update sysParaMeter set ParamValue=convert(varchar,getdate(),111) where ParamId='CUSTOMER_STATUS'

	if @CustomerStatus in (0,1) 
		begin
		  --Insert data to table gnMstCustomerDealer & gnMstCustomerDealerProfitCenter
			declare @ProductType nvarchar(2)
			set @ProductType = (select top 1 ProductType from GnMstCoProfile)
			insert into GnMstCustomerDealer
				select CompanyCode, CustomerCode
					 , case when @ProductType = '4W' then StandardCode else '' end
					 , case when @ProductType = '2W' then StandardCode else '' end
					 , CustomerName, CustomerGovName, Address1, Address2, Address3, Address4, ProvinceCode
					 , (select top 1 LookUpValueName from GnMstLookUpDtl where CodeID = 'PROV' and ParaValue = ProvinceCode)
					 , CityCode
				     , (select top 1 LookUpValueName from GnMstLookUpDtl where CodeID = 'CITY' and ParaValue = CityCode)
					 , ZipNo, KelurahanDesa, KecamatanDistrik, KotaKabupaten, IbuKota, PhoneNo, HPNo, FaxNo
					 , OfficePhoneNo, EMail, Gender, BirthDate, isPKP, NPWPNo, NPWPDate, SKPNo, SKPDate, CustomerType
					 , (select top 1 LookUpValueName from GnMstLookUpDtl where CodeID = 'CSTP' and ParaValue = CustomerType)
					 , CategoryCode
					 , (select top 1 LookUpValueName from GnMstLookUpDtl where CodeID = 'CSCT' and ParaValue = CategoryCode)
					 , Status, '', 0, null, CreatedBy, CreatedDate, LastUpdateBy, getdate() --LastUpdateDate
			      from GnMstCustomer 
				 where not exists (select top 1 CustomerCode from GnMstCustomerDealer a
				                    where a.CompanyCode  = GnMstCustomer.CompanyCode 
									  and a.CustomerCode = GnMstCustomer.CustomerCode)

		  --Update Customer Status 
			update gnMstCustomer set CustomerStatus='D', LastUpdateDate=getdate() 
			 where isnull(CustomerStatus,'')=''
			update gnMstCustomer set CustomerStatus='C', LastUpdateDate=getdate()
			 where exists (select 1 from svTrnInvoice     where CustomerCode=gnMstCustomer.CustomerCode) and CustomerStatus='D'
			update gnMstCustomer set CustomerStatus='B', LastUpdateDate=getdate()
			 where exists (select 1 from omTrSalesInvoice where CustomerCode=gnMstCustomer.CustomerCode) and CustomerStatus='D'
			update gnMstCustomer set CustomerStatus='A', LastUpdateDate=getdate()
			 where exists (select 1 from omTrSalesInvoice where CustomerCode=gnMstCustomer.CustomerCode) and CustomerStatus='C'

			update gnMstCustomerDealer set CustomerStatus='D', LastUpdateDate=getdate(), DCSFlag=0 
			 where isnull(CustomerStatus,'')=''
			update gnMstCustomerDealer set CustomerStatus='C', LastUpdateDate=getdate(), DCSFlag=0 
			 where exists (select 1 from svTrnInvoice     where CustomerCode=gnMstCustomerDealer.CustomerCode) and CustomerStatus='D'
			update gnMstCustomerDealer set CustomerStatus='B', LastUpdateDate=getdate(), DCSFlag=0
			 where exists (select 1 from omTrSalesInvoice where CustomerCode=gnMstCustomerDealer.CustomerCode) and CustomerStatus='D'
			update gnMstCustomerDealer set CustomerStatus='A', LastUpdateDate=getdate(), DCSFlag=0
			 where exists (select 1 from omTrSalesInvoice where CustomerCode=gnMstCustomerDealer.CustomerCode) and CustomerStatus='C'
		end

  --select all customer including their vehicle history
	declare @SysDate	datetime
	set @SysDate = getdate()
    if not exists (select * from sys.objects where object_id = object_id(N'[gnInquirySalesTemp]') and type=N'U')
	begin
	    create table [gnInquirySalesTemp]
		(
	      --[RowID]					[uniqueidentifier],
			[SysDate]				[datetime]		NOT NULL,
			[CompanyCode]			[varchar](15)	NOT NULL,
			[CustomerCode]			[varchar](15)	NOT NULL,
			[CustomerName]			[varchar](100)	NULL,
			[CustomerGovName]		[varchar](100)	NULL,
			[Address]				[varchar](500)	NULL,
			[ProvinceCode]			[varchar](15)	NULL,
			[ProvinceName]			[varchar](100)	NULL,
			[CityCode]				[varchar](15)	NULL,
			[CityName]				[varchar](100)	NULL,
			[ZipNo]					[varchar](15)	NULL,
			[KelurahanDesa]			[varchar](100)	NULL, 
			[KecamatanDistrik]		[varchar](100)	NULL,
			[KotaKabupaten]			[varchar](100)	NULL,
			[IbuKota]				[varchar](100)	NULL,
			[PhoneNo]				[varchar](25)	NULL, 
			[HPNo]					[varchar](25)	NULL, 
			[FaxNo]					[varchar](25)	NULL,
			[OfficePhoneNo]			[varchar](25)	NULL, 
			[Email]					[varchar](100)	NULL, 
			[Gender]				[varchar](25)	NULL, 
			[BirthDate]				[datetime]		NULL,
			[isPKP]					[bit]			NULL,
			[NPWPNo]				[varchar](25)	NULL, 
			[NPWPDate]				[datetime]		NULL, 
			[SKPNo]					[varchar](25)	NULL, 
			[SKPDate]				[datetime]		NULL, 
			[CustomerType]			[varchar](15)	NULL, 
			[CustomerTypeDesc]		[varchar](25)	NULL,
			[Status]				[bit]			NULL, 
			[CustomerStatus]		[varchar](15)	NULL,
			[Salesman]				[varchar](100)	NULL,
			[SC]					[varchar](100)	NULL,
			[SH]					[varchar](100)	NULL,
			[BM]					[varchar](100)	NULL,
			[CommercialYearFirst]	[varchar](4)	NULL,
			[CommercialTypeFirst]	[varchar](25)	NULL,
			[CommercialYearLast]	[varchar](4)	NULL,
			[CommercialTypeLast]	[varchar](25)	NULL,
			[PassengerYearFirst]	[varchar](4)	NULL,
			[PassengerTypeFirst]	[varchar](25)	NULL,
			[PassengerYearLast]		[varchar](4)	NULL,
			[PassengerTypeLast]		[varchar](25)	NULL,
			[LastUpdateDate]		[datetime]		NULL,
		CONSTRAINT [PK_gnInquirySalesTemp] PRIMARY KEY CLUSTERED 
			(
				[SysDate]		ASC,
				[CompanyCode]	ASC,
				[CustomerCode]	ASC
			) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [idx_gnInquirySalesTemp_CustomerCode] ON [dbo].[gnInquirySalesTemp]
		(
			[SysDate]		ASC,
			[CompanyCode]	ASC,
			[CustomerCode]	ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [idx_gnInquirySalesTemp_Salesman] ON [dbo].[gnInquirySalesTemp]
		(
			[SysDate]		ASC,
			[CompanyCode]	ASC,
			[Salesman]		ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [idx_gnInquirySalesTemp_SC] ON [dbo].[gnInquirySalesTemp]
		(
			[SysDate]		ASC,
			[CompanyCode]	ASC,
			[SC]			ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [idx_gnInquirySalesTemp_SH] ON [dbo].[gnInquirySalesTemp]
		(
			[SysDate]		ASC,
			[CompanyCode]	ASC,
			[SH]			ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [idx_gnInquirySalesTemp_BM] ON [dbo].[gnInquirySalesTemp]
		(
			[SysDate]		ASC,
			[CompanyCode]	ASC,
			[BM]			ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	end

	if @Outlet <> ''
	BEGIN
			insert into gnInquirySalesTemp
			select @SysDate, a.CompanyCode, a.CustomerCode, a.CustomerName, a.CustomerGovName
				 , a.Address1 + a.Address2 + a.Address3 + a.Address4 Address, a.ProvinceCode, a.ProvinceName
				 , a.CityCode, a.CityName, a.ZipNo, a.KelurahanDesa, a.KecamatanDistrik, a.KotaKabupaten
				 , a.IbuKota, a.PhoneNo, a.HPNo, a.FaxNo, a.OfficePhoneNo, a.Email, a.Gender, a.BirthDate
				 , a.isPKP, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate, a.CustomerType, a.CustomerTypeDesc
				 , a.Status, a.CustomerStatus, Salesman = NULL, SC = NULL, SH = NULL, BM = NULL
				 , CommercialYearFirst=NULL, CommercialTypeFirst=NULL, CommercialYearLast=NULL, CommercialTypeLast=NULL
				 , PassengerYearFirst=NULL, PassengerTypeFirst=NULL, PassengerYearLast=NULL, PassengerTypeLast=NULL
				 , a.LastUpdateDate
			  from gnMstCustomerDealer a
			  left join GnMstDealerMapping b 
			    on a.CompanyCode = b.DealerCode
				left join gnMstDealerOutletMapping o
				on o.dealercode= a.CompanyCode  and o.OutletCode = @Outlet

			 where a.Status=1
			   and case when @CheckDate = 1 
							 then convert(varchar,a.CreatedDate,112) 
					    else      convert(varchar,@StartDate,112) 
				   end 
				   between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
	  		   and (isnull(b.Area,'') like case when isnull(@Area,'') <> '' 
										        then case when (isnull(@Area,'')='JABODETABEK' or isnull(@Area,'')='CABANG')
														  then 'JABODETABEK'
														  else isnull(@Area,'') 
														  end
											    else '%%' 
												end
				or  isnull(b.Area,'') like case when isnull(@Area,'') <> '' 
												then case when (isnull(@Area,'')='JABODETABEK' or isnull(@Area,'')='CABANG')
														  then 'CABANG'
														  else isnull(@Area,'') 
														  end
												else '%%' 
												end) 	
			   and isnull(b.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer	else '%%' end

			   and isnull(o.OutletCode,'') like  case when isnull(@Outlet,'') <> '' then @Outlet	else '%%' end

			   			   
	delete gnInquirySalesTemp where not exists(select 1 from omTrSalesInvoice where CompanyCode=gnInquirySalesTemp.CompanyCode and BranchCode=@Outlet and CustomerCode=gnInquirySalesTemp.CustomerCode) and 
	not exists(select 1 from svTrnService where CompanyCode=gnInquirySalesTemp.CompanyCode and BranchCode=@Outlet and CustomerCode=gnInquirySalesTemp.CustomerCode)  
	and not exists(select 1 from sptrnsfpjhdr where CompanyCode=gnInquirySalesTemp.CompanyCode and BranchCode=@Outlet and CustomerCode=gnInquirySalesTemp.CustomerCode)  


	END
	ELSE
	BEGIN
	insert into gnInquirySalesTemp
			select @SysDate, a.CompanyCode, a.CustomerCode, a.CustomerName, a.CustomerGovName
				 , a.Address1 + a.Address2 + a.Address3 + a.Address4 Address, a.ProvinceCode, a.ProvinceName
				 , a.CityCode, a.CityName, a.ZipNo, a.KelurahanDesa, a.KecamatanDistrik, a.KotaKabupaten
				 , a.IbuKota, a.PhoneNo, a.HPNo, a.FaxNo, a.OfficePhoneNo, a.Email, a.Gender, a.BirthDate
				 , a.isPKP, a.NPWPNo, a.NPWPDate, a.SKPNo, a.SKPDate, a.CustomerType, a.CustomerTypeDesc
				 , a.Status, a.CustomerStatus, Salesman = NULL, SC = NULL, SH = NULL, BM = NULL
				 , CommercialYearFirst=NULL, CommercialTypeFirst=NULL, CommercialYearLast=NULL, CommercialTypeLast=NULL
				 , PassengerYearFirst=NULL, PassengerTypeFirst=NULL, PassengerYearLast=NULL, PassengerTypeLast=NULL
				 , a.LastUpdateDate
			  from gnMstCustomerDealer a
			  left join GnMstDealerMapping b 
			    on a.CompanyCode = b.DealerCode
			 where a.Status=1
			   and case when @CheckDate = 1 
							 then convert(varchar,a.CreatedDate,112) 
					    else      convert(varchar,@StartDate,112) 
				   end 
				   between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
	  		   and (isnull(b.Area,'') like case when isnull(@Area,'') <> '' 
										        then case when (isnull(@Area,'')='JABODETABEK' or isnull(@Area,'')='CABANG')
														  then 'JABODETABEK'
														  else isnull(@Area,'') 
														  end
											    else '%%' 
												end
				or  isnull(b.Area,'') like case when isnull(@Area,'') <> '' 
												then case when (isnull(@Area,'')='JABODETABEK' or isnull(@Area,'')='CABANG')
														  then 'CABANG'
														  else isnull(@Area,'') 
														  end
												else '%%' 
												end) 	
			   and isnull(b.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer	else '%%' end
	END

	update gnInquirySalesTemp				 
		set CommercialYearFirst = isnull((select top 1 v.SalesModelYear from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate asc),
                                                (select top 1 v.ProductionYear from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel like '%ANG' or v.BasicModel like '%CH' or v.BasicModel like '%PU' or v.BasicModel like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate asc))
				 , CommercialTypeFirst = isnull((select top 1 v.SalesModelCode from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate asc),
                                                (select top 1 v.BasicModel from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel like '%ANG' or v.BasicModel like '%CH' or v.BasicModel like '%PU' or v.BasicModel like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate asc))
				 , CommercialYearLast  = isnull((select top 1 v.SalesModelYear from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate desc),
                                                (select top 1 v.ProductionYear from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel like '%ANG' or v.BasicModel like '%CH' or v.BasicModel like '%PU' or v.BasicModel like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate desc))
				 , CommercialTypeLast  = isnull((select top 1 v.SalesModelCode from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate desc),
                                                (select top 1 v.BasicModel from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel like '%ANG' or v.BasicModel like '%CH' or v.BasicModel like '%PU' or v.BasicModel like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate desc))
				 , PassengerYearFirst  = isnull((select top 1 v.SalesModelYear from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and NOT (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate asc),
                                                (select top 1 v.ProductionYear from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel not like '%ANG' and v.BasicModel not like '%CH' and v.BasicModel not like '%PU' and v.BasicModel not like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate asc))
				 , PassengerTypeFirst  = isnull((select top 1 v.SalesModelCode from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and NOT (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate asc),
                                                (select top 1 v.BasicModel from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel not like '%ANG' and v.BasicModel not like '%CH' and v.BasicModel not like '%PU' and v.BasicModel not like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate asc))
				 , PassengerYearLast   = isnull((select top 1 v.SalesModelYear from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and NOT (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate desc),
                                                (select top 1 v.ProductionYear from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel not like '%ANG' and v.BasicModel not like '%CH' and v.BasicModel not like '%PU' and v.BasicModel not like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate desc))
				 , PassengerTypeLast   = isnull((select top 1 v.SalesModelCode from omTrSalesInvoice i, omTrSalesInvoiceVIN v
												  where i.CompanyCode =v.CompanyCode
													and i.BranchCode  =v.BranchCode
													and i.InvoiceNo   =v.InvoiceNo
													and gnInquirySalesTemp.SysDate     =@SysDate
													and gnInquirySalesTemp.CompanyCode =i.CompanyCode
													and gnInquirySalesTemp.CustomerCode=i.CustomerCode
													and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
													and NOT (v.SalesModelCode like '%CH%' or v.SalesModelCode like '%PU%' or v.SalesModelCode like '%WD%' or v.SalesModelCode like '%FD%')
												  order by i.InvoiceDate desc),
                                                (select top 1 v.BasicModel from svMstCustomerVehicle v, svTrnService s
												  where gnInquirySalesTemp.SysDate     =@SysDate
												    and gnInquirySalesTemp.CompanyCode =v.CompanyCode
													and gnInquirySalesTemp.CustomerCode=v.CustomerCode
													and (v.BasicModel not like '%ANG' and v.BasicModel not like '%CH' and v.BasicModel not like '%PU' and v.BasicModel not like '%WD')
												    and s.CompanyCode =v.CompanyCode
												    and s.BranchCode  =(case when @Outlet='' then s.BranchCode else @Outlet end)
													and s.ChassisCode =v.ChassisCode
													and s.ChassisNo   =v.ChassisNo
												  order by v.CreatedDate desc))


    update gnInquirySalesTemp 
		set Salesman = (select top 1 s.Salesman from omTrSalesInvoice i, omTrSalesSO s
		                 where i.CompanyCode = s.CompanyCode
						   and i.BranchCode  = s.BranchCode
						   and i.SONo        = s.SONo
						   and gnInquirySalesTemp.SysDate     =@SysDate
						   and gnInquirySalesTemp.CompanyCode =i.CompanyCode
						   and gnInquirySalesTemp.CustomerCode=i.CustomerCode
						   and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
						 order by i.InvoiceDate desc)
		  , SC       = (select top 1 s.SalesCoordinator from omTrSalesInvoice i, omTrSalesSO s
		                 where i.CompanyCode = s.CompanyCode
						   and i.BranchCode  = s.BranchCode
						   and i.SONo        = s.SONo
						   and gnInquirySalesTemp.SysDate     =@SysDate
						   and gnInquirySalesTemp.CompanyCode =i.CompanyCode
						   and gnInquirySalesTemp.CustomerCode=i.CustomerCode
						   and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
						 order by i.InvoiceDate desc)
		  , SH       = (select top 1 s.SalesHead from omTrSalesInvoice i, omTrSalesSO s
		                 where i.CompanyCode = s.CompanyCode
						   and i.BranchCode  = s.BranchCode
						   and i.SONo        = s.SONo
						   and gnInquirySalesTemp.SysDate     =@SysDate
						   and gnInquirySalesTemp.CompanyCode =i.CompanyCode
						   and gnInquirySalesTemp.CustomerCode=i.CustomerCode
						   and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
						 order by i.InvoiceDate desc)
		  , BM       = (select top 1 s.BranchManager from omTrSalesInvoice i, omTrSalesSO s
		                 where i.CompanyCode = s.CompanyCode
						   and i.BranchCode  = s.BranchCode
						   and i.SONo        = s.SONo
						   and gnInquirySalesTemp.SysDate     =@SysDate
						   and gnInquirySalesTemp.CompanyCode =i.CompanyCode
						   and gnInquirySalesTemp.CustomerCode=i.CustomerCode
						   and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
						 order by i.InvoiceDate desc)
	 where exists (select top 1 1 from omTrSalesInvoice i, omTrSalesSO s
		                 where i.CompanyCode = s.CompanyCode
						   and i.BranchCode  = s.BranchCode
						   and i.SONo        = s.SONo
						   and gnInquirySalesTemp.SysDate     =@SysDate
						   and gnInquirySalesTemp.CompanyCode =i.CompanyCode
						   and gnInquirySalesTemp.CustomerCode=i.CustomerCode
						   and i.BranchCode  =(case when @Outlet='' then i.BranchCode else @Outlet end)
						 order by i.InvoiceDate desc)

    update gnInquirySalesTemp 
		set Salesman = isnull((select top 1 EmployeeName from gnMstEmployee
		                        where gnInquirySalesTemp.SysDate    =@SysDate 
								  and gnInquirySalesTemp.CompanyCode=CompanyCode
			                      and gnInquirySalesTemp.Salesman   =EmployeeID
					              and PersonnelStatus=1),'')
		  , SC       = isnull((select top 1 EmployeeName from gnMstEmployee
		                        where gnInquirySalesTemp.SysDate    =@SysDate 
								  and gnInquirySalesTemp.CompanyCode=CompanyCode
			                      and gnInquirySalesTemp.SC         =EmployeeID
					              and PersonnelStatus=1),'')
		  , SH       = isnull((select top 1 EmployeeName from gnMstEmployee
		                        where gnInquirySalesTemp.SysDate    =@SysDate 
								  and gnInquirySalesTemp.CompanyCode=CompanyCode
			                      and gnInquirySalesTemp.SH         =EmployeeID
					              and PersonnelStatus=1),'')
		  , BM       = isnull((select top 1 EmployeeName from gnMstEmployee
		                        where gnInquirySalesTemp.SysDate    =@SysDate 
								  and gnInquirySalesTemp.CompanyCode=CompanyCode
			                      and gnInquirySalesTemp.BM         =EmployeeID
					              and PersonnelStatus=1),'')

    update gnInquirySalesTemp 
		set Salesman = (case when Salesman is not NULL then Salesman
		                     when SC       is not NULL then SC
							 when SH       is not NULL then SH
							 when BM       is not NULL then BM
					         else                           ''
					    end)

    update gnInquirySalesTemp 
		set SC       = (case when SC       is not NULL then SC
							 when SH       is not NULL then SH
							 when BM       is not NULL then BM
					         else                           ''
					    end)

    update gnInquirySalesTemp 
		set SH       = (case when SH       is not NULL then SH
							 when BM       is not NULL then BM
					         else                           ''
					    end)

    update gnInquirySalesTemp 
		set BM       = (case when BM       is not NULL then BM
					         else                           ''
					    end)

	select m.Area, m.DealerName, m.DealerAbbreviation, a.* 
	  from gnInquirySalesTemp a
	 inner join gnMstDealerMapping m
	    on m.DealerCode=a.CompanyCode
		where SysDate=@SysDate

	 order by m.Area, m.DealerAbbreviation, a.CustomerStatus, a.CustomerName
	delete gnInquirySalesTemp where SysDate=@SysDate
  --drop table gnInquirySalesTemp 
END