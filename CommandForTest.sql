select * from Unique_Ids 

delete from Unique_Ids WHERE (Scope = 'variable');

delete from Unique_Ids WHERE (Scope = 'alarm');

delete from Unique_Ids WHERE (Scope = 'event');

insert into Unique_Ids values('variable','GemPPChangeStatus','908',GETDATE(),'datavariable')

select * from Unique_Ids where ID = '908'

select * from Aliases

delete from Aliases WHERE (Scope = 'variable');

delete from Aliases WHERE (Scope = 'alarm');

select * from Unique_Ids where Name = 'SendData'