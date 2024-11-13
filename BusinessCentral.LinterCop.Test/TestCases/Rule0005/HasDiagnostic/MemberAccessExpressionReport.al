codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|report|]::MyReport;
    end;
}

report 50100 MyReport { }