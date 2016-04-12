
-- =============================================
-- Author:		fhy
-- Create date: 20012016
-- Description:	sp sparepartanalisis bulanan for insert manual
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spAnalisisBulananLoad]
	@CompanyCode varchar(15),
	@BranchCodes  varchar(15)='',
	@TypeOfGoods varchar(15)='',
	@PeriodYear  int,
	@Month       int,
	@UserID      varchar(20)

--declare
--	@CompanyCode varchar(15)='6006400001',
--	@BranchCodes  varchar(15)='',
--	@PeriodYear  int=2015,
--	@Month       int = 4,
--	@UserID      varchar(20)='system'


AS
BEGIN


--AA TotalJaringan
declare @JumlahJaringan numeric(18, 4) -- Total Jariangan

--C. Harga Pokok (HPP)
declare @C_01 numeric(18, 4) -- Workshop HPP
declare @C_02 numeric(18, 4) -- Counter HPP
declare @C_03 numeric(18, 4) -- Partshop HPP
declare @C_04 numeric(18, 4) -- SubDealer HPP
declare @C_05 numeric(18, 4) -- Total HPP

--E. Nilai Stock
declare @E_01 numeric(18, 4) -- Nilai Stock

--F. ITO
declare @F_01 numeric(18, 4) -- ITO

-- J. Moving Code 0
declare @J_01 numeric(18, 4) -- Ammount Moving Code 0
declare @J_02 numeric(18, 4) -- Qty Moving Code 0

-- K. Moving Code 1
declare @K_01 numeric(18, 4) -- Ammount Moving Code 1
declare @K_02 numeric(18, 4) -- Qty Moving Code 1

-- L. Moving Code 2
declare @L_01 numeric(18, 4) -- Ammount Moving Code 2
declare @L_02 numeric(18, 4) -- Qty Moving Code 2

-- M. Moving Code 3
declare @M_01 numeric(18, 4) -- Ammount Moving Code 3
declare @M_02 numeric(18, 4) -- Qty Moving Code 3

-- N. Moving Code 4
declare @N_01 numeric(18, 4) -- Ammount Moving Code 4
declare @N_02 numeric(18, 4) -- Qty Moving Code 4

-- O. Moving Code 5
declare @O_01 numeric(18, 4) -- Ammount Moving Code 5
declare @O_02 numeric(18, 4) -- Qty Moving Code 5

-- P. Slow Moving
declare @P_01 numeric(18, 4) -- Slow Moving

-- Q. Lead Time Order (hari)
declare @Q_01 numeric(18, 4) -- Reguler
declare @Q_02 numeric(18, 4) -- Emergency

declare @t_branch as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	seqno int
)

insert into @t_branch
select companycode,branchcode,row_number() over (order by branchcode) seqno 
from gnMstOrganizationDtl
where BranchCode like case when @BranchCodes='' then '%%' else @BranchCodes end

declare @BranchCode  varchar(15)
declare @count int
declare @flagno int

set @count = (select count(*) from @t_branch)
set @flagno=1


while (@flagno <= @count)
begin

set @BranchCode = (select branchcode from @t_branch where seqno=@flagno)

-- Jumlah Jaringan
set @JumlahJaringan = isnull((select TotalJaringan from spMstSalesTarget
                                where CompanyCode = @CompanyCode
								and BranchCode  = @BranchCode
								and Year        = @PeriodYear
								and Month       = @Month),0)
if  @JumlahJaringan = 0 
set @JumlahJaringan = isnull((select isnull(count(*),0) from gnMstOrganizationDtl
							 where CompanyCode = @CompanyCode),0)

-- declare tbl t_supply_bengkel
declare @t_supply_bengkel as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	SrvSupLine   numeric(18, 4),
	SrvSupQty   numeric(18, 4),
	SrvSupGross   numeric(18, 4),
	SrvSupNilai   numeric(18, 4),
	SrvSupCost   numeric(18, 4)
)

-- declare tbl --t_supply
declare @t_supply as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	SupLine	 numeric(18, 4),
	SupQty   numeric(18, 4),
	SupGross   numeric(18, 4),
	SupNilai   numeric(18, 4),
	SupCost   numeric(18, 4)
)

-- declare tbl --t_supply
declare @t_return as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	ReturGross   numeric(18, 4),
	ReturNet   numeric(18, 4),
	ReturCost   numeric(18, 4)
)

