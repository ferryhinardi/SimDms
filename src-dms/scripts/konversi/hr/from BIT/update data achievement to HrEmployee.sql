declare c cursor for (
	select 
		a.CompanyCode,
		a.EmployeeId
	from 
		HrEmployee a
	where
		a.EmployeeID in (
			select distinct
				x.EmployeeID
			from
				HrEmployeeAchievement  x
		)
)

declare @CompanyCode varchar(25);
declare @EmployeeID varchar(25);

open c
	fetch next from c into @CompanyCode, @EmployeeID

	while @@FETCH_STATUS = 0
	begin
		select @CompanyCode, @EmployeeID;	
		fetch next from c into @CompanyCode, @EmployeeID;
	end
close c
deallocate c