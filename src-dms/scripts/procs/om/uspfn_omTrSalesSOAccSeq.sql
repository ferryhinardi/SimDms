go
if object_id('uspfn_omTrSalesSOAccSeqList') is not null
	drop procedure uspfn_omTrSalesSOAccSeqList

go
create procedure uspfn_omTrSalesSOAccSeqList
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	declare @CodeID varchar(25);
	set @CodeID = 'TPGO';

	SELECT a.PartNo
         , (
				SELECT b.PartName 
				  FROM spMstItemInfo b 
				 WHERE b.CompanyCode = a.CompanyCode 
				   AND b.PartNo = a.PartNo 
		   )  AS PartName
         , a.DemandQty
         , a.RetailPrice
         , a.DemandQty * a.RetailPrice as Total
         , dtl.LookUpValueName as JenisPart
         , a.Qty
		 , a.AfterDiscDPP
		 , a.AfterDiscPPn
		 , a.AfterDiscTotal
		 , a.RetailPrice
		 , a.DemandQty
		 , QtyUnit = a.DemandQty / a.Qty
		 , a.PartSeq
		 , a.SONo as SONumber
      FROM omTrSalesSOAccsSeq a 
      LEFT JOIN GnMstLookUpDtl dtl 
	    ON dtl.companyCode = a.companyCode 
       AND dtl.LookUpValue = a.TypeOfGoods 
       AND dtl.CodeId = @CodeId
     WHERE a.CompanyCode = @CompanyCode
       AND a.BranchCode = @BranchCode
       AND a.SONo = @SONumber;

end




go
exec uspfn_omTrSalesSOAccSeqList '6115204', '611520401', 'SOB/11/000001'


