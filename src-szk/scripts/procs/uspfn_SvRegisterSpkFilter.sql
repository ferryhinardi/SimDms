alter procedure uspfn_SvRegisterSpkFilter 

as 

select Area value, Area as text
  from gnMstDealerMapping 
 group by Area
 order by Area

select a.DealerCode as value
     , a.DealerCode + ' - ' + a.DealerName as text
     , b.Area as [group]
  from DealerInfo a
  left join gnMstDealerMapping b 
    on b.DealerCode = a.DealerCode
 where 1 = 1
   and a.DealerName not like 'BUANA INDOMOBIL TRADA%'
   and substring(a.DealerCode, 5, 1) = '4'
   and b.Area is not null
   and a.DealerCode not in ('6015402')
 order by a.DealerCode

select BranchCode as value
     , BranchCode + ' - ' + ShortBranchName as text
     , CompanyCode as [group]
  from OutletInfo
 order by BranchCode

return
-- for performance reasone, not use real transaction

declare @dealers as table (DealerCode varchar(20))
declare @outlets as table (OutletCode varchar(20))

insert into @dealers select CompanyCode from SvRegisterSpk group by CompanyCode
insert into @outlets select BranchCode from SvRegisterSpk group by BranchCode

select a.Area value, a.Area as text
  from gnMstDealerMapping a, @dealers b
 where b.DealerCode = a.DealerCode
 group by a.GroupNo, a.Area
 order by GroupNo

select a.DealerCode as value, a.DealerCode + ' - ' + b.DealerName as text, c.Area as "group"
  from @dealers a
  left join DealerInfo b 
    on b.DealerCode = a.DealerCode
  left join gnMstDealerMapping c 
    on c.DealerCode = a.DealerCode
 order by a.DealerCode

select a.OutletCode as value, a.OutletCode + ' - ' + b.ShortBranchName as text, b.CompanyCode as "group"
  from @outlets a
  left join OutletInfo b 
    on b.BranchCode = a.OutletCode
 order by b.BranchCode

go

exec uspfn_SvRegisterSpkFilter 

