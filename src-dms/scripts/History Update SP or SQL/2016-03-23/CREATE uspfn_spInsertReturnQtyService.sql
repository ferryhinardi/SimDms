create procedure [dbo].[uspfn_spInsertReturnQtyService]
(
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(10),
	@ReturnNo varchar(15),
	@LastUpdateBy varchar(30)
)
as
begin
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, d.ProductType
		, ISNULL(d.ServiceNo, 0) ServiceNo
		, a.PartNo
		, (SELECT TOP 1 ISNULL(PartSeq, 0) FROM svTrnSrvItem WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND ProductType = d.ProductType AND ServiceNo = d.ServiceNo AND 
			PartNo = a.PartNo AND SupplySlipNo = c.DocNo ORDER BY PartSeq DESC) PartSeq
		, ISNULL(a.QtyReturn, 0) QtyReturn
		, c.DocNo
		, b.ReturnNo
		, b.ReturnDate
	INTO
		#SrvItem
	FROM 
		spTrnSRturSSDtl a WITH(NOLOCK, NOWAIT)
		INNER JOIN spTrnSRturSSHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.ReturnNo = b.ReturnNo
		INNER JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.DocNo = c.DocNo
		INNER JOIN svTrnService d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.BranchCode = d.BranchCode
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.UsageDocNo = d.JobOrderNo
		AND a.ReturnNo = @ReturnNo

	DECLARE @ServiceNo	VARCHAR(MAX)
	DECLARE @PartNo		VARCHAR(MAX)
	DECLARE @DocNo  	VARCHAR(MAX)
	DECLARE @QtyReturn	NUMERIC(18,2)
	DECLARE @ReturnDate	DATETIME
	DECLARE @PartSeq	VARCHAR(MAX) 

	DECLARE db_cursor CURSOR FOR
	SELECT ServiceNo, PartNo, QtyReturn, ReturnDate, DocNo FROM #SrvItem

	OPEN db_cursor
	FETCH NEXT FROM db_cursor INTO @ServiceNo, @PartNo, @QtyReturn, @ReturnDate, @DocNo
	WHILE @@FETCH_STATUS = 0
	BEGIN

		DECLARE db_cursor_2 CURSOR FOR
		SELECT PartSeq 
		FROM SvTrnSrvItem 
		WHERE 1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND ProductType = @ProductType
			AND	ServiceNo = @ServiceNo 
			AND SupplySlipNo = @DocNo
			AND PartNo = @PartNo ORDER BY partseq DESC
		OPEN db_cursor_2
		FETCH NEXT FROM db_cursor_2 INTO @PartSeq
		WHILE @@FETCH_STATUS = 0
		BEGIN
	
			SELECT 
				CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, 
				CASE WHEN @QtyReturn >= SupplyQty 
					THEN @QtyReturn - (@QtyReturn - SupplyQty)
					ELSE CASE WHEN @QtyReturn + ReturnQty > SupplyQty THEN 0 ELSE @QtyReturn END
				END QtyAva
				, ReturnQty
			INTO #SrvItemDtl FROM SvTrnSrvItem 
			WHERE 1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND ProductType = @ProductType 
				AND ServiceNo = @ServiceNo 
				AND PartNo = @PartNo 
				AND PartSeq = @PartSeq
		
			SET @QtyReturn = CASE WHEN ISNULL(
				(
					SELECT  CASE WHEN ReturnQty  + QtyAva <= SupplyQty 
						THEN @QtyReturn
						ELSE @QtyReturn - SupplyQty	END
					FROM #SrvItemDtl	
				),0) < 0 
				THEN 0 
				ELSE ISNULL((SELECT ReturnQty + @QtyReturn - SupplyQty FROM #SrvItemDtl),0) 
			END
		
			UPDATE SvTrnSrvItem
			SET	SSReturnNo = @ReturnNo
				, SSReturnDate = @ReturnDate
				, ReturnQty = a.ReturnQty + b.QtyAva
				, LastUpdateBy = @LastUpdateBy 
				, LastUpdateDate = GetDate()
			FROM SvTrnSrvItem a, #SrvItemDtl b
			WHERE 1 = 1
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
				AND a.ProductType = b.ProductType
				AND a.ServiceNo = b.ServiceNo
				AND a.PartNo = b.PartNo
				AND a.PartSeq = b.PartSeq
				AND b.QtyAva > 0
				AND a.ReturnQty  + b.QtyAva <= a.SupplyQty

			DROP TABLE #SrvItemDtl

		FETCH NEXT FROM db_cursor_2 INTO @PartSeq
		END
		CLOSE db_cursor_2
		DEALLOCATE db_cursor_2

	FETCH NEXT FROM db_cursor INTO @ServiceNo, @PartNo, @QtyReturn, @ReturnDate, @DocNo
	END
	CLOSE db_cursor
	DEALLOCATE db_cursor 
	DROP TABLE #SrvItem
end	