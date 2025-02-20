pageextension 50000 MyPageExtension extends MyPage
{
    trigger OnOpenPage()
    var
        MyTable: Record MyTable;
    begin
        [|MyTable.FindFirst();|]
        [|MyTable.Insert();|]
        [|MyTable.Modify();|]
        [|MyTable.Delete();|]
    end;
}

page 50000 MyPage
{
    SourceTable = MyTable;
}

table 50000 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
    }
}