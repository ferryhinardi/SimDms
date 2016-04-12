alter procedure uspfn_SysDealerExtracted
	@TableName  varchar(50) = '',
	@DealerCode varchar(20) = ''
as

declare @DateParam as date
set @DateParam = getdate()


if (isnull(@DealerCode, '') != '')
begin
	;with x as (
	select a.TableName
		 , a.ProcessedDate
		 , a.FileName
		 , a.FileSize
	  from SysDealerHist a with (nolock,nowait)
	  left join DealerInfo b
		on b.DealerCode = a.DealerCode
	 where a.Status = 'PROCESSED'
	   and convert(date, a.ProcessedDate) = @DateParam
	   and a.DealerCode = @DealerCode
	   and a.FileSize > 0
	),
	y as (
	select TableName
		 , LastExtractedDate = (select top 1 ProcessedDate from x as sub where sub.TableName = x.TableName order by ProcessedDate)
	     , count(*) as DataCount
	  from x
	 group by TableName
	)
	select TableName as name
	     , TableName as text
		 , 'Last Extracted : ' 
		 + left(convert(varchar, LastExtractedDate, 21), 19) as subtext
		 , DataCount as infotext
	  from y
	 order by DataCount desc
end
else
begin
	;with x as (
	select a.DealerCode
		 , b.ShortName as DealerName
		 , a.ProcessedDate
		 , a.FileName
		 , a.FileSize
	  from SysDealerHist a with (nolock,nowait)
	  left join DealerInfo b
		on b.DealerCode = a.DealerCode
	 where a.Status = 'PROCESSED'
	   and convert(date, a.ProcessedDate) = @DateParam
	   and a.TableName = (case when @TableName = '' then a.TableName else @TableName end)
	   and a.FileSize > 0
	),
	y as (
	select DealerCode
		 , DealerName
		 , count(*) as DataCount
		 , LastExtractedDate = (select top 1 ProcessedDate from x as sub where sub.DealerCode = x.DealerCode order by ProcessedDate)
	  from x
	 group by DealerCode, DealerName
	),
	z as (
	select DealerCode 
		 , DealerName
		 , DataCount
		 , LastExtractedDate
	  from y
	)
	select DealerCode as name
		 , DealerName as text
		 , 'CompanyCode : ' + DealerCode + '<br/>Last Extracted : ' 
		 + left(convert(varchar, LastExtractedDate, 21), 19) as subtext
		 , DataCount as infotext
	  from z
	 where 1 = 1
	   --and DelayDate > 0
	 order by infotext desc
end

go 

uspfn_SysDealerExtracted 'HrEmployee', '6006400001'
