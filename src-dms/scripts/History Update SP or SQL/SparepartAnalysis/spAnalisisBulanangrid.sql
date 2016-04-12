
-- =============================================
-- Author:		fhy
-- Create date: 18012016
-- Description:	SP for Data Grid Sparepart Analisis Bulanan
-- =============================================
CREATE PROCEDURE  [dbo].[uspfn_spAnalisisBulanangrid]
		@CompanyCode varchar(15)='6006400001',
		@BranchCode  varchar(15)='6006400106',
		@ProductType varchar(15)='4w', 
		@PeriodYear  int=2015,
		@Month       int = 4,
		@UserID      varchar(20)='system'
AS
BEGIN

--AA TotalJaringan
declare @JumlahJaringan numeric(18, 4) -- Total Jariangan

--A. Penjualan Kotor
declare @A_01 numeric(18, 4) -- Workshop Penjualan Kotor
declare @A_02 numeric(18, 4) -- Counter Penjualan Kotor
declare @A_03 numeric(18, 4) -- Partshop Penjualan Kotor
declare @A_04 numeric(18, 4) -- SubDealer Penjualan Kotor

--B. Penjualan Bersih
declare @B_01 numeric(18, 4) -- Workshop Penjualan Bersih
declare @B_02 numeric(18, 4) -- Counter Penjualan Bersih
declare @B_03 numeric(18, 4) -- Partshop Penjualan Bersih
declare @B_04 numeric(18, 4) -- SubDealer Penjualan Bersih

--C. Harga Pokok (HPP)
declare @C_01 numeric(18, 4) -- Workshop HPP
declare @C_02 numeric(18, 4) -- Counter HPP
declare @C_03 numeric(18, 4) -- Partshop HPP
declare @C_04 numeric(18, 4) -- SubDealer HPP
declare @C_05 numeric(18, 4) -- Total HPP

--D. Penerimaan Pembelian
declare @D_01 numeric(18, 4) -- Penerimaan Pembelian

--E. Nilai Stock
declare @E_01 numeric(18, 4) -- Nilai Stock

--F. ITO
declare @F_01 numeric(18, 4) -- ITO

--G. Demand
declare @G_01 numeric(18, 4) -- Line Demand
declare @G_02 numeric(18, 4) -- Quantity Demand
declare @G_03 numeric(18, 4) -- Nilai Demand

--H. Supply
declare @H_01 numeric(18, 4) -- Line Supply
declare @H_02 numeric(18, 4) -- Quantity Supply
declare @H_03 numeric(18, 4) -- Nilai Supply

--I. Service Ratio
declare @I_01 numeric(18, 4) -- Line Service Ratio
declare @I_02 numeric(18, 4) -- Quantity Service Ratio
declare @I_03 numeric(18, 4) -- Nilai Service Ratio

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
	DmnLine   numeric(18, 4),
	DmnQty   numeric(18, 4),
	DmnNilai   numeric(18, 4)
)

--t_supply_bengkel
insert into @t_supply_bengkel
select  
a.CompanyCode,a.BranchCode,d.TypeOfGoods,
count(b.LmpNo)                          SrvSupLine, 
sum(isnull(b.QtyBill,0))                       SrvSupQty,
sum(isnull(b.SalesAmt,0))                      SrvSupGross,
sum(isnull(b.NetSalesAmt,0))                   SrvSupNilai,
sum(isnull(b.CostPrice,0)*isnull(b.QtyBill,0)) SrvSupCost
from spTrnSLmpHdr a,  spTrnSLmpDtl b, spTrnSORDHdr c,spmstitems d
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
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.LmpDate)  = @PeriodYear
and month(a.LmpDate) = @Month 
group by a.CompanyCode,a.BranchCode,d.TypeOfGoods

--t_supply
insert into @t_supply
select 
a.CompanyCode,a.BranchCode,d.TypeOfGoods,c.CategoryCode,
count(b.FPJNo) SupLine, 
sum(isnull(b.QtyBill,0))                       SupQty, 
sum(isnull(b.SalesAmt,0))                      SupGross, 
sum(isnull(b.NetSalesAmt,0))                   SupNilai, 
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
group by a.CompanyCode,a.BranchCode,c.CategoryCode,d.TypeOfGoods

