ALTER procedure [dbo].[uspfn_SvMstLbrPriceUpdInMstJob]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@UserId varchar(15),
	@Now datetime
as      

-- update <> ('FSC')
;with t_task
as
(
    select CompanyCode, ProductType, BasicModel, OperationNo, Description, LaborPrice, LastUpdateBy, LastUpdateDate,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTask.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTask.ProductType
            and LaborCode   = 'CUSTOMER'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as CustomerPrice,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTask.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTask.ProductType
            and LaborCode   = 'AREAFACT'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as AreaFact,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTask.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTask.ProductType
            and LaborCode   = svMstTask.BasicModel
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as ModelFact,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTask.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTask.ProductType
            and LaborCode   = 'DLRSPTYPE'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as DealerFact
     from svMstTask
    where 1 = 1
      and CompanyCode = @CompanyCode
      and ProductType = @ProductType
      and exists (
     select * from svMstJob
      where CompanyCode  = svMstTask.CompanyCode
        and ProductType  = svMstTask.ProductType
        and BasicModel   = svMstTask.BasicModel
        and JobType      = svMstTask.JobType
        and GroupJobType not in ('FSC') -- update 23 Apr  BDR dihapus
    )
)
--select * from t_task where LaborPrice <> (CustomerPrice * AreaFact * ModelFact * DealerFact)
update t_task
   set LaborPrice = (CustomerPrice * AreaFact * ModelFact * DealerFact)
     , LastUpdateBy   = @UserID
     , LastUpdateDate = @Now
 where LaborPrice <> (CustomerPrice * AreaFact * ModelFact * DealerFact)

--//Penambahan 
 ;with t_taskprice
as
(
    select CompanyCode, BranchCode, ProductType, BasicModel, OperationNo, LaborPrice,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTaskPrice.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTaskPrice.ProductType
            and LaborCode   = 'CUSTOMER'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as CustomerPrice,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTaskPrice.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTaskPrice.ProductType
            and LaborCode   = 'AREAFACT'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as AreaFact,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTaskPrice.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTaskPrice.ProductType
            and LaborCode   = svMstTaskPrice.BasicModel
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as ModelFact,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTaskPrice.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTaskPrice.ProductType
            and LaborCode   = 'DLRSPTYPE'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as DealerFact
     from svMstTaskPrice
    where 1 = 1
      and CompanyCode = @CompanyCode
      and ProductType = @ProductType
      and BranchCode = @BranchCode
      and exists (
     select * from svMstJob
      where CompanyCode  = svMstTaskPrice.CompanyCode
        and ProductType  = svMstTaskPrice.ProductType
        and BasicModel   = svMstTaskPrice.BasicModel
        and JobType      = svMstTaskPrice.JobType
        and GroupJobType not in ('FSC')
    )
)

update t_taskprice
   set LaborPrice = (CustomerPrice * AreaFact * ModelFact * DealerFact)
where LaborPrice <> (CustomerPrice * AreaFact * ModelFact * DealerFact)

--//Penambahan sampai disini
 
-- update CLM
;with t_task
as
(
    select CompanyCode, ProductType, BasicModel, OperationNo, Description, LaborCost, LastUpdateBy, LastUpdateDate,
        isnull((
         select top 1 LaborPrice from svMstlaborRate
          where CompanyCode = svMstTask.CompanyCode
            and BranchCode  = @BranchCode
            and ProductType = svMstTask.ProductType
            and LaborCode   = 'SUZUKI'
            and IsActive    = 1
            and EffectiveDate <= getdate()
          order by EffectiveDate desc
        ),0) as NewLaborCost
     from svMstTask
    where 1 = 1
      and CompanyCode = @CompanyCode
      and ProductType = @ProductType
      and exists (
     select * from svMstJob
      where CompanyCode  = svMstTask.CompanyCode
        and ProductType  = svMstTask.ProductType
        and BasicModel   = svMstTask.BasicModel
        and JobType      = svMstTask.JobType
        and GroupJobType = 'CLM'
    )
)
--select * from t_task where LaborCost <> NewLaborCost
update t_task
   set LaborCost = NewLaborCost
     , LastUpdateBy   = @UserID
     , LastUpdateDate = @Now
 where LaborCost <> NewLaborCost
