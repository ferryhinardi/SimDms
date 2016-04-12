IF EXISTS (SELECT * FROM sys.objects WHERE type IN ('FN', 'IF', 'TF') and name = 'typeofgiftToName')
	DROP FUNCTION [dbo].typeofgiftToName
GO
CREATE FUNCTION [dbo].typeofgiftToName ( @stringToSplit VARCHAR(MAX), @delimiter varchar(5) = ',' )
RETURNS  [nvarchar] (500)
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT, @XYZ int
 DECLARE @retValue VARCHAR(50)

 SELECT @retValue = '', @XYZ = 0

 WHILE CHARINDEX(@delimiter, @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(@delimiter, @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1), @XYZ = @XYZ + 1

  IF @name ='true'
  SELECT @retValue = @retValue + 
	CASE WHEN LEN(@retValue) > 0 THEN ',' ELSE '' END + 
	CASE @XYZ WHEN 1 THEN 'Gift'		
			  WHEN 2 THEN 'Letter' 
			  WHEN 3 THEN 'SMS' 
			  WHEN 4 THEN 'Souvenir' 
			  WHEN 5 THEN 'Telepon' 
	ELSE '' END
  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END
	
 IF @stringToSplit ='true'
  SELECT 
	@XYZ = @XYZ + 1, 
	@retValue = @retValue + 
	CASE WHEN LEN(@retValue) > 0 THEN ',' ELSE '' END + 
	CASE @XYZ WHEN 1 THEN 'Gift'		
			  WHEN 2 THEN 'Letter' 
			  WHEN 3 THEN 'SMS' 
			  WHEN 4 THEN 'Souvenir' 
			  WHEN 5 THEN 'Telepon' 
	ELSE '' END

 RETURN @retValue
END
GO