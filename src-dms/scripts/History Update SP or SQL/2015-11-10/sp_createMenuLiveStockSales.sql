INSERT INTO SysMenuDms (
	MenuId,
	MenuCaption,
	MenuHeader,
	MenuIndex,
	MenuLevel,
	MenuUrl
)
VALUES (
	'omInquiryLiveStock',
	'Live Stock',
	'omInquiry',
	11, 
	2,
	'inquiry/LiveStock'
)
INSERT INTO SysRoleMenu VALUES('ADMIN', 'omInquiryLiveStock')
INSERT INTO SysRoleMenu VALUES('Sales', 'omInquiryLiveStock')