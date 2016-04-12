select * from SysMenu where MenuHeader = 'cs'



-- Insert root menu 
insert into SysMenu (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl)
values ('cs', 'Customer Satisfaction', '', 1, 0, '')

insert into SysMenu (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl)
values ('csInquiry', 'Inquiry', 'cs', 1, 1, '')


insert into SysMenu (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl)
values ('csInq3daycall', 'Rekap 3 Days Call', 'csInquiry', 1, 2, 'inq/Rekap3DaysCall')
     , ('csInqStnkExt', 'STNK Extension', 'csInquiry', 2, 2, 'inq/StnlExtension')
     , ('csInqBpkbRem', 'BPKB Reminder', 'csInquiry', 3, 2, 'inq/BpkbReminder')
     , ('csInqBday', 'Customer Birthday', 'csInquiry', 4, 2, 'inq/CustomerBirthday')
     , ('csInqfeedback', 'Customer Feedback', 'csInquiry', 5, 2, 'inq/CustomerFeedback')


