ALTER procedure [dbo].[uspfn_MonitoringDealer]
	@ProductType nvarchar(20),
	@DealerCode varchar(20),
	@IsWhat varchar(200)
as
	--if exists (select *	from sysLastTrnInfo where DealerCode = @DealerCode and ModuleName = @IsWhat )
	--BEGIN
		select a.DealerCode, case when @IsWhat <> 'cs' then b.DealerName + ' - ' +OutletName else b.DealerName end as DealerName, 
		a.OutletCode, c.OutletName, a.UserName, a.LastLogin, a.LastUpdated
		from sysLastTrnInfo a
		inner join (select DealerCode, DealerName, ProductType from dealerinfo) b
			on a.DealerCode = b.DealerCode
		inner join (select DealerCode, OutletCode, OutletName from gnMstDealerOutletMapping) c
			on a.DealerCode = c.DealerCode and a.OutletCode = c.OutletCode
		where b.ProductType = case when @ProductType = '' then b.ProductType else @ProductType end  
		and a.DealerCode = case when @DealerCode = '' then a.DealerCode else @DealerCode end 
		and a.ModuleName = case when @IsWhat = '' then a.ModuleName else @IsWhat end
		and a.UserName <> 'ga'
	--END ELSE BEGIN
	--print 'Else'
		--select '6159401000'DealerCode, 'SEJAHTERA ARMADA TRADA'DealerName, ''OutletCode, ''OutletName, 'Syawal'UserName, '2015-05-04'LastLogin, '2015-05-04'LastUpdated
	--END