-- declare tbl t_return_bengkel
declare @t_return_bengkel as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	SrvReturGross   numeric(18, 4),
	SrvReturNet   numeric(18, 4),
	SrvReturCost   numeric(18, 4)
)

-- declare tbl t_return_ss
declare @t_return_ss as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	SrvReturSSGross   numeric(18, 4),
	SrvReturSSNet   numeric(18, 4),
	SrvReturSSCost   numeric(18, 4)
)

-- declare tbl t_penerimaanpemb
declare @t_penerimaanpemb as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	PenerimaanPemb   numeric(18, 4)
)

-- declare tbl t_datastock
declare @t_datastock as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	NilaiStock      numeric(18, 4),
	NilaiStockMC0   numeric(18, 4),
	QtyStockMC0		numeric(18, 4),
	NilaiStockMC1   numeric(18, 4),
	QtyStockMC1		numeric(18, 4),
	NilaiStockMC2   numeric(18, 4),
	QtyStockMC2		numeric(18, 4),
	NilaiStockMC3   numeric(18, 4),
	QtyStockMC3		numeric(18, 4),
	NilaiStockMC4   numeric(18, 4),
	QtyStockMC4		numeric(18, 4),
	NilaiStockMC5   numeric(18, 4),
	QtyStockMC5		numeric(18, 4)
)

-- declare tbl t_demand
declare @t_demand as table
(
	CompanyCode varchar(25),
	BranchCode   varchar(25),
	TypeOfGoods varchar(15),
	CategoryCode varchar(15),
	DmnLine   numeric(18, 4),
	DmnQty   numeric(18, 4),
	DmnNilai   numeric(18, 4)
)

--t_supply_bengkel
insert into @t_supply_bengkel
select  
a.CompanyCode,a.BranchCode,d.TypeOfGoods,e.CategoryCode,
count(b.LmpNo)                          SrvSupLine, 
sum(isnull(b.QtyBill,0))                       SrvSupQty,
sum(isnull((b.QtyBill*b.retailprice),0))                      SrvSupGross,
sum(isnull(((b.QtyBill*b.retailprice)-(b.QtyBill*b.RetailPrice*b.DiscPct*0.01)),0))                   SrvSupNilai,
sum(isnull(b.CostPrice,0)*isnull(b.QtyBill,0)) SrvSupCost
from spTrnSLmpHdr a,  spTrnSLmpDtl b, spTrnSORDHdr c,spmstitems d,gnmstcustomer e
where a.CompanyCode    = b.CompanyCode 
and a.BranchCode     = b.branchCode 
and a.LmpNo          = b.LmpNo
and a.CompanyCode    = c.CompanyCode
and a.BranchCode     = c.BranchCode
and b.DocNo          = c.DocNo
and c.SalesType      = '2'
and b.companycode=d.companycode
and b.branchcode = d.branchcode
and b.partno=d.partno
and a.companycode = e.companycode
and a.Customercode = e.customercode
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.LmpDate)  = @PeriodYear
and month(a.LmpDate) = @Month 
group by a.CompanyCode,a.BranchCode,d.TypeOfGoods,e.CategoryCode

--t_supply
insert into @t_supply
select 
a.CompanyCode,a.BranchCode,d.TypeOfGoods,c.CategoryCode,
count(b.FPJNo) SupLine, 
sum(isnull(b.QtyBill,0))                       SupQty, 
sum(isnull((b.QtyBill*b.RetailPrice),0))                      SupGross, 
sum(isnull(((b.QtyBill*b.RetailPrice)-(b.QtyBill*b.RetailPrice*b.DiscPct*0.01)),0))                   SupNilai,
sum(isnull(b.CostPrice,0)*isnull(b.QtyBill,0)) SupCost
from spTrnSFPJHdr a,  spTrnSFPJDtl b, gnmstcustomer c,spmstitems d
where a.CompanyCode    = b.CompanyCode
and a.BranchCode     = b.branchCode 
and a.FPJNo          = b.FPJNo
and a.companycode= c.companycode
and a.Customercode = c.customercode
and b.CompanyCode=d.CompanyCode
and b.BranchCode=d.BranchCode
and b.PartNo=d.PartNo
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.FPJDate)  = @PeriodYear
and month(a.FPJDate) = @Month
group by a.CompanyCode,a.BranchCode,d.TypeOfGoods,c.CategoryCode

