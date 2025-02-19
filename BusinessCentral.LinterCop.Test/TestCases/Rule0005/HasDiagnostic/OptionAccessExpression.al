codeunit 50100 OptionAccessExpression
{
    procedure MyProcedure()
    var
        i: Integer;
    begin
        i := [|CODEUNIT|]::MyCodeunit;
        i := [|codeunit|]::MyCodeunit;

        i := [|DATABASE|]::MyTable;
        i := [|database|]::MyTable;

        i := [|ENUM|]::MyEnum::MyValue;
        i := [|enum|]::MyEnum::MyValue;

        i := [|PAGE|]::MyPage;
        i := [|page|]::MyPage;

        i := [|QUERY|]::MyQuery;
        i := [|query|]::MyQuery;

        i := [|REPORT|]::MyReport;
        i := [|report|]::MyReport;

        i := [|XMLPORT|]::MyXmlport;
        i := [|XmlPort|]::MyXmlport;
    end;
}

codeunit 50101 MyCodeunit { }
enum 50100 MyEnum { value(0; MyValue) { } }
page 50100 MyPage { }
query 50100 MyQuery { elements { dataitem(MyTable; MyTable) { column(Myfield; Myfield) { } } } }
report 50100 MyReport { }
table 50100 MyTable { fields { field(1; Myfield; Code[10]) { } } }
xmlport 50100 MyXmlport { }