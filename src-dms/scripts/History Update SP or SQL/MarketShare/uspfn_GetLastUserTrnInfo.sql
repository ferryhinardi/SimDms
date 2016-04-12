USE [SimDms]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_GetLastUserTrnInfo]    Script Date: 7/1/2015 9:52:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_GetLastUserTrnInfo]  
	@ModuleName varchar(30),  
	@TableName varchar(50),  
	@UserId varchar(20),  
	@TrnDate datetime  
AS  
BEGIN  
	declare @companyCode varchar(20), @BranchCode varchar(20), @LastLogin datetime  

	SELECT TOP 1 @companyCode=DealerCode , @BranchCode=OutletCode       
	FROM [dbo].sysuser where Username=@UserId  

	IF @BranchCode IS NULL Set @BranchCode = @companyCode

	IF @UserId IS NULL or @UserId=''  
		return 0  

	SELECT TOP 1 @LastLogin=[CreateDate] FROM [dbo].[SysSession]  
	where SessionUser=@UserId order by CreateDate desc  

	IF EXISTS( select * from sysLastTrnInfo   
	where [DealerCode]=@companyCode and [OutletCode]=@BranchCode   
	and [ModuleName] = @ModuleName and  [TableName]= @TableName )  
		update sysLastTrnInfo  
		set UserName=@UserId, Lastlogin=@LastLogin, LastUpdated=@TrnDate  
		where [DealerCode]=@companyCode and [OutletCode]=@BranchCode   
		and [ModuleName] = @ModuleName and  [TableName]= @TableName   
	ELSE  
		insert into sysLastTrnInfo ([DealerCode],[OutletCode],[ModuleName],[TableName],[UserName],LastLogin,LastUpdated)  
		values (@companyCode,@BranchCode,@ModuleName,@TableName,@UserId,@LastLogin, @TrnDate)
END  