--t_return
insert into @t_return
select 
a.companycode,a.branchcode,c.typeOfGoods,
isnull(sum(b.ReturAmt),0) ReturGross, 
isnull(sum(b.NetReturAmt),0)   ReturNet, 
isnull(sum(b.CostAmt),0)  ReturCost
from spTrnSRturHdr a,spTrnSRturDtl b,spmstitems c
where 
a.companycode=b.companycode
and a.branchcode= b.branchcode
and a.returnNo=b.ReturnNo
and b.companycode = c.companycode
and b.branchcode=c.companycode
and b.partno=c.partno
and a.CompanyCode       = @CompanyCode
and a.BranchCode        = @BranchCode 
and year(a.ReturnDate)  = @PeriodYear
and month(a.ReturnDate) = @Month
and a.Status            = 2
group by a.companycode,a.branchcode,c.typeOfGoods

--t_return_bengkel
insert into @t_return_bengkel
select 
a.companycode,a.branchcode,c.TypeOfGoods,
sum(isnull(b.ReturAmt,0))  SrvReturGross, 
sum(isnull(b.NetReturAmt,0))      SrvReturNet,
sum(isnull(b.CostPrice,0)*isnull(b.QtyReturn,0)) SrvReturCost
from spTrnSRturSrvHdr a, spTrnSRturSrvDtl b,spmstitems c
where a.CompanyCode       = b.CompanyCode 
and a.BranchCode        = b.BranchCode 
and a.ReturnNo          = b.ReturnNo
and b.CompanyCode=c.CompanyCode
and b.BranchCode=c.BranchCode
and b.PartNo = c.PartNo
and a.CompanyCode       = @CompanyCode
and a.BranchCode        = @BranchCode 
and year(a.ReturnDate)  = @PeriodYear
and month(a.ReturnDate) = @Month
and a.Status            = 2
group by a.companycode,a.branchcode,c.TypeOfGoods

--t_return_ss
insert into @t_return_ss
select 
a.companycode,a.branchcode,c.TypeOfGoods,
sum(b.ReturAmt) SrvReturSSGross, 
sum(b.NetReturAmt)   SrvReturSSNet, 
sum(b.CostAmt)  SrvReturSSCost
 from spTrnSRturSSHdr a,spTrnSRturSSDtl b,spmstitems c
 where 
 a.companycode=b.companycode
 and a.branchcode=b.branchcode
 and a.ReturnNo=b.ReturnNo
 and b.companycode=c.CompanyCode
 and b.BranchCode=c.BranchCode
 and b.PartNo = c.PartNo
 and a.CompanyCode       = @CompanyCode
 and a.BranchCode        = @BranchCode 
 and year(a.ReturnDate)  = @PeriodYear
 and month(a.ReturnDate) = @Month
 and a.Status            = 2
 group by a.companycode,a.branchcode,c.TypeOfGoods

--A. Penjualan Kotor
-- Workshop Penjualan Kotor
set @A_01 = (select ISNULL(sum(SrvSupGross),0) -((select isnull(sum(ReturGross),0) from @t_return)
			+(select isnull(sum(SrvReturGross),0) from @t_return_bengkel) 
			+ (select isnull(sum(SrvReturSSGross),0) from @t_return_ss)) from @t_supply_bengkel)

-- Counter Penjualan Kotor
set @A_02 = (select sum(SupGross) from @t_supply where CategoryCode !='15')

-- Partshop Penjualan Kotor
set @A_03 = (select sum(SupGross) from @t_supply where CategoryCode ='15')

-- SubDealer Penjualan Kotor
set @A_04 = 0


--B. Penjualan Bersih
-- Workshop Penjualan Bersih
set @B_01=(select ISNULL(sum(SrvSupNilai),0) -((select isnull(sum(ReturNet),0) from @t_return)
			+(select isnull(sum(SrvReturNet),0) from @t_return_bengkel) 
			+ (select isnull(sum(SrvReturSSNet),0) from @t_return_ss)) from @t_supply_bengkel)

-- Counter Penjualan Bersih
set @B_02= (select sum(SupNilai) from @t_supply where CategoryCode !='15')

-- Partshop Penjualan Bersih
set @B_03= (select sum(SupNilai) from @t_supply where CategoryCode ='15')

--SubDealer Penjualan Bersih
set @B_04=0

--C. Harga Pokok (HPP)
-- Workshop HPP
set @C_01=(select ISNULL(sum(SrvSupCost),0) -((select isnull(sum(ReturCost),0) from @t_return)
			+(select isnull(sum(SrvReturCost),0) from @t_return_bengkel) 
			+ (select isnull(sum(SrvReturSSCost),0) from @t_return_ss)) from @t_supply_bengkel)