--t_return
insert into @t_return
select 
a.companycode,a.branchcode,c.typeOfGoods,d.CategoryCode,
isnull(sum(b.QtyReturn * b.RetailPrice),0) ReturGross, 
isnull(sum(((b.QtyReturn * b.RetailPrice)-(b.QtyReturn * b.RetailPrice*b.DiscPct*0.01))),0)   ReturNet, 
isnull(sum(b.QtyReturn*b.CostPrice),0)  ReturCost
from spTrnSRturHdr a,spTrnSRturDtl b,spmstitems c,gnmstcustomer d
where 
a.companycode=b.companycode
and a.branchcode= b.branchcode
and a.returnNo=b.ReturnNo
and b.companycode = c.companycode
and b.branchcode=c.companycode
and b.partno=c.partno
and a.companycode = d.companycode
and a.CompanyCode=d.CompanyCode
and a.Customercode=d.Customercode
and a.CompanyCode       = @CompanyCode
and a.BranchCode        = @BranchCode 
and year(a.ReturnDate)  = @PeriodYear
and month(a.ReturnDate) = @Month
and a.Status            = 2
group by a.companycode,a.branchcode,c.typeOfGoods,d.CategoryCode

--t_return_bengkel
insert into @t_return_bengkel
select 
a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode,
sum(isnull((b.QtyReturn*b.Retailprice),0))  SrvReturGross, 
sum(isnull(((b.QtyReturn*b.Retailprice)-((b.QtyReturn*b.Retailprice*b.DiscPct*0.01))),0))      SrvReturNet,
sum(isnull(b.CostPrice,0)*isnull(b.QtyReturn,0)) SrvReturCost
from spTrnSRturSrvHdr a, spTrnSRturSrvDtl b,spmstitems c,gnmstcustomer d
where a.CompanyCode       = b.CompanyCode 
and a.BranchCode        = b.BranchCode 
and a.ReturnNo          = b.ReturnNo
and b.CompanyCode=c.CompanyCode
and b.BranchCode=c.BranchCode
and b.PartNo = c.PartNo
and a.CompanyCode=d.CompanyCode
and a.Customercode=d.Customercode
and a.CompanyCode       = @CompanyCode
and a.BranchCode        = @BranchCode 
and year(a.ReturnDate)  = @PeriodYear
and month(a.ReturnDate) = @Month
and a.Status            = 2
group by a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode

--t_return_ss
insert into @t_return_ss
select 
a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode,
sum(isnull(b.QtyReturn*b.RetailPrice,0)) SrvReturSSGross, 
sum(isnull(((b.QtyReturn*b.RetailPrice)-(b.QtyReturn*b.RetailPrice*b.DiscPct*0.01)),0))   SrvReturSSNet, 
sum(isnull((b.QtyReturn*b.costPrice),0))  SrvReturSSCost
 from spTrnSRturSSHdr a,spTrnSRturSSDtl b,spmstitems c,gnmstcustomer d
 where 
 a.companycode=b.companycode
 and a.branchcode=b.branchcode
 and a.ReturnNo=b.ReturnNo
 and b.companycode=c.CompanyCode
 and b.BranchCode=c.BranchCode
 and b.PartNo = c.PartNo
 and a.CompanyCode=d.CompanyCode
 and a.Customercode=d.Customercode
 and a.CompanyCode       = @CompanyCode
 and a.BranchCode        = @BranchCode 
 and year(a.ReturnDate)  = @PeriodYear
 and month(a.ReturnDate) = @Month
 and a.Status            = 2
 group by a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode

 -- Workshop HPP
set @C_01=(select ISNULL(sum(SrvSupCost),0) -((select isnull(sum(ReturCost),0) from @t_return)
			+(select isnull(sum(SrvReturCost),0) from @t_return_bengkel) 
			+ (select isnull(sum(SrvReturSSCost),0) from @t_return_ss)) from @t_supply_bengkel)

-- Counter HPP
set @C_02= (select sum(SupCost) from @t_supply where CategoryCode not in ('00','01','15'))

-- Partshop HPP
set @C_03= (select sum(SupCost) from @t_supply where CategoryCode ='15')

-- SubDealer HPP
set @C_04= (select sum(SupCost) from @t_supply where CategoryCode in ('00','01'))

