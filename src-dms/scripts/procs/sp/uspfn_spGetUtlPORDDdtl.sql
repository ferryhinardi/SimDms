
Create procedure [dbo].[uspfn_spGetUtlPORDDdtl]   
(  
--DECLARE
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @CustomerCode varchar(15),
 @OrderNo varchar(20) 

--set @CompanyCode   = 6006400001
--set @BranchCode  = 6006400000
--set @CustomerCode ='0000028'
--set @OrderNo ='SOC/14/100143'

)  
AS  

begin

declare @md bit
declare @dbMD varchar(25)
declare @CompanyMD varchar(25)

set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 1)
BEGIN

SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    e.RetailPrice,
	                (e.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
					gnMstCompanyMapping AS d with(nolock, nowait),
	                spMstItemPrice AS e with(nolock, nowait)
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
					d.CompanyMD = e.CompanyCode AND
	                a.PartNo = e.PartNo AND
	                a.CompanyCode = @CompanyCode AND
	                a.BranchCode = @BranchCode AND
	                b.DealerCode = @CustomerCode AND
	                a.OrderNo = @OrderNo
                ORDER BY a.PartNo
END
ELSE
BEGIN

set @dbMD = (select DISTINCT dbmd from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

--set @CompanyMD = (select DISTINCT CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
--select @CompanyMD
declare @query varchar(max)

if(isnull(cast(@dbMD as varchar(max)),'') = '')
BEGIN
SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    d.RetailPrice,
	                (d.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
	                spMstItemPrice AS d with(nolock, nowait) 
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
	                a.PartNo = d.PartNo AND
	                a.CompanyCode = @CompanyCode AND
	                a.BranchCode = @BranchCode AND
	                b.DealerCode = @CustomerCode AND
	                a.OrderNo = @OrderNo
                ORDER BY a.PartNo
END
ELSE
BEGIN

set @query = '
SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    e.RetailPrice,
	                (e.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
					gnMstCompanyMapping AS d with(nolock, nowait),
	                '+@dbMD+'..spMstItemPrice AS e with(nolock, nowait)
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
					d.CompanyMD = e.CompanyCode AND
	                a.PartNo = e.PartNo AND
	                a.CompanyCode = '+@CompanyCode+' AND
	                a.BranchCode = '+@BranchCode +'AND
	                b.DealerCode = '''+@CustomerCode +'''AND
	                a.OrderNo = '''+@OrderNo+'''
                ORDER BY a.PartNo'
EXEC(@query)
print(@query)
				
		END	 
	END
end	