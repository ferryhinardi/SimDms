
ALTER procedure [dbo].[uspfn_SvTrnServiceSelectDtl]
--declare
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   bigint
as      

begin
--select @CompanyCode='6006400001',	@BranchCode='6006400104', @ProductType='4W', @ServiceNo=58463

declare @t1 as table
(
 TaskPartSeq int
,BillType varchar(10)
,BillTypeDesc varchar(max)
,TypeOfGoods varchar(10)
,TypeOfGoodsDesc varchar(70)
,TaskPartNo varchar(50)
,OprHourDemandQty numeric(18,2)
,SupplyQty numeric(18,2)
,ReturnQty numeric(18,2)
,OprRetailPrice numeric(18,2)
,OprRetailPriceTotal numeric(18,2)
,SupplySlipNo varchar(20)
,TaskPartDesc varchar(max)
,BasicModel varchar(15)
,JobType varchar(15)
,IsSubCon bit
,Status varchar(10)
,GTGO varchar(10)
,DiscPct numeric(18,2)
,QtyAvail numeric(18,2)
,TaskStatus varchar(50)
)

declare @JobOrderNo varchar(15)
    set @JobOrderNo = isnull((select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType and ServiceNo = @ServiceNo), '')

insert into @t1
select 0 TaskSeq 
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,case a.TypeOfGoods when 'L' then 'Labor (Jasa)' end TypeOfGoodsDesc
      ,a.OperationNo
      ,a.OperationHour
      ,0 OperationHourSupply
      ,0 OperationHourReturn
      ,a.OperationCost
      ,a.OperationHour * a.OperationCost * (100 - a.DiscPct) * 0.01 OprRetailPriceTotal
      ,'' SupplySlipNo
      ,rtrim(d.Description) OperationDesc 
	  ,c.BasicModel
	  ,c.JobType
	  ,a.IsSubCon
	  ,(select min(MechanicStatus) from svTrnSrvMechanic 
		where CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode
			and ProductType = a.ProductType
			and ServiceNo = a.ServiceNo
			and OperationNo = a.OperationNo) MechanicStatus
	  ,''
	  ,a.DiscPct
	  ,0
      ,case(a.TaskStatus)
          when 0 then 'Open Task'
          when 1 then 'Work In Progress'
          when 2 then 'Close Task'
          when 9 then 'Cancel'
       end TaskStatus
  from svTrnSrvTask a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join svTrnService c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.ProductType = a.ProductType
   and c.ServiceNo = a.ServiceNo
  left join svMstTask d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.BasicModel = c.BasicModel
   and (d.JobType = c.JobType or d.JobType = 'CLAIM' or d.JobType = 'OTHER')
   and d.OperationNo = a.OperationNo 
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

declare @tblTemp as table
(
	PartNo  varchar(20),
	QtyAvail numeric(18,2)
)

declare @DealerCode as varchar(2)
declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

set @DealerCode = 'MD'
set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
begin	
	set @DealerCode = 'SD'
	declare @DbName as varchar(50)
	set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	declare @QueryTemp as varchar(max)
	
	set @QueryTemp = 'select 
			 distinct
			 a.PartNo
			 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) 
		 from svTrnSrvItem a	 
		 left join ' + @DbName + '..spMstItems b on 
			a.PartNo = b.PartNo 
			and b.CompanyCode = ''' + @CompanyMD + '''
			and b.BranchCode = ''' + @BranchMD + '''
		 where a.CompanyCode = ''' + @CompanyCode + '''
		   and a.BranchCode  = ''' + @BranchCode + '''
		   and a.ProductType = ''' + @ProductType + '''
		   and a.ServiceNo   = ' + convert(varchar,@ServiceNo) + ''		   
				
		print(@QueryTemp)		
		insert into @tblTemp		
		exec (@QueryTemp)		
end