-- Total HPP
set @C_05= @C_01+@C_02+@C_03+@C_04

  
--D. Penerimaan Pembelian
insert into @t_penerimaanpemb
select a.companycode,a.branchcode,d.TypeOfGoods,sum(isnull(a.TotWRSAmt,0)) as PenerimaanPemb
from spTrnPRcvHdr a,  gnMstSupplier b, spTrnPRcvDtl c,spmstitems d
where a.CompanyCode    = b.CompanyCode
and a.SupplierCode   = b.SupplierCode
and a.companycode=c.companycode
and a.branchcode=c.branchcode
and c.companycode=d.companycode
and c.branchcode=d.branchcode
and c.partno=d.partno
and a.wrsno=c.wrsno
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.WRSDate)  = @PeriodYear
and month(a.WRSDate) = @Month
and b.StandardCode   = '1000000'
and a.Status         = 4
group by a.companycode,a.branchcode,d.TypeOfGoods

--E. Nilai Stock
insert into @t_datastock
select companycode,branchcode,typeofgoods,sum(isnull(OnHandQty * CostPrice,0)) NilaiStock,
sum(isnull(OnHandQty * (case MovingCode when '0' then CostPrice else 0 end ),0)) NilaiStockMC0,
sum(isnull((case MovingCode when '0' then OnHandQty else 0 end ),0)) QtyStockMC0,
sum(isnull(OnHandQty * (case MovingCode when '1' then CostPrice else 0 end ),0)) NilaiStockMC1,
sum(isnull((case MovingCode when '1' then OnHandQty else 0 end ),0)) QtyStockMC1,
sum(isnull(OnHandQty * (case MovingCode when '2' then CostPrice else 0 end ),0)) NilaiStockMC2,
sum(isnull((case MovingCode when '2' then OnHandQty else 0 end ),0)) QtyStockMC2,
sum(isnull(OnHandQty * (case MovingCode when '3' then CostPrice else 0 end ),0)) NilaiStockMC3,
sum(isnull((case MovingCode when '3' then OnHandQty else 0 end ),0)) QtyStockMC3,
sum(isnull(OnHandQty * (case MovingCode when '4' then CostPrice else 0 end ),0)) NilaiStockMC4,
sum(isnull((case MovingCode when '4' then OnHandQty else 0 end ),0)) QtyStockMC4,
sum(isnull(OnHandQty * (case MovingCode when '5' then CostPrice else 0 end ),0)) NilaiStockMC5,
sum(isnull((case MovingCode when '5' then OnHandQty else 0 end ),0)) QtyStockMC5
from spHstTransaction
where CompanyCode   = @CompanyCode
and BranchCode    = @BranchCode
and Year          = @PeriodYear
and Month         = @Month
and WarehouseCode between '00' and '88'
group by companycode,branchcode,typeofgoods


-- Nilai Stock
set @E_01 = (select isnull(sum(nilaiStock),0) from @t_datastock)

--F. ITO
SET @F_01=(@E_01/@C_05)

if isnull(@C_05,0) = 0
	set @F_01 = 0
else
	set @F_01 = @E_01 / @C_05

--G. Demand
insert into @t_demand
select 
a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode,
count(b.DocNo)  DmnLine, 
sum(isnull(b.QtyOrder,0))    DmnQty, 
sum(isnull(((b.QtyBill*b.RetailPrice)-(b.QtyBill*b.RetailPrice*b.DiscPct*0.01)),0)) DmnNilai
from spTrnSORDHdr a,  spTrnSORDDtl b,spmstitems c,gnmstcustomer d
where a.CompanyCode    = b.CompanyCode 
and a.BranchCode     = b.branchCode 
and a.DocNo          = b.DocNo
and b.companycode=c.companycode
and b.branchcode = c.branchcode
and b.partno=c.partno
 and a.CompanyCode=d.CompanyCode
 and a.Customercode=d.Customercode
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.DocDate)  = @PeriodYear
and month(a.DocDate) = @Month
and a.SalesType     in ('0','2')             -- 0:Sales Sparepart, 1:Internal, 2:Service
group by a.companycode,a.branchcode,c.TypeOfGoods,d.CategoryCode

-- J. Moving Code 0
-- Ammount Moving Code 0
set @J_01 = (select isnull(sum(NilaiStockMC0),0) from @t_datastock )
-- Qty Moving Code 0
set @J_02 = (select isnull(sum(QtyStockMC0),0) from @t_datastock )

-- Ammount Moving Code 1
set @K_01 = (select isnull(sum(NilaiStockMC1),0) from @t_datastock )
-- Qty Moving Code 1
set @K_02 = (select isnull(sum(QtyStockMC1),0) from @t_datastock )

-- Ammount Moving Code 2
set @L_01 = (select isnull(sum(NilaiStockMC2),0) from @t_datastock )
-- Qty Moving Code 2
set @L_02 = (select isnull(sum(QtyStockMC2),0) from @t_datastock )

