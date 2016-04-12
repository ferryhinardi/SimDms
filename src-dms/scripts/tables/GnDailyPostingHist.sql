create table GnDailyPostingHist(
	CompanyCode varchar(20) NOT NULL,
	PostingDate datetime NOT NULL,
	PostingSeq int NOT NULL,
	PostingLog varchar(max) NULL,
	CreatedBy varchar(20) NULL,
	CreatedDate datetime NULL,
 primary key clustered 
(
	CompanyCode asc,
	PostingDate asc,
	PostingSeq asc
)) 

GO



 