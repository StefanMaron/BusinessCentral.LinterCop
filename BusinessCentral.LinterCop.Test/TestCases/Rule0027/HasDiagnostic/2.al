codeunit 50100 MyCodeunit
{
    procedure MyProcedure()
    var
        SalesHeader: Record "Sales Header";
    begin
        [|Page.Run(Page::MyPage, SalesHeader)|];
    end;
}

page 50100 MyPage { }
table 36 "Sales Header"
{
    fields { field(1; MyField; Integer) { } }
}
