codeunit 50100 MyCodeunit
{
    local procedure MyProcedure()
    begin
        if [|Confirm('Confirm something')|] then;
    end;
}