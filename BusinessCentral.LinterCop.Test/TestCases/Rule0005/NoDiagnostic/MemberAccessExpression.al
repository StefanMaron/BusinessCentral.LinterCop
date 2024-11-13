codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|Codeunit|]::MyCodeunit;
        i := [|Database|]::MyTable;
        i := [|Enum|]::MyEnum::MyValue;
        i := [|Page|]::MyPage;
        i := [|Query|]::MyQuery;
        i := [|Report|]::MyReport;
        i := [|Xmlport|]::MyXmlport;
    end;
}

codeunit 50101 MyCodeunit { }
enum 50100 MyEnum { value(0; MyValue) { } }
page 50100 MyPage { }
query 50100 MyQuery { elements { dataitem(MyTable; MyTable) { column(Myfield; Myfield) { } } } }
report 50100 MyReport { }
table 50100 MyTable { fields { field(1; Myfield; Code[10]) { } } }
xmlport 50100 MyXmlport { }