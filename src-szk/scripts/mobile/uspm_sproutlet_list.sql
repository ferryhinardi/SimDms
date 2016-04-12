alter procedure uspm_sproutlet_list
	@DealerCode varchar(20)
as

declare @t_dealer as table(
	DealerCode varchar(20),
	DealerName varchar(90),
	ApiUrl  varchar(900)
)

select BranchCode as OutletCode, CompanyName as OutletName
  from GnMstCoProfile
 where CompanyCode = @DealerCode

go 

exec uspm_sproutlet_list '6021406'

