
insert into SysRoleMenu
select 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E'
     , MenuId 
  from SysMenu 
 where MenuId like '%cs%'