-- Ammount Moving Code 3
set @M_01 = (select isnull(sum(NilaiStockMC3),0) from @t_datastock )
-- Qty Moving Code 3
set @M_02 = (select isnull(sum(QtyStockMC3),0) from @t_datastock )

-- Ammount Moving Code 4
set @N_01 = (select isnull(sum(NilaiStockMC4),0) from @t_datastock )
-- Qty Moving Code 4
set @N_02 = (select isnull(sum(QtyStockMC4),0) from @t_datastock )

-- Ammount Moving Code 5
set @O_01 = (select isnull(sum(NilaiStockMC5),0) from @t_datastock )
-- Qty Moving Code 5
set @O_02 = (select isnull(sum(QtyStockMC5),0) from @t_datastock )

-- P. Slow Moving
-- Slow Moving
set @P_01=@M_01+@N_01+@O_01

delete from [dbo].[spHstSparePartMonthly] 
where companycode=@CompanyCode
and branchcode=@BranchCode
and PeriodYear=@PeriodYear
and PeriodMonth=@Month

insert into [dbo].[spHstSparePartMonthly] 
select
	isnull(ts.CompanyCode,@CompanyCode) CompanyCode ,isnull(ts.BranchCode,@BranchCode) BranchCode,@PeriodYear as PeriodYear,@Month as PeriodMonth
	, isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(ts.TypeOfGoods,ts1.TypeOfGoods),ts2.TypeOfGoods),tsb.TypeOfGoods),tr.TypeOfGoods),trb.TypeOfGoods),trss.TypeOfGoods),tpb.TypeOfGoods),tds.TypeOfGoods),td.TypeOfGoods),ts3.TypeOfGoods) TypeOfGoods
	,@JumlahJaringan as JumlahJaringan
	, sum((isnull(tsb.SrvSupGross,0)) - ((isnull(tr.ReturGross,0))+(isnull(trb.SrvReturGross,0))+(ISNULL(trss.SrvReturSSGross,0)))) Workshop_PK
	, sum(isnull(ts1.SupGross,0)) Counter_PK, sum(isnull(ts2.SupGross,0)) Partshop_PK,sum(isnull(ts3.SupGross,0)) Subdealer_PK
	, sum((isnull(tsb.SrvSupNilai,0)) - ((isnull(tr.ReturNet,0))+(isnull(trb.SrvReturNet,0))+(ISNULL(trss.SrvReturSSNet,0)))) Workshop_PB
	, sum(isnull(ts1.SupNilai,0)) Counter_PB, sum(isnull(ts2.SupNilai,0)) Partshop_PB,sum(isnull(ts3.SupNilai,0)) Subdealer_PB
	, sum((isnull(tsb.SrvSupCost,0)) - ((isnull(tr.ReturCost,0))+(isnull(trb.SrvReturCost,0))+(ISNULL(trss.SrvReturSSCost,0)))) Workshop_HPP
	, sum(isnull(ts1.SupCost,0)) Counter_HPP, sum(isnull(ts2.SupCost,0)) Partshop_HPP,sum(isnull(ts3.SupCost,0)) Subdealer_HPP
	, sum((isnull(tsb.SrvSupCost,0)) - ((isnull(tr.ReturCost,0))+(isnull(trb.SrvReturCost,0))+(ISNULL(trss.SrvReturSSCost,0))) + (isnull(ts1.SupCost,0)) +(isnull(ts2.SupCost,0)) + 0) Total_HPP
	, sum(isnull(PenerimaanPemb,0)) PenerimaanPemb
	, @E_01 NilaiStock, @F_01 ITO
	, sum(ISNULL(td.DmnLine,0)) DmnLine, sum(ISNULL(td.DmnQty,0)) DmnQty, sum(ISNULL(td.DmnNilai,0)) DmnNilai
	, sum((isnull(ts.SupLine,0)) + (isnull(tsb.SrvSupLine,0))) SupplyLine
	, sum((isnull(ts.SupQty,0)) + (isnull(tsb.SrvSupQty,0))) SupplyQty
	, sum((isnull(ts.SupNilai,0)) + (isnull(tsb.SrvSupNilai,0))) SupplyNilai
	, case when sum(ISNULL(td.DmnLine,0))=0 then 0 else (((sum(isnull(ts.SupLine,0))) + (sum(isnull(tsb.SrvSupLine,0))))/(sum(ISNULL(td.DmnLine,0)))) * 100 end SrvRatioLine
	, case when sum(ISNULL(td.DmnQty,0))=0 then 0 else (((sum(isnull(ts.SupQty,0))) + (sum(isnull(tsb.SrvSupQty,0))))/(sum(ISNULL(td.DmnQty,0)))) * 100 end SrvRatioQty
	, case when sum(ISNULL(td.DmnNilai,0))=0 then 0 else (((sum(isnull(ts.SupNilai,0))) + (sum(isnull(tsb.SrvSupNilai,0))))/(sum(ISNULL(td.DmnNilai,0)))) * 100 end SrvRatioNilai
	, @J_01 AmtMC0,@J_02 QtyMC0,@K_01 AmtMC1,@K_02 QtyMC1, @L_01 AmtMC2,@L_02 QtyMC2, @M_01 AmtMC3,@M_02 QtyMC3, @N_01 AmtMC4,@N_02 QtyMC4, @o_01 AmtMC5,@O_02 QtyMC5,@P_01 SlowMoving
	, 0 LT_Reguler,0 LT_Emergency
	, @UserID CreatedBy
	, getdate() CreatedDate
	, @UserID CreatedBy
	, getdate() CreatedDate
