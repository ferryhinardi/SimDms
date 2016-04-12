
-- =============================================
-- Author:		fhy
-- Create date: 21012016
-- Description:	SP for view to grid Sparapart analisis bulanan
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spAnalisisBulananViewgrid] 
	@CompanyCode varchar(15),
	@BranchCodes  varchar(15)='',
	@TypeOfGoods varchar(15)='',
	@PeriodYear  int,
	@Month       int,
	@UserID      varchar(20)

	--declare
	--@CompanyCode varchar(15)='6006400001',
	--@BranchCodes  varchar(15)='',
	--@PeriodYear  int=2015,
	--@Month       int = 4,
	--@UserID      varchar(20)='system'
AS
BEGIN

declare @t_spHstSpMonthlyBranch as table(
	[CompanyCode] [varchar](15),
	[BranchCode] [varchar](15),
	[PeriodYear] [numeric](4, 0),
	[PeriodMonth] [numeric](2, 0),
	[JumlahJaringan] [numeric](9, 0),
	[Workshop_PK] [numeric](18, 2),
	[Counter_PK] [numeric](18, 2),
	[Partshop_PK] [numeric](18, 2),
	[SubDealer_PK] [numeric](18, 2),
	[Workshop_PB] [numeric](18, 2),
	[Counter_PB] [numeric](18, 2),
	[Partshop_PB] [numeric](18, 2),
	[SubDealer_PB] [numeric](18, 2),
	[Workshop_HPP] [numeric](18, 2),
	[Counter_HPP] [numeric](18, 2),
	[Partshop_HPP] [numeric](18, 2),
	[SubDealer_HPP] [numeric](18, 2),
	[Total_HPP] [numeric](18, 2),
	[Penerimaan_Pembelian] [numeric](18, 2),
	[Nilai_Stock] [numeric](18, 2),
	[ITO] [numeric](18, 2),
	[Line_Demand] [numeric](9, 2),
	[Quantity_Demand] [numeric](12, 2),
	[Nilai_Demand] [numeric](18, 2),
	[Line_Supply] [numeric](9, 2),
	[Quantity_Supply] [numeric](12, 2),
	[Nilai_Supply] [numeric](18, 2),
	[Line_Service_Ratio] [numeric](9, 2),
	[Quantity_Service_Ratio] [numeric](12, 2),
	[Nilai_Service_Ratio] [numeric](18, 2),
	[Ammount_MC0] [numeric](18, 2),
	[Qty_MC0] [numeric](18, 2),
	[Ammount_MC1] [numeric](18, 2),
	[Qty_MC1] [numeric](18, 2),
	[Ammount_MC2] [numeric](18, 2),
	[Qty_MC2] [numeric](18, 2) ,
	[Ammount_MC3] [numeric](18, 2) ,
	[Qty_MC3] [numeric](18, 2),
	[Ammount_MC4] [numeric](18, 2) ,
	[Qty_MC4] [numeric](18, 2),
	[Ammount_MC5] [numeric](18, 2),
	[Qty_MC5] [numeric](18, 2),
	[Slow_Moving] [numeric](18, 2),
	[LT_Reguler] [numeric](18, 2),
	[LT_Emergency] [numeric](18, 2)
)

declare @t_spHstSpMonthlyAllBranch as table(
	[CompanyCode] [varchar](15),
	[PeriodYear] [numeric](4, 0),
	[PeriodMonth] [numeric](2, 0),
	[JumlahJaringan] [numeric](9, 0),
	[Workshop_PK] [numeric](18, 2),
	[Counter_PK] [numeric](18, 2),
	[Partshop_PK] [numeric](18, 2),
	[SubDealer_PK] [numeric](18, 2),
	[Workshop_PB] [numeric](18, 2) ,
	[Counter_PB] [numeric](18, 2) ,
	[Partshop_PB] [numeric](18, 2) ,
	[SubDealer_PB] [numeric](18, 2) ,
	[Workshop_HPP] [numeric](18, 2) ,
	[Counter_HPP] [numeric](18, 2) ,
	[Partshop_HPP] [numeric](18, 2) ,
	[SubDealer_HPP] [numeric](18, 2) ,
	[Total_HPP] [numeric](18, 2) ,
	[Penerimaan_Pembelian] [numeric](18, 2) ,
	[Nilai_Stock] [numeric](18, 2) ,
	[ITO] [numeric](18, 2) ,
	[Line_Demand] [numeric](9, 2) ,
	[Quantity_Demand] [numeric](12, 2) ,
	[Nilai_Demand] [numeric](18, 2) ,
	[Line_Supply] [numeric](9, 2) ,
	[Quantity_Supply] [numeric](12, 2) ,
	[Nilai_Supply] [numeric](18, 2) ,
	[Line_Service_Ratio] [numeric](9, 2) ,
	[Quantity_Service_Ratio] [numeric](12, 2) ,
	[Nilai_Service_Ratio] [numeric](18, 2) ,
	[Ammount_MC0] [numeric](18, 2) ,
	[Qty_MC0] [numeric](18, 2) ,
	[Ammount_MC1] [numeric](18, 2) ,
	[Qty_MC1] [numeric](18, 2) ,
	[Ammount_MC2] [numeric](18, 2) ,
	[Qty_MC2] [numeric](18, 2) ,
	[Ammount_MC3] [numeric](18, 2) ,
	[Qty_MC3] [numeric](18, 2) ,
	[Ammount_MC4] [numeric](18, 2) ,
	[Qty_MC4] [numeric](18, 2) ,
	[Ammount_MC5] [numeric](18, 2) ,
	[Qty_MC5] [numeric](18, 2) ,
	[Slow_Moving] [numeric](18, 2) ,
	[LT_Reguler] [numeric](18, 2) ,
	[LT_Emergency] [numeric](18, 2) 
)

