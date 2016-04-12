create proc [dbo].[uspfn_spGetSubstitusi] --'01104-05088-000'
(
	@partno varchar(20)
)
as
begin
	select top 1 NewPartNo, PartName from BIT201310.dbo.spMstItemMod a
	join spMstItemInfo b on a.NewPartNo = b.PartNo
	where a.PartNo = @partno
end