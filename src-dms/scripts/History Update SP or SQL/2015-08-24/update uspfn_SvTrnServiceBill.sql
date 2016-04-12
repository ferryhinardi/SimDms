ALTER procedure [dbo].[uspfn_SvTrnServiceBill]
--DECLARE
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   varchar(15),
	@BillType    varchar(15)

as      
--SELECT @CompanyCode='6641401', @BranchCode='664140101', @ProductType='4W',@ServiceNo= 2954, @BillType='C'

select * into #srv from (
  select a.* from svTrnService a
    where 1 = 1
      and a.CompanyCode = @CompanyCode
      and a.BranchCode  = @BranchCode
      and a.ProductType = @ProductType
      and a.ServiceNo   = @ServiceNo
)#srv

declare @LaborHour numeric(9,2)
set @LaborHour = isnull((select sum(isnull(OperationHour,0))
                           from svTrnSrvTask
                          where 1 = 1
                            and CompanyCode = @CompanyCode
                            and BranchCode  = @BranchCode
                            and ProductType = @ProductType
                            and ServiceNo   = @ServiceNo
						 ),0)

-- Recalculate Amount Service
declare @LaborDiscPct    numeric(5,2)   set @LaborDiscPct    = isnull((select top 1 LaborDiscPct from #srv),0)
declare @PartsDiscPct    numeric(5,2)   set @PartsDiscPct    = isnull((select top 1 PartDiscPct from #srv),0)
declare @MaterialDiscPct numeric(5,2)	set @MaterialDiscPct = isnull((select top 1 MaterialDiscPct from #srv),0)
declare @PpnPct          numeric(5,2)	set @PPNPct          = isnull((select PPNPct from #srv),0)

----------------------------------------------------------
-- get data gross amount
declare @LaborGrossAmt decimal
    set @LaborGrossAmt = isnull((
						select sum(OperationHour * OperationCost) from svTrnSrvTask
						 where 1 = 1
                           and CompanyCode = @CompanyCode
                           and BranchCode  = @BranchCode
                           and ProductType = @ProductType
                           and ServiceNo   = @ServiceNo
                           and BillType like @BillType
                         ),0)

declare @PartsGrossAmt decimal
    set @PartsGrossAmt = isnull((
						select sum(a.DemandQty * a.RetailPrice)
						  from svTrnSrvItem a
						  left join gnMstLookUpDtl b
							on b.CompanyCode = a.CompanyCode
						   and b.LookUpValue = a.TypeOfGoods
						   and b.CodeID      = 'GTGO'
						   and b.ParaValue   = 'SPAREPART'
						 where a.CompanyCode = @CompanyCode
					  	   and a.BranchCode  = @BranchCode
						   and a.ProductType = @ProductType
                           and a.ServiceNo   = @ServiceNo
                           and a.BillType like @BillType
                           and b.CompanyCode is not null
                         ),0)

declare @MaterialGrossAmt decimal
    set @MaterialGrossAmt = isnull((
						select sum(a.DemandQty * a.RetailPrice)
						  from svTrnSrvItem a
						  left join gnMstLookUpDtl b
							on b.CompanyCode = a.CompanyCode
						   and b.LookUpValue = a.TypeOfGoods
						   and b.CodeID      = 'GTGO'
						   and b.ParaValue  in ('OLI','MATERIAL')
						 where a.CompanyCode = @CompanyCode
					  	   and a.BranchCode  = @BranchCode
						   and a.ProductType = @ProductType
                           and a.ServiceNo   = @ServiceNo
                           and a.BillType like @BillType
                           and b.CompanyCode is not null
                         ),0)

---- calculate discount
declare @LaborDiscAmt decimal
		set @LaborDiscAmt = isnull((select LaborDiscAmt from svTrnService
						where 1 = 1
                           and CompanyCode = @CompanyCode
                           and BranchCode  = @BranchCode
                           and ProductType = @ProductType
                           and ServiceNo   = @ServiceNo
							),0)
    --set @LaborDiscAmt = isnull((
				--		select sum(ceiling(OperationHour * OperationCost * @LaborDiscPct / 100.0))
    --                      from svTrnSrvTask
				--		 where 1 = 1
    --                       and CompanyCode = @CompanyCode
    --                       and BranchCode  = @BranchCode
    --        and ProductType = @ProductType
    --                       and ServiceNo   = @ServiceNo
    --                       and BillType like @BillType
    --                     ),0)

declare @PartsDiscAmt decimal
		set @PartsDiscAmt = isnull((select PartsDiscAmt from svTrnService
						where 1 = 1
                           and CompanyCode = @CompanyCode
                           and BranchCode  = @BranchCode
                           and ProductType = @ProductType
                           and ServiceNo   = @ServiceNo
							),0)
    --set @PartsDiscAmt = isnull((
				--		select sum(ceiling(a.DemandQty * a.RetailPrice * @PartsDiscPct / 100.0))
    --                      from svTrnSrvItem a
				--		  left join gnMstLookUpDtl b
				--			on b.CompanyCode = a.CompanyCode
				--		   and b.LookUpValue = a.TypeOfGoods
				--		   and b.CodeID      = 'GTGO'
				--		   and b.ParaValue   = 'SPAREPART'
				--		 where a.CompanyCode = @CompanyCode
				--	  	   and a.BranchCode  = @BranchCode
				--		   and a.ProductType = @ProductType
    --                       and a.ServiceNo   = @ServiceNo
    --                       and a.BillType like @BillType
    --                       and b.CompanyCode is not null
    --                     ),0)

declare @MaterialDiscAmt decimal
		set @MaterialDiscAmt = isnull((select MaterialDiscAmt from svTrnService
						where 1 = 1
                           and CompanyCode = @CompanyCode
                           and BranchCode  = @BranchCode
                           and ProductType = @ProductType
                           and ServiceNo   = @ServiceNo
							),0)
    --set @MaterialDiscAmt = isnull((
				--		select sum(ceiling(a.DemandQty * a.RetailPrice * @MaterialDiscPct / 100.0)) 
    --                      from svTrnSrvItem a
  		--				  left join gnMstLookUpDtl b
				--			on b.CompanyCode = a.CompanyCode
				--		   and b.LookUpValue = a.TypeOfGoods
				--		   and b.CodeID      = 'GTGO'
				--		   and b.ParaValue  in ('OLI','MATERIAL')
				--		 where a.CompanyCode = @CompanyCode
				--	  	   and a.BranchCode  = @BranchCode
				--		   and a.ProductType = @ProductType
    --                       and a.ServiceNo   = @ServiceNo
    --                       and a.BillType like @BillType
    --                       and b.CompanyCode is not null
    --                     ),0)

declare @GroupJobType varchar(3) 
set     @GroupJobType = isnull((select GroupJobType from svMstJob
                                 where 1 = 1
                                   and CompanyCode    = @CompanyCode
                                   and ProductType    = @ProductType
                                   and BasicModel     = (select BasicModel from svTrnService
														  where CompanyCode = @CompanyCode
														    and BranchCode  = @BranchCode
														    and ProductType = @ProductType
														    and ServiceNo   = @ServiceNo)
                                   and JobType        = (select JobType    from svTrnService
														  where CompanyCode = @CompanyCode
														    and BranchCode  = @BranchCode
														    and ProductType = @ProductType
														    and ServiceNo   = @ServiceNo)
								),'XXX')

-- calculate DPP (dasar pengenaan pajak)
declare @LaborDppAmt     decimal	set @LaborDppAmt     = floor(@LaborGrossAmt    - @LaborDiscAmt)
declare @PartsDppAmt     decimal	set @PartsDppAmt     = floor(@PartsGrossAmt    - @PartsDiscAmt)
declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = floor(@MaterialGrossAmt - @MaterialDiscAmt)
declare @TotalDppAmt     decimal	set @TotalDppAmt     = floor(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt)

-- calculate ppn & service amount
declare @TotalPpnAmt     decimal	set @TotalPpnAmt = floor(@TotalDppAmt * @PpnPct / 100.0)
declare @TotalSrvAmt     decimal	set @TotalSrvAmt = floor(@TotalDppAmt + @TotalPpnAmt)

select JobType, TotalSrvAmount, @TotalSrvAmt CustTotalSrvAmt,
	   case @BillType
		when 'C' then 'Total Tagihan ke Pelanggan'
		else 'Total Biaya Perawatan'
	   end as AmtDesc
  from #srv

drop table #srv