declare @t_AllBranch as table(
	[CompanyCode] [varchar](15),
	[BranchCode] [varchar](15)
)

declare @count int
declare @flag int
declare @counter int

declare @tbranch int

set @flag =1
set @count=12

insert into @t_AllBranch
select  COMPANYCODE,branchcode from spHstSparePartMonthly 
where CompanyCode=@CompanyCode and PeriodYear=@PeriodYear 
and TypeOfGoods like case when @TypeOfGoods ='' then TypeOfGoods else @TypeOfGoods end
group by CompanyCode,BranchCode

set @tbranch=(select count(*) from @t_AllBranch)

--select @tbranch

if (@BranchCodes='')
begin
	while (@flag <= @Month)
	begin

	set @counter =(select count(*) from spHstSparePartMonthly where companycode=@CompanyCode 
					and periodyear=@PeriodYear and periodmonth=@flag)

	if (@counter=0)
	begin
		insert into @t_spHstSpMonthlyAllBranch
		select @CompanyCode,@PeriodYear,@flag,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
	end
	else
	begin
		insert into @t_spHstSpMonthlyAllBranch
		select 
		 companycode,periodyear,periodmonth,jumlahjaringan 
		 , sum(workshop_pk) workshop_pk,sum(counter_pk) counter_pk,sum(partshop_pk) partshop_pk,sum(subdealer_pk) subdealer_pk
		 , sum(workshop_pb) workshop_pb,sum(counter_pb) counter_pb,sum(partshop_pb) partshop_pb,sum(subdealer_pb) subdealer_pb
		 , sum(workshop_hpp) workshop_hpp,sum(counter_hpp) counter_hpp,sum(partshop_hpp) partshop_hpp,sum(subdealer_hpp) subdealer_hpp,sum(Total_Hpp) Total_Hpp
		 , (sum(Penerimaan_Pembelian))/@tbranch Penerimaan_Pembelian,(sum(Nilai_Stock))/@tbranch Nilai_Stock,(sum(ITO))/@tbranch ITO
		 , sum(Line_Demand) Line_Demand,sum(Quantity_Demand) Quantity_Demand,sum(Nilai_Demand) Nilai_Demand
		 , sum(Line_Supply) Line_Supply,sum(Quantity_Supply) Quantity_Supply,sum(Nilai_Supply) Nilai_Supply
		 , case when (sum(Line_Demand)) = 0 then 0 else (((sum(Line_Supply))/ (sum(Line_Demand))))*100 end Line_Service_Ratio  --
		 , case when (sum(Quantity_Demand)) = 0 then 0 else (((sum(Quantity_Supply))/ (sum(Quantity_Demand)))) * 100 end Quantity_Service_Ratio 
		 , case when (sum(Nilai_Demand)) = 0 then 0 else (((sum(Nilai_Supply))/ (sum(Nilai_Demand)))) * 100 end Nilai_Service_Ratio
		 , (sum(Ammount_MC0))/@tbranch Ammount_MC0,(sum(Qty_MC0))/@tbranch Qty_MC0,(sum(Ammount_MC1))/@tbranch Ammount_MC1,(sum(Qty_MC1))/@tbranch Qty_MC1
		 , sum(Ammount_MC2)/@tbranch Ammount_MC2,sum(Qty_MC2)/@tbranch Qty_MC2,sum(Ammount_MC3)/@tbranch Ammount_MC3,sum(Qty_MC3)/@tbranch Qty_MC3
		 , sum(Ammount_MC4)/@tbranch Ammount_MC4,sum(Qty_MC4)/@tbranch Qty_MC4,sum(Ammount_MC5)/@tbranch Ammount_MC5,sum(Qty_MC5)/@tbranch Qty_MC5
		 , sum(Slow_Moving)/@tbranch Slow_Moving,sum(LT_Reguler)/@tbranch LT_Reguler,sum(LT_Emergency)/@tbranch LT_Emergency
		from spHstSparePartMonthly
		where companycode=@CompanyCode and periodyear=@PeriodYear and periodmonth=@flag
		and TypeOfGoods like case when @TypeOfGoods ='' then TypeOfGoods else @TypeOfGoods end
		group by companycode,periodyear,periodmonth,jumlahjaringan
		end

		set @flag = @flag+1

		end
		set @flag=@Month+1

		while (@flag <= @count)
		begin
			insert into @t_spHstSpMonthlyAllBranch
			select @CompanyCode,@PeriodYear,@flag,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
			set @flag = @flag+1
		end

	select case when PeriodMonth=1 then 'Januari'
			when PeriodMonth=2 then 'Februari'
			when PeriodMonth=3 then 'Maret'
			when PeriodMonth=4 then 'April'
			when PeriodMonth=5 then 'Mei'
			when PeriodMonth=6 then 'Juni'
			when PeriodMonth=7 then 'Juli'
			when PeriodMonth=8 then 'Agustus'
			when PeriodMonth=9 then 'September'
			when PeriodMonth=10 then 'Oktober'
			when PeriodMonth=11 then 'Nopember'
			when PeriodMonth=12 then 'Desember'
			end Bulan
			,* from @t_spHstSpMonthlyAllBranch

