alter procedure uspm_sprdealer_list
as

declare @t_dealer as table(
	DealerCode varchar(20),
	DealerName varchar(90),
	ApiUrl  varchar(900)
)

insert into @t_dealer values ('6006406','SIM (cloud)','http://dms.suzuki.co.id/SimDms/MobileSpr/');
insert into @t_dealer values ('6006408','SIM (local)','http://tbsdmsap01/SimDms/MobileSpr/');
insert into @t_dealer values ('6021406','SIM (local-test)','http://localhost:7701/MobileSpr/');

select * from @t_dealer

go 

exec uspm_sprdealer_list
