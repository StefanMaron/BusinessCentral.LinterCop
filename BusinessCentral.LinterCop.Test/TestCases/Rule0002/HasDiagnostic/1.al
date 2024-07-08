codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    begin
        [|Commit();|]
    end;
}