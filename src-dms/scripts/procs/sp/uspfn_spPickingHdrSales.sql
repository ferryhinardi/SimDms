
/****** Object:  StoredProcedure [dbo].[uspfn_spPickingHdrSales]    Script Date: 6/19/2014 10:46:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[uspfn_spPickingHdrSales] @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(25), @CodeID varchar(6)
as    
SELECT DISTINCT
                            spTrnSORDHdr.DocNo,      
                            spTrnSORDHdr.DocDate,                                                  
                            PaymentName = 
                            (select LookUpValueName from gnMstLookupDtl 
                            where LookupValue = spTrnSORDHdr.PaymentCode and CodeID = @CodeID),
                            spTrnSORDHdr.OrderNo,                            
                            spTrnSORDHdr.OrderDate,
                            CONVERT(bit, 1) ChkSelect                          
                            FROM spTrnSPickingHdr 
                            LEFT JOIN spTrnSPickingDtl ON spTrnSPickingHdr.PickingSlipNo = spTrnSPickingDtl.PickingSlipNo AND
                                spTrnSPickingHdr.CompanyCode = spTrnSPickingDtl.CompanyCode AND
                                spTrnSPickingHdr.BranchCode = spTrnSPickingDtl.BranchCode    
                            INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSPickingDtl.DocNo AND
                                spTrnSORDHdr.CompanyCode = spTrnSPickingDtl.CompanyCode AND
                                spTrnSORDHdr.BranchCode = spTrnSPickingDtl.BranchCode
                            WHERE spTrnSPickingHdr.PickingSlipNo = @PickingSlipNo AND
                            spTrnSPickingHdr.CompanyCode = @CompanyCode AND
                            spTrnSPickingHdr.BranchCode = @BranchCode