table 50100 MyTable
{
    fields
    {
        field(1; Name; Text[100]) { }
    }

    procedure DoSth(MyTable2: Record MyTable)
    begin
        DoSth([|Rec|]);
    end;
}