insert into @t1
select a.PartSeq
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,rtrim(c.LookupValueName) + case lower(g.ParaValue) when 'sparepart' then ' (SPR)' else ' (MTR)' end TypeOfGoodsDesc
      ,a.PartNo
      ,a.DemandQty
      ,a.SupplyQty
      ,a.ReturnQty
      ,a.RetailPrice
      ,(case isnull(a.SupplyQty, 0)
         when 0 then (isnull(a.DemandQty, 0) * isnull(a.RetailPrice, 0))
         else ((isnull(a.SupplyQty, 0) - isnull(a.ReturnQty, 0)) * isnull(a.RetailPrice, 0))
        end) * (100.0 - a.DiscPct) * 0.01
        as RetailPriceTotal
      ,a.SupplySlipNo
      ,rtrim(d.PartName) OperationDesc 
	  ,''
	  ,''
	  ,0
	  ,''
	  ,g.ParaValue
	  ,a.DiscPct
	  ,case when @DealerCode = 'MD' then (i.OnHand - (i.AllocationSP + i.AllocationSR + i.AllocationSL) - (i.ReservedSP + i.ReservedSR + i.ReservedSL)) else e.QtyAvail end QtyAvail
	  ,''
  from svTrnSrvItem a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join gnMstLookupDtl c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'TPGO'
   and c.LookupValue = TypeOfGoods
  left join spMstItemInfo d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.PartNo = a.PartNo
  left join gnMstLookupDtl g with (nolock,nowait)
    on g.CompanyCode = a.CompanyCode
   and g.CodeID = 'GTGO'
   and g.LookupValue = TypeOfGoods
  left join svTrnService s with (nolock,nowait)
    on s.CompanyCode = a.CompanyCode
   and s.BranchCode = a.BranchCode
   and s.ServiceNo = a.ServiceNo
  left join spMstItems i
    on i.CompanyCode = a.CompanyCode 
   and i.BranchCode = a.BranchCode
   and i.ProductType = a.ProductType
   and i.PartNo = a.PartNo
   left join @tblTemp e
    on e.PartNo = a.PartNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

select * into #t1 from (
select 
 a.* 
,P01 = isnull((
	select count(*) from spTrnSORDDtl with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo      = a.TaskPartNo
	   and DocNo in (select aa.DocNo
					   from spTrnSORDHdr aa with(nowait,nolock), svTrnService bb with(nowait,nolock)
					  where 1 = 1
						and bb.CompanyCode = aa.CompanyCode
						and bb.BranchCode  = aa.BranchCode
						and bb.JobOrderNo  = aa.UsageDocNo
						and isnull(bb.JobOrderNo, '') <> ''
						and aa.CompanyCode = @CompanyCode
						and aa.BranchCode  = @BranchCode
						and bb.ServiceNo   = @ServiceNo
					 )
	),0)
,P02 = isnull((
	select count(DocNo) from spTrnSOSupply with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo = a.TaskPartNo
	   and DocNo  = a.SupplySlipNo
	),0)
,P03 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P04 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '2'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P05 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P06 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '1'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,S01 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	),0)
,S02 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '2'
	),0)
,S03 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '3'
	),0)
,S04 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '4'
	),0)
,S05 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '5'
	),0)
from @t1 a
)#t1

update #t1
   set Status = (case P01 when 0 then 0 else 1 end)
			  + (case P02 when 0 then 0 else 1 end)
			  + (case P03 when 0 then 0 else 1 end)
			  + (case P04 when 0 then 0 else 1 end)
			  + (case P05 when 0 then 0 else 1 end)
			  + (case P06 when 0 then 0 else 1 end)
 where TypeOfGoods <> 'L'

update #t1
   set Status = (case S01 when 0 then 0 else 1 end)
			  + (case S02 when 0 then 0 else 1 end)
			  + (case S03 when 0 then 0 else 1 end)
			  + (case S04 when 0 then 0 else 1 end)
			  + (case S05 when 0 then 0 else 1 end)
 where TypeOfGoods = 'L' and IsSubCon = '1'

select
 row_number() over (order by TaskPartSeq) SeqNo
,TaskPartSeq
,BillType
,BillTypeDesc
,TypeOfGoods
,TypeOfGoodsDesc
,case isnull(TypeOfGoods, '') when 'L' then 'L' else '0' end ItemType
,TaskPartNo
,OprHourDemandQty
,SupplyQty
,ReturnQty
,OprRetailPrice
,OprRetailPriceTotal
,isnull(SupplySlipNo, '')SupplySlipNo
,TaskPartDesc
,Status
,StatusDesc = 
 case IsSubCon
	when 0 then
		 case TypeOfGoods 
			when 'L' then
				case Status
					when '0' then '0 - Open Task'
					when '1' then '1 - Work In Progress'
					when '2' then '2 - Finish Task'
				end
			else
				case Status
					when '1' then '1 - Entry Stock'
					when '2' then '2 - Alokasi Stock'
					when '3' then '3 - Generate PL'
					when '4' then '4 - Generate Bill'
					when '5' then '5 - Lampiran'
					when '6' then '6 - Print Lampiran'
				end
		 end	
	else
		case Status
			when '1' then '1 - Draft PO'
			when '2' then '2 - Generate PO'
			when '3' then '3 - Draft Receiving'
			when '4' then '4 - Cancel Receiving'
			when '5' then '5 - Receiving PO'
		end
 end
,QtyAvail
,(case when (SupplyQty > 0) then (SupplyQty - ReturnQty) else OprHourDemandQty end) * OprRetailPrice Price
,DiscPct
,OprRetailPriceTotal as PriceNet
,IsSubCon
,TaskStatus
,@ServiceNo ServiceNo
from #t1

drop table #t1

end

