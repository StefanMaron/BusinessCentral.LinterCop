codeunit 50100 MyCodeunit
{
    [Obsolete]
    procedure MyProcedure()
    begin
        [|Commit();|]
    end;
}