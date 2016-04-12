alter procedure uspfn_SysDataInfoStatusSimDms
as

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join HrEmployee b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'HrEmployee' and LastUpdate > '1910-01-01'
   and substring(c.CompanyCode, 5, 1) = '4'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join CsTDayCall b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'CsTDayCall' and LastUpdate > '1910-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join CsCustBirthDay b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'CsCustBirthDay' and LastUpdate > '1910-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join CsStnkExt b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'CsStnkExt' and LastUpdate > '1910-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join CsCustBpkb b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'CsCustBpkb' and LastUpdate > '1910-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc
