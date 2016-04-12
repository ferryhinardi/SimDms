create view [dbo].[LookUpSO]
as
               SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, '1900/01/01') 
                then '' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, '1900/01/01') 
                then 'undefined' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, '1900/01/01') 
                then 'undefined' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, '1900/01/01') 
                then 'undefined' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + ' ' + e.Address2 + ' ' + e.Address3 + ' ' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = 'GRPR' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT b.lookupvaluename
                        FROM gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID ='MPWH')  
						AS WareHouseName,
                (a.CustomerCode
                    + ' || '
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + ' || '
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + ' || '
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = 'GRPR' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then 'OPEN'
                                when 1 then 'PRINTED'
                                when 2 then 'APPROVED'
                                when 3 then 'DELETED'
                                when 4 then 'REJECTED'
                                when 9 then 'FINISHED' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
GO