alter procedure uspfn_InqGroupModelList

as


select GroupModel as value, GroupModel as text
  from PmDashboardByDay 
 where GroupModel != 'MOVINGAVG'
 group by GroupModel

go 

exec uspfn_InqGroupModelList
