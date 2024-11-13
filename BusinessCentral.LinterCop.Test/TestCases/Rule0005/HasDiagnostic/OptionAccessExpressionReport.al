codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    begin
        Report.Run([|report|]::MyReport);
    end;
}

report 50100 MyReport { }