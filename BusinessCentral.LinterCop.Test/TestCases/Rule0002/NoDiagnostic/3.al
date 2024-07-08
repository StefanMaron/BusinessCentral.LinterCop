codeunit 50100 MyCodeunit
{
    ObsoleteState = Pending;

    procedure MyProcedure()
    begin
        [|Commit();|]
    end;
}