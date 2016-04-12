
go
if object_id('uspfn_CheckITSOrganization') is not null
	drop procedure uspfn_CheckITSOrganization

go
create procedure uspfn_CheckITSOrganization
	@CompanyCode varchar(17),
	@BranchCode varchar(17),
	@EmployeeID varchar(25)
as 

begin
	declare @Employee table (
		CompanyCode varchar(13),
		BranchCode varchar(13),
		EmployeeID varchar(17),
		EmployeeName varchar(100),
		Department varchar(35),
		Position varchar(10),
		PositionName varchar(100),
		Grade varchar(10),
		Leader varchar(17)
	);
	declare @LoopStatus bit;
	declare @CurrentEmployeeID varchar(17);
	declare @CurrentEmployeeName varchar(17);
	declare @CurrentDeparment varchar(17);
	declare @CurrentPosition varchar(17);
	declare @CurrentPositionName varchar(17);
	declare @CurrentLeader varchar(17);
	declare @PositionHeader varchar(25);
	declare @PositionHeaderName varchar(100);
	declare @Status bit;
	declare @Message varchar(200);
	declare @RecordCount int;
	declare @Iterator int;
	declare @GeneralManager varchar(17);
	declare @OperationManager varchar(17);
	declare @BranchManager varchar(17);
	declare @SalesHead varchar(17);
	declare @SalesCoordinator varchar(17);
	declare @Grade varchar(17);

	set @LoopStatus = 1;
	set @CurrentEmployeeID = @EmployeeID;
	set @Iterator = 1;
	set @Status = convert(bit, 0);

	while (@LoopStatus = convert(bit, 1)) 
	begin
		delete @Employee;

	    insert into @Employee 
		select a.CompanyCode
		     , BranchCode = (
					select top 1
						   x.BranchCode
					  from HrEmployeeMutation x
				     where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID 	
					   and ( x.IsDeleted = convert(bit, 0) or x.IsDeleted is null)
					 order by x.MutationDate desc
		       )
			 , a.EmployeeID
			 , a.EmployeeName
			 , Department = (
					select top 1
					       x.Department
					  from HrEmployeeAchievement x
					 where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID
					   and x.IsDeleted = convert(bit, 0)
					 order by x.AssignDate desc
			   )
			 , Position = (
					select top 1
					       x.Position
					  from HrEmployeeAchievement x
					 where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID
					   and x.IsDeleted = convert(bit, 0)
					 order by x.AssignDate desc
			   )
			 , PositionName = (
					select top 1
					       x.Position
					  from HrEmployeeAchievement x
					  left join gnMstPosition y
						on y.CompanyCode = x.CompanyCode
					   and y.DeptCode = x.Department
					   and y.PosCode = x.Position
					 where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID
					   and ( x.IsDeleted = convert(bit, 0) or x.IsDeleted is null )
					 order by x.AssignDate desc
			   )
			 , Grade = (
					select top 1
					       x.Grade
					  from HrEmployeeAchievement x
					 where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID
					   and ( x.IsDeleted = convert(bit, 0) or x.IsDeleted is null )
					 order by x.AssignDate desc
			   )
			 , a.TeamLeader
		  from HrEmployee a
		 where a.CompanyCode = @CompanyCode
		   and @BranchCode = (
					select top 1
						   x.BranchCode
					  from HrEmployeeMutation x
				     where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID 	
					   and ( x.IsDeleted = convert(bit, 0)  or IsDeleted is null )	  
					 order by x.MutationDate desc
		       )
		   and a.EmployeeID = @CurrentEmployeeID
		   and (
					select top 1
					       x.Department
					  from HrEmployeeAchievement x
					 where x.CompanyCode = a.CompanyCode
					   and x.EmployeeID = a.EmployeeID
					   and x.IsDeleted = convert(bit, 0)
					 order by x.AssignDate desc
			   ) = 'SALES';

		
		--select * from HrEmployee where EmployeeID=@EmployeeID;
		
		--select * from HrEmployeeAchievement where EmployeeID=@EmployeeID;
		--select * from HrEmployeeMutation where EmployeeID=@EmployeeID;


		set @RecordCount = (select count(a.EmployeeID) from @Employee a);
		set @CurrentEmployeeName = ( select top 1 a.EmployeeName from @Employee a );
		set @CurrentDeparment = ( select top 1 a.Department from @Employee a );
		set @CurrentPosition = ( select top 1 a.Position from @Employee a );
		set @CurrentPositionName = ( select top 1 a.PositionName from @Employee a );
		set @CurrentLeader = ( select top 1 a.Leader from @Employee a );
		set @Status = convert(bit, 0);
		set @PositionHeader = (
				select top 1
				       a.PosHeader
				  from gnMstPosition a
				 where a.CompanyCode = @CompanyCode
				   and a.DeptCode = @CurrentDeparment
				   and a.PosCode = @CurrentPosition
		    );
		set @PositionHeaderName = (
				select top 1
				       a.PosName
				  from gnMstPosition a
				 where a.CompanyCode = @CompanyCode
				   and a.DeptCode = @CurrentDeparment
				   and a.PosCode = @PositionHeader
		    );
		if @Iterator = 1
			set @Grade = ( select top 1 a.Grade from @Employee a );

		if @CurrentPosition = 'SC'
			set @SalesCoordinator = @CurrentEmployeeID;

		if @CurrentPosition = 'SH'
			set @SalesHead = @CurrentEmployeeID;

		if @CurrentPosition = 'BM'
		begin
			set @BranchManager = @CurrentEmployeeID;
			set @OperationManager = @CurrentEmployeeID;
		end

		if @CurrentPosition = @GeneralManager
			set @GeneralManager = @CurrentEmployeeID;



		if @RecordCount = 0
		begin
			if @Iterator = 1
				set @Message = 'Salesman dengan NIK "' + @CurrentEmployeeID + '" tidak ditemukan atau data mutasi dan achievement belum dilengkapi.';
			else
				set @Message = 'Leader dengan NIK "' + @CurrentEmployeeID + '" tidak ditemukan atau data mutasi dan achievement belum dilengkapi.';

			set @LoopStatus = convert(bit, 0);
		end
		else if isnull(@CurrentPosition, '') = '' or ltrim(rtrim(@CurrentPosition)) = ''
		begin
			if @Iterator = 1
				set @Message = 'Salesman dengan NIK "' + @CurrentEmployeeID + '" tidak memiliki data achievement.';
			else
				set @Message = 'Leader dengan NIK "' + @CurrentEmployeeID + '" tidak memiliki data achievement.';

			set @LoopStatus = convert(bit, 0);
		end
		else if isnull(@PositionHeader, '') = '' or ltrim(rtrim(@PositionHeader)) = '' or @PositionHeader = 'GM'
		begin
			set @Status = convert(bit, 1);
			set @Message = 'Struktur organisasi ITS sudah valid.';
			set @LoopStatus = convert(bit, 0);	
		end
		else if isnull(@CurrentLeader, '') = '' or ltrim(rtrim(@CurrentLeader)) = ''
		begin
			if @Iterator = 1
				set @Message = 'Salesman dengan NIK "' + @CurrentEmployeeID + '" tidak memiliki leader.';
			else
				set @Message = 'Leader dengan NIK "' + @CurrentEmployeeID + '" tidak memiliki leader yang sesuai dengan struktur organisasi ITS.';


			set @CurrentEmployeeID = '';
			set @LoopStatus = convert(bit, 0);
		end

		if(@Status = convert(bit, 1)) 
		begin
			set @Message = @Message;	
			set @Status = @Status;
		end
		else
		begin
			if isnull(@CurrentLeader, '') != '' or ltrim(rtrim(@CurrentLeader)) != ''
				set @CurrentEmployeeID = ( select top 1 a.Leader from @Employee a );
			else
				set @CurrentEmployeeID = '';
		end
		
		set @Iterator = @Iterator + 1;
	end

	select @Status as Status, @Message as Message, @SalesCoordinator as SalesCoordinator, @SalesHead as SalesHead, @BranchManager as BranchManager, @GeneralManager as GeneralManager, @Grade as Grade
end


go
exec uspfn_CheckITSOrganization '6115204', '611520401', 'SLS01- 0038'
