
go
if object_id('uspfn_SvLkuTaskPart') is not null
	drop procedure uspfn_SvLkuTaskPart


go
CREATE procedure uspfn_SvLkuTaskPart  
    @CompanyCode varchar(20),  
    @BranchCode  varchar(20),  
    @BasicModel  varchar(50),
    @JobType     varchar(20),
    @ChassisCode varchar(50),  
    @ChassisNo   varchar(50),  
    @TransType   varchar(20),
    @ItemType    varchar(20),
    @BillType    varchar(20)

as  

if @ItemType = 'L' and @BillType = 'F'
begin
	select a.OperationNo
		 , a.OperationHour
		 , LaborPrice = isnull((
			select top 1 b.RegularLaborAmount from svMstPdiFscRate b
			 inner join svMstCustomerVehicle c on c.TransmissionType = c.TransmissionType
			   and c.CompanyCode = b.CompanyCode
			   and c.ChassisCode = @ChassisCode
			   and c.ChassisNo = @ChassisNo
			 where b.EffectiveDate <= getdate()
			   and b.CompanyCode = a.CompanyCode
			   and b.ProductType = a.ProductType
			   and b.BasicModel = a.BasicModel
			   and b.IsCampaign = a.IsCampaign
			   and b.TransmissionType = @TransType
			   and b.PdiFscSeq = d.PdiFscSeq
			   and b.IsActive = 1
			 order by EffectiveDate desc
			 ),0)
		 , a.Description
		 , case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as IsActive
	  from svMstTask a
	  left join svMstJob d
		on d.CompanyCode = a.CompanyCode
	   and d.ProductType = a.ProductType
	   and d.BasicModel = a.BasicModel
	   and d.JobType = a.JobType
	   and d.IsPdiFsc = 1
	   and d.IsActive = 1
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BasicModel = @BasicModel
	   and a.JobType = @JobType    	
	   and a.IsActive = 1
	 order by a.OperationNo asc  
end
else if @ItemType = 'L' and @BillType != 'F'
begin
	select a.OperationNo
		 , case @JobType when 'CLAIM' then isnull(b.ClaimHour, a.ClaimHour) else isnull(b.OperationHour, a.OperationHour) end as OperationHour
		 , case @JobType when 'CLAIM' then a.LaborCost else isnull(b.LaborPrice, a.LaborPrice) end as LaborPrice
		 , a.Description
		 , case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as IsActive
	  from svMstTask a
	  left join svMstTaskPrice b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = @BranchCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel  = a.BasicModel
	   and b.JobType     = a.JobType
	   and b.OperationNo = a.OperationNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BasicModel  = @BasicModel
	   and a.IsActive    = 1
	   and a.Description != ''
end	 
else
begin
	select TOP 2500 a.PartNo
		 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) 
		 - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) as Available
		 , c.RetailPriceInclTax, a.PartName, b.TypeOfGoods
		 , case e.ParaValue when 'SPAREPART' then 'SPAREPART' else 'MATERIAL' end GroupTypeOfGoods
		 , (case a.Status when 1 then 'Aktif' else 'Tidak Aktif' end) as Status
		 , c.RetailPrice as NilaiPart
	  from spMstItemInfo a 
	  left join spMstItems b
	    on b.CompanyCode = a.CompanyCode 
	   and b.BranchCode = @BranchCode
	   and b.PartNo = a.PartNo
	 inner join spMstItemPrice c
	    on c.CompanyCode = a.CompanyCode 
	   and c.BranchCode = @BranchCode
	   and c.PartNo = b.PartNo
	  left join spMstItemLoc d
	    on d.CompanyCode = a.CompanyCode 
	   and d.BranchCode = @BranchCode
	   and d.PartNo = a.PartNo
	   and d.WarehouseCode = '00'
	  left join gnMstLookupDtl e
		on e.CompanyCode = b.CompanyCode
	   and e.CodeID = 'GTGO'
	   and e.LookupValue = b.TypeOfGoods
	 where a.CompanyCode = @CompanyCode
	   and d.partno is not null
	   and a.PartName is not null
end