table 50100 [|MyTable|]
{
    TableType = Temporary;

    fields
    {
        field(1; MyField; Integer) { }
    }
}

page 50100 MyPage
{
    SourceTable = MyTable;

    procedure MyProcedure()
    begin
        [|Rec|].MyField := 0;
    end;
}