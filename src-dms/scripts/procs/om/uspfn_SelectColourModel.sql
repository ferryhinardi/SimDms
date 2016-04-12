if object_id('uspfn_SelectColourModel') is not null
	drop procedure uspfn_SelectColourModel


go
create procedure uspfn_SelectColourModel
	@CompanyCode varchar(17),
	@BranchCode varchar(17),
	@SalesModelCode varchar(17),
	@InquiryNumber varchar(17)
as

begin
	declare @ReffType varchar(4);
	set @ReffType = 'COLO';

	DECLARE @Colour AS VARCHAR(15)
	SET     @Colour = (SELECT ColourCode FROM pmKDP WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND InquiryNumber = @InquiryNumber)

	SELECT a.ColourCode
	     , (
				SELECT b.RefferenceDesc1
				  FROM omMstRefference b
				 WHERE b.RefferenceCode = a.ColourCode
				   AND b.CompanyCode = a.CompanyCode 
				   AND b.RefferenceType = @ReffType
		    ) AS colourDesc
		 , a.Remark
	  FROM omMstModelColour a
	 WHERE a.CompanyCode = @CompanyCode 
	   AND a.SalesModelCode = @SalesModelCode 
	   AND (	
				CASE 
					WHEN @InquiryNumber = '' THEN '' 
					ELSE a.ColourCode 
				END
			) = (CASE WHEN @InquiryNumber = '' THEN '' ELSE @Colour END)
end





go
