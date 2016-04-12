ALTER procedure [dbo].[uspfn_gnInquiryBtn]
	@Area			varchar(100),
	@Dealer			varchar(100),
	@Outlet			varchar(100),
	@Detail			int 
AS
Begin
declare @National varchar(10);
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

Declare @TempTable table
(
	GroupNo		varchar(100),
	Area		varchar(100),
	DealerCode	varchar(100),
	DealerName	varchar(100),
	OutletCode	varchar(100),
	OutletName	varchar(100)
)
insert into @TempTable
select '000','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->'

	if (@Detail = 1)
	begin
		insert into @TempTable
		select distinct a.GroupNo, a.Area ,'' [1],'' [2],'' [3],'' [4]
		from gnMstDealerMapping a
		where a.DealerCode like case when @Dealer <> '' then @Dealer else '%%' end
--		  and a.GroupNo <> '100'
		  and a.isActive = 1
			order by a.groupNo asc
	end
	else if(@Detail = 2)
	begin
		insert into @TempTable
		select distinct a.GroupNo, a.Area, a.DealerCode, a.DealerName ,'' [1],'' [2]
		from gnMstDealerMapping a
		where (a.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK')
										then 'JABODETABEK'
										else a.Area end
								else '%%' end
			or a.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK')
										then 'CABANG'
										else a.Area end
								else '%%' end)
			and a.DealerCode like case when @Dealer <> '' then @Dealer else '%%' end
			and (a.DealerCode <> isnull((select Top 1 DealerCode from GnMstDealerMapping where DealerAbbreviation = 'CAB' order By SeqNo desc),0)
			or a.DealerCode <> isnull((select Top 1 DealerCode from GnMstDealerMapping where DealerAbbreviation = 'MPS' order By SeqNo desc),0))
			and a.isActive = 1
		order by a.DealerCode
	end
	else if(@Detail = 3)
	begin
		insert into @TempTable
		select distinct  a.GroupNo, a.Area, a.DealerCode
			, a.DealerName, b.OutletCode, b.OutletName 
		from gnMstDealerMapping a
		left join gnMstDealerOutletMapping b on a.DealerCode = b.DealerCode
		--where a.DealerCode = @Dealer
		where (a.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or a.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		  and a.DealerCode like case when @Dealer <> '' then @Dealer else '%%' end
		  and a.isActive = 1
		order by b.OutletName
	end

select * from @TempTable
end
	