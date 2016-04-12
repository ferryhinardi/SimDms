CREATE VIEW omTrSalesDraftSOLookupView
AS
SELECT a.CompanyCode, a.BranchCode, a.DraftSONo, 
    CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, '1900/01/01') 
    then '' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
    e.Address1 + ' ' + e.Address2 + ' ' + e.Address3 + ' ' + e.Address4 as Address,
    a.RefferenceNo, a.SKPKNo, a.DraftSODate, (a.CustomerCode
        + ' || '
        + (SELECT b.CustomerName
            FROM gnMstCustomer b
            WHERE a.CompanyCode = b.CompanyCode
		    AND a.CustomerCode = b.CustomerCode))  
		    AS Customer, (a.Salesman
        + ' || '
        + (SELECT c.EmployeeName
            FROM gnMstEmployee c
            WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
	        AND a.Salesman = c.EmployeeID))  AS Salesman, (a.GroupPriceCode
        + ' || '
        + (SELECT d.RefferenceDesc1
            FROM omMstRefference d
            WHERE a.CompanyCode = d.CompanyCode
		    AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPriceCode, 
    CASE a.Status when 0 then 'OPEN'
                    when 1 then 'PRINTED'
                    when 2 then 'APPROVED'
                    when 3 then 'DELETED'
                    when 4 then 'REJECTED'
                    when 9 then 'FINISHED' END as Stat
    , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales
FROM omTrSalesDraftSO a
    INNER JOIN gnMstCustomer e
        ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
--WHERE a.CompanyCode = '6006410' AND a.BranchCode = '600641001'