codeunit 50000 MyCodeunit
{
    trigger OnRun()
    var
        Integer: Record Integer;
    begin
        [|Integer.FindFirst();|]
    end;
}

