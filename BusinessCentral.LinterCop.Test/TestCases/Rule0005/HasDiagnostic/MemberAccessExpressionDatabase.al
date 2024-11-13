codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|database|]::MyTable;
    end;
}

table 50100 MyTable { fields { field(1; Myfield; Code[10]) { } } }