end
else
begin
	while (@flag <= @Month)
	begin

	set @counter =(select count(*) from spHstSparePartMonthly where companycode=@CompanyCode 
				and branchcode=@BranchCodes and periodyear=@PeriodYear and periodmonth=@flag)

	if (@counter=0)
	begin
		insert into @t_spHstSpMonthlyBranch
		select @CompanyCode,@BranchCodes,@PeriodYear,@flag,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
	end
	else
	begin
		insert into @t_spHstSpMonthlyBranch
		select 
		 companycode,branchcode,periodyear,periodmonth,jumlahjaringan 
		 , sum(workshop_pk) workshop_pk,sum(counter_pk) counter_pk,sum(partshop_pk) partshop_pk,sum(subdealer_pk) subdealer_pk
		 , sum(workshop_pb) workshop_pb,sum(counter_pb) counter_pb,sum(partshop_pb) partshop_pb,sum(subdealer_pb) subdealer_pb
		 , sum(workshop_hpp) workshop_hpp,sum(counter_hpp) counter_hpp,sum(partshop_hpp) partshop_hpp,sum(subdealer_hpp) subdealer_hpp,sum(Total_Hpp) Total_Hpp
		 , Penerimaan_Pembelian ,Nilai_Stock,ITO,sum(Line_Demand) Line_Demand,sum(Quantity_Demand) Quantity_Demand,sum(Nilai_Demand) Nilai_Demand
		 , sum(Line_Supply) Line_Supply,sum(Quantity_Supply) Quantity_Supply,sum(Nilai_Supply) Nilai_Supply
		 , case when (sum(Line_Demand)) = 0 then 0 else (((sum(Line_Supply))/ (sum(Line_Demand)))) * 100 end Line_Service_Ratio  --
		 , case when (sum(Quantity_Demand)) = 0 then 0 else (((sum(Quantity_Supply))/ (sum(Quantity_Demand)))) * 100 end Quantity_Service_Ratio 
		 , case when (sum(Nilai_Demand)) = 0 then 0 else (((sum(Nilai_Supply))/ (sum(Nilai_Demand)))) * 100 end Nilai_Service_Ratio
		 , Ammount_MC0,Qty_MC0,Ammount_MC1,Qty_MC1,Ammount_MC2,Qty_MC2,Ammount_MC3,Qty_MC3,Ammount_MC4,Qty_MC4,Ammount_MC5,Qty_MC5
		 , Slow_Moving,LT_Reguler,LT_Emergency
		from spHstSparePartMonthly
		where companycode=@CompanyCode 
		and branchcode=@BranchCodes and periodyear=@PeriodYear and periodmonth=@flag
		and TypeOfGoods like case when @TypeOfGoods ='' then TypeOfGoods else @TypeOfGoods end
		group by companycode,branchcode,periodyear,periodmonth,jumlahjaringan,Penerimaan_Pembelian,Nilai_Stock,ITO
			,Ammount_MC0,Qty_MC0,Ammount_MC1,Qty_MC1,Ammount_MC2,Qty_MC2,Ammount_MC3,Qty_MC3,Ammount_MC4,Qty_MC4
			,Ammount_MC5,Qty_MC5,Slow_Moving,LT_Reguler,LT_Emergency
		end

		set @flag = @flag+1

		end
		set @flag=@Month+1

		while (@flag <= @count)
		begin
			insert into @t_spHstSpMonthlyBranch
			select @CompanyCode,@BranchCodes,@PeriodYear,@flag,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
			set @flag = @flag+1
		end

	select 
		case when PeriodMonth=1 then 'Januari'
			when PeriodMonth=2 then 'Februari'
			when PeriodMonth=3 then 'Maret'
			when PeriodMonth=4 then 'April'
			when PeriodMonth=5 then 'Mei'
			when PeriodMonth=6 then 'Juni'
			when PeriodMonth=7 then 'Juli'
			when PeriodMonth=8 then 'Agustus'
			when PeriodMonth=9 then 'September'
			when PeriodMonth=10 then 'Oktober'
			when PeriodMonth=11 then 'Nopember'
			when PeriodMonth=12 then 'Desember'
			end Bulan
		,* from @t_spHstSpMonthlyBranch
end

delete @t_spHstSpMonthlyBranch
delete @t_spHstSpMonthlyAllBranch

END




GO


