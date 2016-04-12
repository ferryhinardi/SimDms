ALTER FUNCTION [dbo].[CheckDivided] (
	@Number1 DECIMAL(18,2),
	@Number2 DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
BEGIN
	IF (@Number2 = 0)
		RETURN 0;

	RETURN (@Number1 / @Number2);
END