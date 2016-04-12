alter procedure usprpt_HrUpdateGnEmployee
	@CompanyCode varchar(50),
	@EmployeeID  varchar(50)

as

select * into #t1 from (
select top 1 * from HrEmployeeMutation
 where CompanyCode = @CompanyCode
   and EmployeeID = @EmployeeID
 order by MutationDate desc
)#t1

if(exists (select * from #t1))
begin

	declare @BranchCode varchar(20)
	set @BranchCode = (select top 1 BranchCode from #t1)
	 
	select * into #t2 from (
	select a.CompanyCode
		 , BranchCode = @BranchCode
		 , a.EmployeeID
		 , a.EmployeeName
		 , a.Address1
		 , a.Address2
		 , a.Address3
		 , a.Address4
		 , a.Handphone1 as PhoneNo
		 , a.Handphone2 as HpNo
		 , ProvinceCode = ''
		 , AreaCode = ''
		 , CityCode = ''
		 , a.ZipCode
		 , TitleCode = ''
		 , a.JoinDate
		 , a.ResignDate
		 , a.Gender as GenderCode
		 , a.BirthPlace
		 , a.BirthDate
		 , a.MaritalStatusCode
		 , a.Religion as ReligionCode
		 , a.BloodCode
		 , a.IdentityNo
		 , a.Height
		 , a.Weight
		 , a.UniformSize
		 , a.ShoesSize
		 , a.FormalEducation
		 , a.PersonnelStatus
		 , 0 as IsLocked
		 , a.CreatedBy
		 , a.CreatedDate
		 , a.UpdatedBy
		 , a.UpdatedDate
	  from HrEmployee a
	 where a.CompanyCode = @CompanyCode
	   and a.EmployeeID = @EmployeeID
	)#t2
	
	
	if (not exists (select * from GnMstEmployee where CompanyCode = @CompanyCode and BranchCode = @BranchCode and EmployeeID = @EmployeeID))
	begin
		select * from #t2
		select CompanyCode
		     , BranchCode
			 , EmployeeID
			 , EmployeeName
			 , Address1
			 , Address2
			 , Address3
			 , Address4
			 , PhoneNo
			 , HpNo
			 , '' FaxNo
			 , ProvinceCode
			 , AreaCode
			 , CityCode
			 , ZipCode as ZipNo
			 , TitleCode
			 , JoinDate
			 , ResignDate
			 , GenderCode
			 , BirthPlace
			 , BirthDate
			 , MaritalStatusCode
			 , ReligionCode
			 , BloodCode
			 , IdentityNo
			 , Height
			 , Weight
			 , UniformSize
			 , ShoesSize
			 , FormalEducation
			 , PersonnelStatus
			 , IsLocked
			 , CreatedBy
			 , CreatedDate
			 , UpdatedBy as LastupdateBy
			 , UpdatedBy as LastupdateDate
			 , EmployeeID Nik
		  from #t2
	end

end

go 

exec usprpt_HrUpdateGnEmployee '6006406', '00032'