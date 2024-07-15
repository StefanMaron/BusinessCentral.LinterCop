[|codeunit 50100 MyCodeunit|]
{
    procedure MyProcedure()
    begin
    end;
}

permissionset 50100 MyPermSet
{
    Permissions = Codeunit MyCodeunit = X;
}