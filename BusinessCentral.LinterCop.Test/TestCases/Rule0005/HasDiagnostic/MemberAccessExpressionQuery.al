codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|query|]::MyQuery;
    end;
}

table 50100 MyTable { fields { field(1; Myfield; Code[10]) { } } }
query 50100 MyQuery { elements { dataitem(MyTable; MyTable) { column(Myfield; Myfield) { } } } }