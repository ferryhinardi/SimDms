insert into sysMenuDms values ('csReview', 'Review', 'cs', 6, 1, NULL, NULL)
insert into sysMenuDms values ('csrvInputReview', 'Input Review', 'csReview', 1, 2, 'review/inputreview', NULL)
insert into sysMenuDms values ('csrvReviews', 'Reviews', 'csReview', 2, 2, 'review/reviews', NULL)

insert into sysRoleMenu (RoleId, MenuId) values ('ADMIN', 'csReview')
insert into sysRoleMenu (RoleId, MenuId) values ('ADMIN', 'csrvInputReview')
insert into sysRoleMenu (RoleId, MenuId) values ('ADMIN', 'csrvReviews')

insert into sysRoleMenu (RoleId, MenuId) values ('CS-ADM', 'csReview')
insert into sysRoleMenu (RoleId, MenuId) values ('CS-ADM', 'csrvInputReview')
insert into sysRoleMenu (RoleId, MenuId) values ('CS-ADM', 'csrvReviews')

insert into SysRole (RoleId, RoleName, [Description]) values ('CS-GM', 'CS GM', 'CS GM Role')

insert into sysRoleMenu (RoleId, MenuId) values ('CS-GM', 'csrvInputReview')
insert into sysRoleMenu (RoleId, MenuId) values ('CS-GM', 'csrvReviews')

