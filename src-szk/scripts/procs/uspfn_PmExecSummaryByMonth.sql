alter procedure uspfn_PmExecSummaryByMonth

as

select FieldRow = ('R' + convert(varchar, a.GroupSeq))
     , FieldCol = GroupType
	 , FieldVal = DataCount
  from PmDashboardData a
 where a.DashboardName = 'PmExecutiveSummary3'

 union all
select FieldRow = ('R' + convert(varchar, a.GroupSeq))
     , FieldCol = 'C4'
	 , FieldVal = sum(DataCount)
  from PmDashboardData a
 where a.DashboardName = 'PmExecutiveSummary3'
 group by a.GroupSeq

 union all
select FieldRow = 'R6'
     , FieldCol = GroupType
	 , FieldVal = sum(DataCount)
  from PmDashboardData a
 where a.DashboardName = 'PmExecutiveSummary3'
 group by GroupType

 union all
select FieldRow = 'R6'
     , FieldCol = 'C4'
	 , FieldVal = sum(DataCount)
  from PmDashboardData a
 where a.DashboardName = 'PmExecutiveSummary3'


go


exec  uspfn_PmExecSummaryByMonth