from  @t_supply ts 
left join @t_supply ts1 on ts1.Companycode=ts.companycode and ts1.branchcode=ts.branchcode 
	and ts1.typeofgoods= ts.typeofgoods and ts1.CategoryCode=ts.CategoryCode and ts1.CategoryCode not in ('00','01','15')
left join @t_supply ts2 on ts2.Companycode=ts.companycode and ts2.branchcode=ts.branchcode 
	and ts2.typeofgoods= ts.typeofgoods and ts2.CategoryCode=ts.CategoryCode and ts2.CategoryCode ='15'
left join @t_supply ts3 on ts3.Companycode=ts.companycode and ts3.branchcode=ts.branchcode 
	and ts3.typeofgoods= ts.typeofgoods and ts3.CategoryCode=ts.CategoryCode and ts3.CategoryCode in ('00','01')
full outer join @t_supply_bengkel tsb on tsb.Companycode=ts.companycode 
	and tsb.branchcode=ts.branchcode and tsb.typeofgoods= ts.typeofgoods and tsb.CategoryCode=ts.CategoryCode
full outer join @t_return tr on tr.companycode=ts.companycode 
	and tr.branchcode = ts.branchcode and tr.TypeOfGoods=ts.TypeOfGoods and tr.CategoryCode=ts.CategoryCode
full outer join @t_return_bengkel trb on trb.companycode = ts.companycode
	and trb.branchcode = ts.branchcode and trb.typeofgoods =ts.typeofgoods and trb.CategoryCode=ts.CategoryCode
full outer join @t_return_ss trss on trss.companycode = ts.companycode
	and trss.branchcode = ts.branchcode and trss.typeofgoods =ts.typeofgoods and trss.CategoryCode = ts.CategoryCode
full outer join @t_penerimaanpemb tpb on tpb.companycode = ts.companycode
	and tpb.branchcode = ts.branchcode and tpb.typeofgoods =ts.typeofgoods
full outer join @t_datastock tds on tds.companycode = ts.companycode
	and tds.branchcode = ts.branchcode and tds.typeofgoods =ts.typeofgoods
full outer join @t_demand td on td.companycode = ts.companycode
	and td.branchcode = ts.branchcode and td.typeofgoods =ts.typeofgoods and td.CategoryCode = ts.CategoryCode
group by isnull(ts.CompanyCode,@CompanyCode),isnull(ts.BranchCode,@BranchCode), isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(isnull(ts.TypeOfGoods,ts1.TypeOfGoods),ts2.TypeOfGoods),tsb.TypeOfGoods),tr.TypeOfGoods),trb.TypeOfGoods),trss.TypeOfGoods),tpb.TypeOfGoods),tds.TypeOfGoods),td.TypeOfGoods),ts3.TypeOfGoods)

delete @t_supply
delete @t_supply_bengkel
delete @t_return
delete @t_return_bengkel
delete @t_return_ss
delete @t_penerimaanpemb
delete @t_datastock
delete @t_demand

set @flagno= @flagno+1

end

exec uspfn_spAnalisisBulananViewgrid @CompanyCode,@BranchCodes,@TypeOfGoods,@PeriodYear,@Month,@UserID

delete @t_branch

END



GO


