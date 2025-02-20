codeunit 50100 "My Codeunit"
{
    [|trigger|] [|OnRun|]()
    [|begin|]
    [|end|];

    [|var|]
        MyTable: [|Record|] "My Table" [|temporary|];
        MyEnum: [|Enum|] "My Enum";
        MyQuery: [|Query|] "My Query";
        MyReport: [|Report|] "My Report";
        MyXmlport: [|XmlPort|] "My Xmlport";
        MyLabel: [|Label|] 'My Label',  [|Comment|] = 'My Comment',  [|Locked|] = true,  [|MaxLength|] = 100;
        i: [|Integer|];

    [|procedure|] MyProcedure()
    [|begin|]
        i := [|Codeunit|]::"My Codeunit";
        i := [|Database|]::"My Table";
        i := [|Enum|]::"My Enum"::"My Value";
        i := [|Page|]::"My Page";
        i := [|Query|]::"My Query";
        i := [|Report|]::"My Report";
        i := [|Xmlport|]::"My Xmlport";
    [|end|];
}

enum 50100 "My Enum" { value(0; "My Value") { } }
page 50100 "My Page" { }
query 50100 "My Query" { elements { dataitem(MyTable; "My Table") { column(Myfield; "My field") { } } } }
report 50100 "My Report" { }
table 50100 "My Table" { fields { field(1; "My field"; Code[10]) { } } }
xmlport 50100 "My Xmlport" { }