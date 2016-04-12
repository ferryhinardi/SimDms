CREATE VIEW [dbo].[VW_TREEMENUS]
AS
select moduleid MenuId, ModuleCaption MenuCaption,NULL MenuHeader,ModuleIndex MenuIndex,0 MenuLevel,'' MenuUrl from sysmodule
UNION
select MenuId, MenuCaption, MenuHeader,MenuIndex, MenuLevel, MenuUrl from sysmenudms
GO