-- Counter HPP
set @C_02= (select sum(SupCost) from @t_supply where CategoryCode !='15')

-- Partshop HPP
set @C_03= (select sum(SupCost) from @t_supply where CategoryCode ='15')

-- SubDealer HPP
set @C_04=0

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

set @D_01=(Select isnull(PenerimaanPemb,0) from @t_penerimaanpemb)

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
a.companycode,a.branchcode,c.TypeOfGoods,
count(b.DocNo)  DmnLine, 
sum(isnull(b.QtyOrder,0))    DmnQty, 
sum(isnull(b.NetSalesAmt,0)) DmnNilai
from spTrnSORDHdr a,  spTrnSORDDtl b,spmstitems c
where a.CompanyCode    = b.CompanyCode 
and a.BranchCode     = b.branchCode 
and a.DocNo          = b.DocNo
and b.companycode=c.companycode
and b.branchcode = c.branchcode
and b.partno=c.partno
and a.CompanyCode    = @CompanyCode
and a.BranchCode     = @BranchCode
and year(a.DocDate)  = @PeriodYear
and month(a.DocDate) = @Month
and a.SalesType     in ('0','2')             -- 0:Sales Sparepart, 1:Internal, 2:Service
group by a.companycode,a.branchcode,c.TypeOfGoods

-- Line Demand
set @G_01=(select sum(DmnLine) from @t_demand)

--Quantity Demand
set @G_02=(select sum(DmnQty) from @t_demand)

-- Nilai Demand
set @G_03=(select sum(DmnNilai) from @t_demand)

--H. Supply
-- Line Supply
set @H_01 =((select sum(SupLine) from @t_supply)+(select sum(SrvSupLine) from @t_supply_bengkel)) 

-- Quantity Supply
set @H_02 =((select sum(SupQty) from @t_supply)+(select sum(SrvSupQty) from @t_supply_bengkel)) 

-- Nilai Supply
set @H_03 =((select sum(SupNilai) from @t_supply)+(select sum(SrvSupNilai) from @t_supply_bengkel)) 

--I. Service Ratio
-- Line Service Ratio
set @I_01 = (@H_01/@G_01)*100

-- Quantity Service Ratio
set @I_02 = (@H_02/@G_02)*100

-- Nilai Service Ratio
set @I_03 = (@H_03/@G_03)*100

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

-- Q. Lead Time Order (hari)
-- Reguler
set @Q_01=0

-- Emergency
set @Q_02=0

--Execute Result Query
select 'April' Bulan, @JumlahJaringan [Jumlah_Jaringan],@A_01 [Workshop_PK],@A_02 [Counter_PK],@A_03 [Partshop_PK],@A_04 [SubDealer_PK],
	@B_01 [Workshop_PB],@B_02 [Counter_PB],@B_03 [Partshop_PB],@B_04 [SubDealer_PB],
	@C_01 [Workshop_HPP],@C_02 [Counter_HPP],@C_03 [Partshop_HPP],@C_04 [SubDealer_HPP],@C_05 [Total_HPP],
	isnull(@D_01,0) [Penerimaan_Pembelian],@E_01 [Nilai_Stock],@F_01 [ITO],
	@G_01 [Line_Demand],@G_02 [Quantity_Demand],@G_03 [Nilai_Demand],
	 @H_01 [Line_Supply],@H_02 [Quantity_Supply],@H_03 [Nilai_Supply],
	 @I_01 [Line_Service_Ratio],@I_02 [Quantity_Service_Ratio],@I_03 [Nilai_Service_Ratio],
	@J_01 [Ammount_MC0],@J_02 [Qty_MC0],
	@K_01 [Ammount_MC1],@K_02 [Qty_MC1],@L_01 [Ammount_MC2],@L_02 [Qty_MC2],@M_01 [Ammount_MC3],@M_02 [Qty_MC3],
	@N_01 [Ammount_MC4],@N_02 [Qty_MC4],@O_01 [Ammount_MC5],@O_02 [Qty_MC5],	
	 @P_01 [Slow_Moving],@Q_01 [LT_Reguler],@Q_02 [LT_Emergency]

--select * from @t_supply

delete @t_supply
delete @t_supply_bengkel
delete @t_return
delete @t_return_bengkel
delete @t_return_ss
delete @t_penerimaanpemb
delete @t_datastock
delete @t_demand


END


GO


