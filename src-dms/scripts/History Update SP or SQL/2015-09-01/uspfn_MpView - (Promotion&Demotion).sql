IF(OBJECT_ID('uspfn_MpViewPromotion') is not null)
	drop proc dbo.uspfn_MpViewPromotion
GO
/****** Object:  StoredProcedure [dbo].[uspfn_MpViewPromotion]    Script Date: 9/1/2015 3:15:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_MpViewPromotion '', '2012-07-01', '2015-08-01'
create proc [dbo].[uspfn_MpViewPromotion]
@BranchCode varchar(25),
@PeriodFrom datetime,
@PeriodTo datetime
as
begin

	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end

	begin try
		drop table #temp1;
		drop table #temp2;
	end try
	begin catch
	end catch

	select Outlet, COUNT(EmployeeID) as demosi, Position into #temp1 from
	(
		select distinct a.CompanyCode, a.EmployeeID, Position, AssignDate, Grade,
			(
				select top 1 BranchCode from HrEmployeeMutation
				where 1 = 1 and ISNULL(IsDeleted, 0) = 0
				  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
				  and MutationDate <= AssignDate
				order by MutationDate desc
			) Outlet
		from HrEmployeeAchievement a
		where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position is not null
		  and (select top 1 PosLevel from gnMstPosition where PosCode = a.Position and CompanyCode = '0000000' and deptcode = 'SALES') >
				(select top 1 PosLevel from gnMstPosition where PosCode = 
					(
						select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
						  and Department = 'SALES' and Position <> 'SC'
						  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
						  and AssignDate < a.AssignDate
						  --and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
					)
					and CompanyCode = '0000000' and deptcode = 'SALES'
				)
		  and a.AssignDate between @PeriodFrom and @PeriodTo
		--and a.EmployeeID = '10124'
	) x
	where Position in ('BM', 'SH')
	group by CompanyCode, Outlet, Position


	select Outlet, COUNT(EmployeeID) as demosi, Grade into #temp2 from
	(
		select distinct a.CompanyCode, a.EmployeeID, Position, Grade, AssignDate, 
			(
				select top 1 BranchCode from HrEmployeeMutation
				where 1 = 1 --ISNULL(IsDeleted, 0) = 0
				  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
				  and MutationDate <= AssignDate
				order by MutationDate desc
			) Outlet
		from HrEmployeeAchievement a
		where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position = 'S'
		  and a.Grade >
				case when
				(
					select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							and Department = 'SALES' and Position <> 'SC'
							and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							and AssignDate < a.AssignDate
							--and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
				) = 'S'
				then
				(
					select top 1 Grade from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
						and Department = 'SALES' and Position <> 'SC'
						and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
						and AssignDate < a.AssignDate
						--and AssignDate between @PeriodFrom and @PeriodTo
					order by AssignDate desc
				)
				else a.Grade end
		  and a.AssignDate between @PeriodFrom and @PeriodTo
		--and a.EmployeeID = '10124'
	) x
	where Grade in (4, 3, 2)
	group by CompanyCode, Outlet, Grade

	;with x as (
	select g.OutletCode, OutletAbbreviation as Outlet, 
		isnull((
			select top 1 demosi from #temp1
			where Outlet = g.OutletCode
			  and Position = 'BM'
		), 0) BM,
		isnull((
			select top 1 demosi from #temp1
			where Outlet = g.OutletCode
			  and Position = 'SH'
		), 0)  SH,
		isnull((
			select top 1 demosi from #temp2
			where Outlet = g.OutletCode
			  and Grade = 4
		), 0)  Platinum,
		isnull((
			select top 1 demosi from #temp2
			where Outlet = g.OutletCode
			  and Grade = 3
		), 0)  Gold,
		isnull((
			select top 1 demosi from #temp2
			where Outlet = g.OutletCode
			  and Grade = 2
		), 0)  Silver
	from gnmstdealeroutletmapping g
	--  left join #temp1 t1
	--on g.OutletCode = t1.Outlet
	--  left join #temp2 t2
	--on g.OutletCode = t2.Outlet
	where isactive = 1
	  and OutletAbbreviation <> ''
	) select * from x
	where OutletCode like @BranchCode
	order by Outlet

	drop table #temp1
	drop table #temp2
end
GO

IF(OBJECT_ID('uspfn_MpViewPromotionDetail') is not null)
	drop proc dbo.uspfn_MpViewPromotionDetail
GO
/**************************************************************************************************************/
/**************************************************************************************************************/
/**************************************************************************************************************/
/****** Object:  StoredProcedure [dbo].[uspfn_MpViewPromotionDetail]    Script Date: 9/1/2015 3:15:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_MpViewPromotionDetail '6002401', '600240100', '2012-07-01', '2015-08-01', '2'
create proc [dbo].[uspfn_MpViewPromotionDetail]
@CompanyCode varchar(25),
@BranchCode varchar(25),
@PeriodFrom datetime,
@PeriodTo datetime,
@PromosiType varchar(3)
as
begin

	if @CompanyCode is null or @CompanyCode = ''
	begin
		set @CompanyCode = '%';
	end

	if (@PromosiType in ('BM', 'SH'))
	begin
		;with x as (
			select distinct a.CompanyCode, a.EmployeeID, Position, AssignDate, Grade,
				(
					select top 1 BranchCode from HrEmployeeMutation
					where 1 = 1 and ISNULL(IsDeleted, 0) = 0
					  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
					  and MutationDate <= AssignDate
					order by MutationDate desc
				) Outlet
			from HrEmployeeAchievement a
			where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position is not null
			  and (select top 1 PosLevel from gnMstPosition where PosCode = a.Position and CompanyCode = '0000000' and deptcode = 'SALES') >
					(select top 1 PosLevel from gnMstPosition where PosCode = 
						(
							select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							  and Department = 'SALES' and Position <> 'SC'
							  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							  and AssignDate < a.AssignDate
							  --and AssignDate between @PeriodFrom and @PeriodTo
							order by AssignDate desc
						)
						and CompanyCode = '0000000' and deptcode = 'SALES'
					)
			  and a.CompanyCode like @CompanyCode
			  and a.AssignDate between @PeriodFrom and @PeriodTo
			--and a.EmployeeID = '10124'
		)
		select (select top 1 OutletAbbreviation from gnMstDealerOutletMapping where OutletCode = Outlet) OutletName
		, (select top 1 EmployeeName from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)	Name
		, (select top 1 PosName from gnMstPosition where PosCode = Position and CompanyCode = '0000000' and deptcode = 'SALES')						Position
		, isnull((select top 1 LookUpValueName from gnMstLookUpDtl where CodeID = 'ITSG' and CompanyCode = '0000000' and LookUpValue = Grade), '')	Grade
		, (select top 1 JoinDate from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)		JoinDate
		from x
		where Position = @PromosiType and Outlet = @BranchCode
	end
	else if (@PromosiType in ('4', '3', '2'))
	begin
		;with x as (
			select distinct a.CompanyCode, a.EmployeeID, Position, Grade, AssignDate, 
				(
					select top 1 BranchCode from HrEmployeeMutation
					where 1 = 1 --ISNULL(IsDeleted, 0) = 0
					  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
					  and MutationDate <= AssignDate
					order by MutationDate desc
				) Outlet
			from HrEmployeeAchievement a
			where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position = 'S'
			  and a.Grade >
					case when
					(
						select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
								and Department = 'SALES' and Position <> 'SC'
								and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
								and AssignDate < a.AssignDate
								--and AssignDate between @PeriodFrom and @PeriodTo
							order by AssignDate desc
					) = 'S'
					then
					(
						select top 1 Grade from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							and Department = 'SALES' and Position <> 'SC'
							and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							and AssignDate < a.AssignDate
							--and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
					)
					else a.Grade end
			  and a.CompanyCode like @CompanyCode
			  and a.AssignDate between @PeriodFrom and @PeriodTo
		)
		select (select top 1 OutletAbbreviation from gnMstDealerOutletMapping where OutletCode = Outlet) OutletName
		, (select top 1 EmployeeName from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)	Name
		, (select top 1 PosName from gnMstPosition where PosCode = Position and CompanyCode = '0000000' and deptcode = 'SALES')						Position
		, isnull((select top 1 LookUpValueName from gnMstLookUpDtl where CodeID = 'ITSG' and CompanyCode = '0000000' and LookUpValue = Grade), '')	Grade
		, (select top 1 JoinDate from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)		JoinDate
		from x
		where Grade = @PromosiType and Outlet = @BranchCode
	end
end
GO

IF(OBJECT_ID('uspfn_MpViewDemotion') is not null)
	drop proc dbo.uspfn_MpViewDemotion
GO
/**************************************************************************************************************/
/**************************************************************************************************************/
/**************************************************************************************************************/
/****** Object:  StoredProcedure [dbo].[uspfn_MpViewDemotion]    Script Date: 9/1/2015 3:40:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_MpViewDemotion '', '2012-07-01', '2015-08-01'
create proc [dbo].[uspfn_MpViewDemotion]
@BranchCode varchar(25),
@PeriodFrom datetime,
@PeriodTo datetime
as
begin

	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end

	begin try
		drop table #temp1;
		drop table #temp2;
	end try
	begin catch
	end catch

	select Outlet, COUNT(EmployeeID) as demosi, Position into #temp1 from
	(
		select distinct a.CompanyCode, a.EmployeeID, Position, AssignDate, Grade,
			(
				select top 1 BranchCode from HrEmployeeMutation
				where 1 = 1 and ISNULL(IsDeleted, 0) = 0
				  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
				  and MutationDate <= AssignDate
				order by MutationDate desc
			) Outlet
		from HrEmployeeAchievement a
		where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position is not null
		  and (select top 1 PosLevel from gnMstPosition where PosCode = a.Position and CompanyCode = '0000000' and deptcode = 'SALES') <
				(select top 1 PosLevel from gnMstPosition where PosCode = 
					(
						select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
						  and Department = 'SALES' and Position <> 'SC'
						  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
						  and AssignDate < a.AssignDate
						  --and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
					)
					and CompanyCode = '0000000' and deptcode = 'SALES'
				)
		  and a.AssignDate between @PeriodFrom and @PeriodTo
		--and a.EmployeeID = '10124'
	) x
	where Position in ('SH', 'S')
	group by CompanyCode, Outlet, Position


	select Outlet, COUNT(EmployeeID) as demosi, Grade into #temp2 from
	(
		select distinct a.CompanyCode, a.EmployeeID, Position, Grade, AssignDate, 
			(
				select top 1 BranchCode from HrEmployeeMutation
				where 1 = 1 --ISNULL(IsDeleted, 0) = 0
				  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
				  and MutationDate <= AssignDate
				order by MutationDate desc
			) Outlet
		from HrEmployeeAchievement a
		where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position = 'S'
		  and a.Grade <
				case when
				(
					select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							and Department = 'SALES' and Position <> 'SC'
							and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							and AssignDate < a.AssignDate
							--and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
				) = 'S'
				then
				(
					select top 1 Grade from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
						and Department = 'SALES' and Position <> 'SC'
						and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
						and AssignDate < a.AssignDate
						--and AssignDate between @PeriodFrom and @PeriodTo
					order by AssignDate desc
				)
				else 
				(
					case when
					(
						select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
								and Department = 'SALES' and Position <> 'SC'
								and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
								and AssignDate < a.AssignDate
								--and AssignDate between @PeriodFrom and @PeriodTo
							order by AssignDate desc
					) = 'SH'
					then
						a.Grade + '1'
					else a.Grade end
				) end
		  and a.AssignDate between @PeriodFrom and @PeriodTo
		--and a.EmployeeID = '10124'
	) x
	where Grade in (3, 2)
	group by CompanyCode, Outlet, Grade

	;with x as (
	select g.OutletCode, OutletAbbreviation as Outlet, 
		isnull((
			select top 1 demosi from #temp1
			where Outlet = g.OutletCode
			  and Position = 'SH'
		), 0) SH,
		isnull((
			select top 1 demosi from #temp1
			where Outlet = g.OutletCode
			  and Position = 'S'
		), 0) SC,
		isnull((
			select top 1 demosi from #temp2
			where Outlet = g.OutletCode
			  and Grade = 3
		), 0)  Gold,
		isnull((
			select top 1 demosi from #temp2
			where Outlet = g.OutletCode
			  and Grade = 2
		), 0)  Silver
	from gnmstdealeroutletmapping g
	--  left join #temp1 t1
	--on g.OutletCode = t1.Outlet
	--  left join #temp2 t2
	--on g.OutletCode = t2.Outlet
	where isactive = 1
	  and OutletAbbreviation <> ''
	) select * from x
	where OutletCode like @BranchCode
	order by Outlet

	drop table #temp1
	drop table #temp2
end
GO

IF(OBJECT_ID('uspfn_MpViewDemotionDetail') is not null)
	drop proc dbo.uspfn_MpViewDemotionDetail
GO
/**************************************************************************************************************/
/**************************************************************************************************************/
/**************************************************************************************************************/
/****** Object:  StoredProcedure [dbo].[uspfn_MpViewDemotionDetail]    Script Date: 9/1/2015 3:40:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_MpViewDemotionDetail '6002401', '600240100', '2012-07-01', '2015-08-01', '2'
create proc [dbo].[uspfn_MpViewDemotionDetail]
@CompanyCode varchar(25),
@BranchCode varchar(25),
@PeriodFrom datetime,
@PeriodTo datetime,
@DemosiType varchar(3)
as
begin

	if @CompanyCode is null or @CompanyCode = ''
	begin
		set @CompanyCode = '%';
	end

	if (@DemosiType in ('SH', 'SC'))
	begin
		;with x as (
			select distinct a.CompanyCode, a.EmployeeID, Position, AssignDate, Grade,
				(
					select top 1 BranchCode from HrEmployeeMutation
					where 1 = 1 and ISNULL(IsDeleted, 0) = 0
					  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
					  and MutationDate <= AssignDate
					order by MutationDate desc
				) Outlet
			from HrEmployeeAchievement a
			where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position is not null
			  and (select top 1 PosLevel from gnMstPosition where PosCode = a.Position and CompanyCode = '0000000' and deptcode = 'SALES') <
					(select top 1 PosLevel from gnMstPosition where PosCode = 
						(
							select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							  and Department = 'SALES' and Position <> 'SC'
							  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							  and AssignDate < a.AssignDate
							  --and AssignDate between @PeriodFrom and @PeriodTo
							order by AssignDate desc
						)
						and CompanyCode = '0000000' and deptcode = 'SALES'
					)
			  and a.CompanyCode like @CompanyCode
			  and a.AssignDate between @PeriodFrom and @PeriodTo
			--and a.EmployeeID = '10124'
		)
		select (select top 1 OutletAbbreviation from gnMstDealerOutletMapping where OutletCode = Outlet) OutletName
		, (select top 1 EmployeeName from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)	Name
		, (select top 1 PosName from gnMstPosition where PosCode = Position and CompanyCode = '0000000' and deptcode = 'SALES')						Position
		, isnull((select top 1 LookUpValueName from gnMstLookUpDtl where CodeID = 'ITSG' and CompanyCode = '0000000' and LookUpValue = Grade), '')	Grade
		, (select top 1 JoinDate from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)		JoinDate
		from x
		where Position = @DemosiType and Outlet = @BranchCode
	end
	else if (@DemosiType in ('3', '2'))
	begin
		;with x as (
			select distinct a.CompanyCode, a.EmployeeID, Position, Grade, AssignDate, 
				(
					select top 1 BranchCode from HrEmployeeMutation
					where 1 = 1 --ISNULL(IsDeleted, 0) = 0
					  and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
					  and MutationDate <= AssignDate
					order by MutationDate desc
				) Outlet
			from HrEmployeeAchievement a
			where isnull(a.IsDeleted, 0) = 0 and Department = 'SALES' and a.Position = 'S'
			  and a.Grade <
					case when
					(
						select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
								and Department = 'SALES' and Position <> 'SC'
								and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
								and AssignDate < a.AssignDate
								--and AssignDate between @PeriodFrom and @PeriodTo
							order by AssignDate desc
					) = 'S'
					then
					(
						select top 1 Grade from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
							and Department = 'SALES' and Position <> 'SC'
							and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
							and AssignDate < a.AssignDate
							--and AssignDate between @PeriodFrom and @PeriodTo
						order by AssignDate desc
					)
					else 
					(
						case when
						(
							select top 1 Position from HrEmployeeAchievement where isnull(IsDeleted, 0) = 0
									and Department = 'SALES' and Position <> 'SC'
									and CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID
									and AssignDate < a.AssignDate
									--and AssignDate between @PeriodFrom and @PeriodTo
								order by AssignDate desc
						) = 'SH'
						then
							a.Grade + '1'
						else a.Grade end
					) end
			  and a.CompanyCode like @CompanyCode
			  and a.AssignDate between @PeriodFrom and @PeriodTo
		)
		select (select top 1 OutletAbbreviation from gnMstDealerOutletMapping where OutletCode = Outlet) OutletName
		, (select top 1 EmployeeName from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)	Name
		, (select top 1 PosName from gnMstPosition where PosCode = Position and CompanyCode = '0000000' and deptcode = 'SALES')						Position
		, isnull((select top 1 LookUpValueName from gnMstLookUpDtl where CodeID = 'ITSG' and CompanyCode = '0000000' and LookUpValue = Grade), '')	Grade
		, (select top 1 JoinDate from HrEmployee where isnull(isdeleted, 0) = 0 and CompanyCode = x.CompanyCode and EmployeeId = x.EmployeeId)		JoinDate
		from x
		where Grade = @DemosiType and Outlet = @BranchCode
	end
end
GO
