exec sp_executesql N'exec uspfn_SelectITSNo @CompanyCode, @BranchCode',N'@CompanyCode nvarchar(7),@BranchCode nvarchar(7)',@CompanyCode=N'6115204',@BranchCode=N'611520402'

select * from gnMstCoProfile

exec uspfn_SelectITSNo '6115204', ''
