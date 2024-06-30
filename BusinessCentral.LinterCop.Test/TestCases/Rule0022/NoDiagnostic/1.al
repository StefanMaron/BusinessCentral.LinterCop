codeunit 50100 MyCodeunit
{
    ObsoleteState = Pending;

    local procedure MyProcedure()
    begin
        [|GlobalLanguage(1033);|]
    end;
}