codeunit 50100 "My Codeunit"
{
    [|TRIGGER|] [|ONRUN|]()
    [|BEGIN|]
    [|END|];

    [|VAR|]
        MyTable: [|RECORD|] "My Table" [|TEMPORARY|];
        MyEnum: [|ENUM|] "My Enum";
        MyQuery: [|QUERY|] "My Query";
        MyReport: [|REPORT|] "My Report";
        MyXmlport: [|Xmlport|] "My Xmlport"; // The right casing here should be XmlPort
        MyLabel: [|LABEL|] 'My Label', [|COMMENT|] = 'My Comment', [|LOCKED|] = [|TRUE|], [|MAXLENGTH|] = 100;
        i: [|INTEGER|];

    [|PROCEDURE|] MyProcedure()
    [|BEGIN|]
        i := [|CODEUNIT|]::"My Codeunit";
        i := [|codeunit|]::"My Codeunit";

        i := [|DATABASE|]::"My Table";
        i := [|database|]::"My Table";

        i := [|ENUM|]::"My Enum"::"My Value";;
        i := [|enum|]::"My Enum"::"My Value";;

        i := [|PAGE|]::"My Page";
        i := [|page|]::"My Page";

        i := [|QUERY|]::"My Query";
        i := [|query|]::"My Query";

        i := [|REPORT|]::"My Report";
        i := [|report|]::"My Report";

        i := [|XMLPORT|]::"My Xmlport";
        i := [|XmlPort|]::"My Xmlport";
    [|END|];
}

enum 50100 "My Enum" { value(0; "My Value") { } }
page 50100 "My Page" { }
query 50100 "My Query" { elements { dataitem(MyTable; "My Table") { column(Myfield; "My field") { } } } }
report 50100 "My Report" { }
table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
xmlport 50100 "My Xmlport" { }