[|table 50100 MyTable|]
{
    fields
    {
        field(1; MyField; Integer) {}
    }
}

page 50100 MyPage
{
    PageType = List;
    SourceTable = MyTable;
    SourceTableTemporary = true;
}