[|table 50100 MyTable|]
{
    fields
    {
        field(1; MyField; Integer) {}
    }

    procedure DoSthTable() 
    begin
    end;
}

page 50100 MyPage
{
    PageType = List;
    SourceTable = MyTable;

    local procedure DoSth() begin
        [|Rec|].DoSthTable();
    end;
}