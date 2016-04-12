USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_ModelAccountBrowse]    Script Date: 11/12/2014 2:26:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_ModelAccountBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as

 SELECT a.CompanyCode, a.BranchCode, a.SalesModelCode, z.SalesModelDesc
		, SalesAccNo, DiscountAccNo, ReturnAccNo, COGsAccNo,HReturnAccNo
		, b.Description as SalesAccDesc, c.Description as DiscountAccDesc, d.Description as ReturnAccDesc, e.Description as COGsAccDesc, f.Description as HReturnAccDesc
		, SalesAccNoAks, ReturnAccNoAks, DiscountAccNoAks
		, g.Description as SalesAccDescAks, h.Description as ReturnAccDescAks, i.Description as DiscountAccDescAks
		, ShipAccNo, DepositAccNo, OthersAccNo, BBNAccNo, KIRAccNo
		,j.Description as ShipAccDesc, k.Description as DepositAccDesc, l.Description as OthersAccDesc, m.Description as BBNAccDesc, n.Description as KIRAccDesc
		, PReturnAccNo, InTransitTransferStockAccNo
		, o.Description as PReturnAccDesc, p.Description as IntransitAccDesc
		, a.Remark, IsActive, a.InventoryAccNo, q.Description as InventoryAccDesc
-- select *
FROM omMstModelAccount a
LEFT JOIN GnMstAccount b
	ON a.SalesAccNo = b.AccountNo
LEFT JOIN GnMstAccount c
	ON a.DiscountAccNo = c.AccountNo
LEFT JOIN GnMstAccount d
	ON a.ReturnAccNo = d.AccountNo
LEFT JOIN GnMstAccount e
	ON a.COGsAccNo = e.AccountNo
LEFT JOIN GnMstAccount f
	ON a.HReturnAccNo = f.AccountNo
LEFT JOIN GnMstAccount g
	ON a.SalesAccNoAks = g.AccountNo
LEFT JOIN GnMstAccount h
	ON a.ReturnAccNoAks = h.AccountNo
LEFT JOIN GnMstAccount i
	ON a.DiscountAccNoAks = i.AccountNo
LEFT JOIN GnMstAccount j
	ON a.ShipAccNo = j.AccountNo
LEFT JOIN GnMstAccount k
	ON a.DepositAccNo = k.AccountNo
LEFT JOIN GnMstAccount l
	ON a.OthersAccNo = l.AccountNo
LEFT JOIN GnMstAccount m
	ON a.BBNAccNo = m.AccountNo
LEFT JOIN GnMstAccount n
	ON a.KIRAccNo = n.AccountNo
LEFT JOIN GnMstAccount o
	ON a.PReturnAccNo = o.AccountNo AND o.BranchCode = @BranchCode
LEFT JOIN GnMstAccount p
	ON a.InTransitTransferStockAccNo = p.AccountNo AND p.BranchCode = @BranchCode
LEFT JOIN GnMstAccount q
	ON a.InventoryAccNo = q.AccountNo
INNER JOIN omMstModel z
	ON a.SalesModelCode = z.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 