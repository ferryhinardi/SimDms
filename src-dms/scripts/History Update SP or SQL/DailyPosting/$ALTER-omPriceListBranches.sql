-- select * from omPriceListBranches

ALTER TABLE omPriceListBranches 
	ADD OthersDPP numeric(18,0) NULL, OthersPPn numeric(18,0);
GO

UPDATE omPriceListBranches set OthersDPP=0, OthersPPn=0
GO

ALTER procedure [dbo].[uspfn_omGetPriceList]
	@CompanyCode varchar(20)='',
	@BranchCode varchar(20)='',
	@salesmodelcode varchar(32)='',
	@salesmodelyear int=0,
	@Status int=2,
	@SupplierCode varchar(20)=''
AS
SELECT 
       a.[SalesModelCode]
      ,a.[SalesModelYear]
      ,b.[SalesModelDesc]
      ,a.[GroupPrice]
      ,a.[RetailPriceIncludePPN]
      ,a.[DiscPriceIncludePPN]
      ,a.[NetSalesIncludePPN]
      ,a.[RetailPriceExcludePPN]
      ,a.[DiscPriceExcludePPN]
      ,a.[NetSalesExcludePPN]
      ,a.[PPNBeforeDisc]
      ,a.[PPNAfterDisc]
      ,a.OthersDPP
      ,a.OthersPPn [OthersPPN]
      ,a.[PPNBMPaid]
      ,a.[EffectiveDate]
      ,a.[isStatus]
  FROM [dbo].[omPriceListBranches] a inner join omMstModel b 
  on (a.CompanyCode=b.CompanyCode and a.SalesModelCode=b.SalesModelCode)
where a.companycode=@CompanyCode and a.BranchCode=@BranchCode
and a.SalesModelCode like case when @salesmodelcode='' then '%%' else @salesmodelcode end
and a.SalesModelYear = case when @salesmodelyear=0 then a.SalesModelYear else @salesmodelyear end
and a.isStatus = case when @Status=2 then a.isstatus else @Status end
and a.SupplierCode = @SupplierCode
GO

ALTER Procedure [dbo].[uspfn_omUpdatePriceList]
  @CompanyCode varchar(15)='' ,
  @BranchCode varchar(15)='' ,
  @SupplierCode varchar(15)='' ,
  @GroupPrice varchar(15)='' ,
  @SalesModelCode varchar(20) ='',
  @SalesModelYear numeric(4, 0)=2015 ,
  @EffectiveDate datetime=null ,
  @RetailPriceIncludePPN numeric(18, 0)=0 ,
  @DiscPriceIncludePPN numeric(18, 0) =0,
  @NetSalesIncludePPN numeric(18, 0)=0 ,
  @RetailPriceExcludePPN numeric(18, 0)=0 ,
  @DiscPriceExcludePPN numeric(18, 0) =0,
  @NetSalesExcludePPN numeric(18, 0) =0,
  @PPNBeforeDisc numeric(18, 0) =0,
  @PPNAfterDisc numeric(18, 0) =0,
  @PPNBMPaid numeric(18, 0)=0 ,
  @isStatus bit = 1,
  @UserId varchar(20)='',
  @OthersDPP numeric(18, 0)=0 ,
  @OthersPPN numeric(18, 0)=0 
AS
BEGIN
  
  IF @BranchCode=''
  BEGIN
    declare @loopbranch varchar(20)
    declare crBranch  cursor local for
    select branchcode from gnMstCoProfile
    where companycode=@companycode

    open crBranch
    fetch next from crBranch into @loopbranch

    while @@FETCH_STATUS=0
    begin
      exec uspfn_omUpdatePriceList @companycode,@loopbranch,@SupplierCode,@GroupPrice,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId, @OthersDPP, @OthersPPN
      fetch next from crBranch into @loopbranch
    end

    close crbranch
    deallocate crbranch
  END
  ELSE
  BEGIN

    INSERT INTO [dbo].[omPriceListBranchesLog]
        ([CompanyCode],[BranchCode],[SupplierCode],[GroupPrice],[SalesModelCode],[SalesModelYear]
        ,[EffectiveDate],[RetailPriceIncludePPN],[DiscPriceIncludePPN],[NetSalesIncludePPN]
        ,[RetailPriceExcludePPN],[DiscPriceExcludePPN],[NetSalesExcludePPN],[PPNBeforeDisc],[PPNAfterDisc]
        ,[PPNBMPaid],[isStatus],[LastUpdateBy],[LastUpdateDate])
      VALUES
        (@companycode,@BranchCode,@SupplierCode,@GroupPrice
        ,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN
        ,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId,getdate()) 

    IF  EXISTS(select * from omPriceListBranches  
    where companycode=@companycode and branchcode=@branchcode
    and suppliercode=@suppliercode and groupprice=@groupprice
    and SalesModelCode=@SalesModelCode and SalesModelYear=@SalesModelYear
    and EffectiveDate = @EffectiveDate)
    BEGIN
      UPDATE [omPriceListBranches] 
      SET 
        RetailPriceIncludePPN=@RetailPriceIncludePPN,
        DiscPriceIncludePPN=@DiscPriceIncludePPN,NetSalesIncludePPN=@NetSalesIncludePPN,
        RetailPriceExcludePPN=@RetailPriceExcludePPN,DiscPriceExcludePPN=@DiscPriceExcludePPN,
        NetSalesExcludePPN=@NetSalesExcludePPN,PPNBeforeDisc=@PPNBeforeDisc,
        PPNAfterDisc=@PPNAfterDisc,PPNBMPaid=@PPNBMPaid,isStatus=@isStatus,
        [LastUpdateBy]=@userid,LastUpdateDate=getdate(),
        OthersDPP=@OthersDPP,
        OthersPPN=@OthersPPN
      where companycode=@companycode and branchcode=@branchcode
      and suppliercode=@suppliercode and groupprice=@groupprice
      and SalesModelCode=@SalesModelCode and SalesModelYear=@SalesModelYear
      and EffectiveDate = @EffectiveDate
    END
    ELSE
    BEGIN
      INSERT INTO [dbo].[omPriceListBranches]
           ([CompanyCode],[BranchCode],[SupplierCode],[GroupPrice],[SalesModelCode],[SalesModelYear]
           ,[EffectiveDate],[RetailPriceIncludePPN],[DiscPriceIncludePPN],[NetSalesIncludePPN]
           ,[RetailPriceExcludePPN],[DiscPriceExcludePPN],[NetSalesExcludePPN],[PPNBeforeDisc],[PPNAfterDisc]
           ,[PPNBMPaid],[isStatus],CreatedBy,[CreatedDate],OthersDPP, OthersPPn)
       VALUES
           (@companycode,@BranchCode,@SupplierCode,@GroupPrice
           ,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN
           ,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId,getdate(), @OthersDPP,@OthersPPN) 

    END


  END

END
