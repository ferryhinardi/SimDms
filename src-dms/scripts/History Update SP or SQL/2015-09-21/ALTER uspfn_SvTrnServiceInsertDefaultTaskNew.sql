/****** Object:  StoredProcedure [dbo].[uspfn_SvTrnServiceInsertDefaultTaskNew]    Script Date: 9/18/2015 3:23:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_SvTrnServiceInsertDefaultTaskNew]

	@CompanyCode varchar(15),

	@BranchCode varchar(15),

	@ProductType varchar(15),

	@ServiceNo bigint,

	@ServiceBookNo varchar(15),

	@UserID varchar(15)

as      



--declare @CompanyCode varchar(15),

--		@BranchCode varchar(15),

--		@ProductType varchar(15),

--		@ServiceNo bigint,

--		@ServiceBookNo varchar(15)		



--set @CompanyCode = '6006406'

--set	@BranchCode = '6006401'

--set	@ProductType = '4W'

--set	@ServiceNo = '40655'

--set @ServiceBookNo = 'EJ06'



-- check jika count svTrnSrvTask ada maka tidak perlu insert task 

if(select count(*) from svTrnSrvTask

    where 1 = 1

      and CompanyCode = @CompanyCode

      and BranchCode  = @BranchCode

      and ProductType = @ProductType

      and ServiceNo   = @ServiceNo) > 0

	return



-- select data svTrnService

select * into #srv from (

  select a.* from svTrnService a

	where 1 = 1

	  and a.CompanyCode = @CompanyCode

	  and a.BranchCode  = @BranchCode

	  and a.ProductType = @ProductType

	  and a.ServiceNo   = @ServiceNo

)#srv



-----------------------------------------------

-- insert default svTrnSrvTask

-----------------------------------------------

select * into #task from(

select a.CompanyCode, a.ProductType, a.BasicModel, a.JobType, a.OperationNo, a.Description

	 , isnull(c.OperationHour, a.OperationHour) OperationHour

	 , isnull(c.ClaimHour, a.ClaimHour) ClaimHour

	 , isnull(c.LaborCost, a.LaborCost) LaborCost

	 , isnull(c.LaborPrice, a.LaborPrice) LaborPrice

	 , a.IsSubCon, a.IsCampaign, b.CreatedBy as LastupdateBy, getdate() as  LastupdateDate

	 , case when exists (

			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType

			  from svMstPackage pkg

			 inner join svMstPackageTask tsk

				on tsk.CompanyCode = pkg.CompanyCode

			   and tsk.PackageCode = pkg.PackageCode

			 inner join svMstPackageContract con

				on con.CompanyCode = pkg.CompanyCode

			   and con.PackageCode = pkg.PackageCode

			 where pkg.CompanyCode = b.CompanyCode

			   and pkg.JobType = b.JobType

			   and tsk.OperationNo = a.OperationNo

			   and con.ChassisCode = b.ChassisCode

			   and con.ChassisNo = b.ChassisNo

		) then 'P' else (case when isnull(a.BillType, '') = '' then 'C' else a.BillType end)

		  end as BillType

  from svMstTask a

 inner join #srv b

    on b.CompanyCode = a.CompanyCode

   and b.ProductType = a.ProductType

   and b.BasicModel  = a.BasicModel

   and b.JobType     = a.JobType

  left join svMstTaskPrice c

	on c.CompanyCode = a.CompanyCode

   and c.BranchCode  = b.BranchCode

   and c.ProductType = a.ProductType

   and c.BasicModel  = a.BasicModel

   and c.JobType     = a.JobType

   and c.OperationNo = a.OperationNo

 where 1 = 1

)#task



-- jika svMstTask tidak tepat 1 record, return

if (select count(*) from #task) <> 1 return



select * into #job from(

select a.* from svMstJob a, #task b

 where 1 = 1

   and a.CompanyCode = b.CompanyCode

   and a.ProductType = b.ProductType

   and a.BasicModel  = b.BasicModel

   and a.JobType     = b.JobType

)#job



-- jika svMstJob tidak tepat 1 record, return

if (select count(*) from #job) <> 1 return



-- prepare data svTrnSrvTask yg akan di Insert

declare @JobType varchar(15) set @JobType = (select JobType from #job)



if (left(@JobType,3) = 'FSC' or left(@JobType,3) = 'PDI')

begin

	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)

	select

		 @CompanyCode CompanyCode

		,@BranchCode BranchCode

		,@ProductType ProductType

		,@ServiceNo ServiceNo

		,a.OperationNo

		,a.OperationHour

		,OperationCost = isnull((select top 1 a.RegularLaborAmount

						   from svMstPdiFscRate a, #srv b, #task c, #job d 

						  where 1 = 1

						    and a.CompanyCode = b.CompanyCode

						    and a.ProductType = b.ProductType

						    and a.BasicModel = b.BasicModel

						    and a.IsCampaign = c.IsCampaign

						    and a.TransmissionType = a.TransmissionType

						    and a.PdiFscSeq = d.PdiFscSeq

						    and a.EffectiveDate <= getdate()

						    and a.IsActive = 1

						 order by a.EffectiveDate desc),0)

		,IsSubCon

		,a.LaborCost SubConPrice

		,'' PONo

		,ClaimHour

		,'L' TypeOfGoods

	    , case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType

		,'0' SharingTask

		,'0' TaskStatus

		,null StartService

		,null FinishService

		,b.LastupdateBy CreatedBy

		,b.LastupdateDate CreatedDate

		,b.LastupdateBy

		,b.LastupdateDate

		,isnull((

			select cus.LaborDiscPct

			  from svMstBillingType bil

			 inner join gnMstCustomerProfitCenter cus

				on cus.CompanyCode = bil.CompanyCode

			   and cus.CustomerCode = bil.CustomerCode 

			 where 1 = 1

			   and bil.CompanyCode = @CompanyCode

			   and cus.BranchCode = @BranchCode

			   and cus.ProfitCenterCode = '200'

			   and bil.BillType = 'F'

			), b.LaborDiscPct) as LaborDiscPct

	from #task a, #srv b

end

else if @JobType = 'CLAIM'

begin

	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)

	select

		 @CompanyCode CompanyCode

		,@BranchCode BranchCode

		,@ProductType ProductType

		,@ServiceNo ServiceNo

		,a.OperationNo

		,a.ClaimHour OperationHour

		,a.LaborPrice OperationCost

		,a.IsSubCon

		,a.LaborCost SubConPrice

		,'' PONo

		,a.ClaimHour

		,'L' TypeOfGoods

		,'W' BillType

		,'0' SharingTask

		,'0' TaskStatus

		,null StartService

		,null FinishService

		,b.LastupdateBy CreatedBy

		,b.LastupdateDate CreatedDate

		,b.LastupdateBy

		,b.LastupdateDate

		,b.LaborDiscPct

	from #task a, #srv b

end

else if @JobType = 'REWORK'

begin

	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)

	select

		 @CompanyCode CompanyCode

		,@BranchCode BranchCode

		,@ProductType ProductType

		,@ServiceNo ServiceNo

		,a.OperationNo

		,a.OperationHour

		,a.LaborPrice OperationCost

		,a.IsSubCon

		,a.LaborCost SubConPrice

		,'' PONo

		,ClaimHour

		,'L' TypeOfGoods

		,'I' BillType

		,'0' SharingTask

		,'0' TaskStatus

		,null StartService

		,null FinishService

		,b.LastupdateBy CreatedBy

		,b.LastupdateDate CreatedDate

		,b.LastupdateBy

		,b.LastupdateDate

		,b.LaborDiscPct

	from #task a, #srv b

end

else

begin

	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)

	select

		 @CompanyCode CompanyCode

		,@BranchCode BranchCode

		,@ProductType ProductType

		,@ServiceNo ServiceNo

		,a.OperationNo

		,a.OperationHour

		,a.LaborPrice OperationCost

		,IsSubCon

		,a.LaborCost SubConPrice

		,'' PONo

		,ClaimHour

		,'L' TypeOfGoods

		, case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType

		,'0' SharingTask

		,'0' TaskStatus

		,null StartService

		,null FinishService

		,b.LastupdateBy CreatedBy

		,b.LastupdateDate CreatedDate

		,b.LastupdateBy

		,b.LastupdateDate

		,isnull((

			select top 1 tsk.DiscPct

			  from svMstPackage pkg

			 inner join svMstPackageTask tsk

				on tsk.CompanyCode = pkg.CompanyCode

			   and tsk.PackageCode = pkg.PackageCode

			 inner join svMstPackageContract con

				on con.CompanyCode = pkg.CompanyCode

			   and con.PackageCode = pkg.PackageCode

			 where pkg.CompanyCode = b.CompanyCode

			   and pkg.JobType = b.JobType

			   and tsk.OperationNo = a.OperationNo

			   and con.ChassisCode = b.ChassisCode

			   and con.ChassisNo = b.ChassisNo

			), b.LaborDiscPct) LaborDiscPct

		

	from #task a, #srv b

end

-----------------------------------------------

-- insert default svTrnSrvItem

-----------------------------------------------

select * into #part from(

select a.* from svMstTaskPart a, #task b

 where 1 = 1

   and a.CompanyCode = b.CompanyCode

   and a.ProductType = b.ProductType

   and a.BasicModel  = b.BasicModel

   and a.JobType     = b.JobType

   and a.OperationNo = b.OperationNo

)#part



DECLARE @QueryTemp VARCHAR(MAX)



  declare @tblTemp as table  

	  (  

	  CompanyCode varchar(20),

	  BranchCode varchar(20),

	  PartNo varchar(20),

	  TypeOfGoods varchar(2),

	   RetailPrice decimal(18,2),  
	   CostPrice decimal(18,2),  
	   PurcDiscPct decimal(18,2) 

	  )

		set @QueryTemp = 'select a.CompanyCode,a.BranchCode,a.PartNo,a.TypeOfGoods,b.RetailPrice,b.CostPrice, a.PurcDiscPct from spmstitems a
			join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spMstItemPrice b on  a.PartNo = b.PartNo'
			+ ' where a.Companycode=''' + @companycode + ''' and a.BranchCode = ''' + @BranchCode +
			+ ''' and  b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) 
			+ ''' and b.BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + ''''


	  insert into @tblTemp    

	  exec (@QueryTemp)



	  declare @PurchaseDisc as decimal  

	  set @PurchaseDisc = (select DiscPct from gnMstSupplierProfitCenter   

		   where CompanyCode = @CompanyCode   

		   and BranchCode = @BranchCode  

		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)

		   and ProfitCenterCode = '300')  

	         

	  if (@PurchaseDisc is null) raiserror ('Profit Center 300 belum tersetting pada Supplier tersebut!!!',16,1);         



insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)

select 

	 @CompanyCode CompanyCode

	,@BranchCode BranchCode

	,@ProductType ProductType

	,@ServiceNo ServiceNo

	,a.PartNo

	,(row_number() over (order by a.PartNo)) PartSeq

	,a.Quantity DemandQty

	,'0' SupplyQty

	,'0' ReturnQty

	,
	case when dbo.IsIndependentDealer() = 1 then
		 isnull(d.CostPrice,0)
	else
		case when ISNULL(d.PurcDiscPct, 0) = 0 then 
			 isnull((d.RetailPrice - (d.RetailPrice *(@PurchaseDisc * 0.01))),0) 
		else 
			 isnull((d.RetailPrice - (d.RetailPrice * (d.PurcDiscPct * 0.01))),0) 
		end
	end CostPrice

	,case rtrim(a.BillType) when 'F' then isnull(a.RetailPrice,0) else isnull(d.RetailPrice,0) end RetailPrice

	,d.TypeOfGoods

	,case when exists (

			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType

			  from svMstPackage pkg

			 inner join svMstPackagePart prt

				on prt.CompanyCode = pkg.CompanyCode

			   and prt.PackageCode = pkg.PackageCode

			 inner join svMstPackageContract con

				on con.CompanyCode = pkg.CompanyCode

			   and con.PackageCode = pkg.PackageCode

			 inner join #srv srv

				on srv.CompanyCode = con.CompanyCode

			   and srv.ChassisCode = con.ChassisCode

			   and srv.ChassisNo = con.ChassisNo

			 where pkg.CompanyCode = b.CompanyCode

			   and pkg.JobType = b.JobType

			   and prt.PartNo = a.PartNo

		) then 'P' else 

		(case when isnull(a.BillType, '') = '' then 'C' else 

			(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%OIL FILTER%' and (select PdiFscSeq from #job) = 1 then 'C' else 		

			

				(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%ENGINE%' and (select PdiFscSeq from #job) = 3 then 'C' else 		

				a.BillType 

				end)

			

			end)		

		end)

		  end as BillType

	,null SupplySlipNo

	,null SupplySlipDate

	,null SSReturnNo

	,null SSReturnDate

	,b.LastupdateBy CreatedBy

	,b.LastupdateDate CreatedDate

	,b.LastupdateBy

	,b.LastupdateDate

	,isnull((

		select top 1 prt.DiscPct

		  from svMstPackage pkg

		 inner join svMstPackagePart prt

			on prt.CompanyCode = pkg.CompanyCode

		   and prt.PackageCode = pkg.PackageCode

		 inner join svMstPackageContract con

			on con.CompanyCode = pkg.CompanyCode

		   and con.PackageCode = pkg.PackageCode

		 inner join #srv srv

			on srv.CompanyCode = con.CompanyCode

		   and srv.ChassisCode = con.ChassisCode

		   and srv.ChassisNo = con.ChassisNo

		 where pkg.CompanyCode = b.CompanyCode

		   and pkg.JobType = b.JobType

		   and prt.PartNo = a.PartNo

		), 

		(case when rtrim(a.BillType) = 'F' and rtrim(e.ParaValue) = 'SPAREPART' then 0

		      when rtrim(a.BillType) = 'F' then 0

		      when rtrim(e.ParaValue) = 'SPAREPART' then (select top 1 PartDiscPct from #srv) 

			  else (select top 1 MaterialDiscPct from #srv) end)

		) as DiscPct

  from #part a

  left join #task b

    on b.CompanyCode = a.CompanyCode

   and b.ProductType = a.ProductType

   and b.BasicModel  = a.BasicModel

   and b.JobType     = a.JobType

   and b.OperationNo = a.OperationNo

   LEFT join @tblTemp d

   on d.CompanyCode = a.CompanyCode

   and d.BranchCode = @BranchCode

   and d.PartNo = a.PartNo

  --left join spMstItemPrice c

  --  on c.CompanyCode = a.CompanyCode

  -- and c.BranchCode  = @BranchCode

  -- and c.PartNo      = a.PartNo

  --left join spMstItems d

  --  on d.CompanyCode = a.CompanyCode

  -- and d.BranchCode  = @BranchCode

  -- and d.PartNo      = a.PartNo

  left join gnMstLookupDtl e

    on e.CompanyCode = d.CompanyCode

   and e.CodeID = 'GTGO'

   and e.LookupValue = d.TypeOfGoods

   left join spMstItemInfo f

	on f.CompanyCode = a.CompanyCode

	and f.PartNo = a.PartNo

 where 1 = 1

   and b.CompanyCode = a.CompanyCode

   and b.ProductType = a.ProductType

   and b.BasicModel  = a.BasicModel

   and b.JobType     = a.JobType

   and b.OperationNo = a.OperationNo

exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID



drop table #srv

drop table #task

drop table #part

drop table #job
