table 50100 MyTable
{
    LookupPageId = MyPage;
    DrillDownPageId = MyPage;

    fields
    {
        field(1; MyField; Integer)
        {

        }
    }

    keys
    {
        key(Key1; MyField) { }
    }
}

page 50100 MyPage
{
    PageType = List;
    SourceTable = MyTable;

    layout
    {
        area(Content)
        {
            field(MyField; MyField)
            {

            }
        }
    }
}

codeunit 50100 MyCodeunit
{
    local procedure MyProcedure()
    var
        MyTable: Record MyTable;
    begin
        [|Page.Run(Page::MyPage, MyTable);|]
    end;
}