table 50100 MyTable
{
    fields
    {
        field(1; Id; Integer) { }
    }

    internal procedure ClearRec()
    begin
        Clear([|Rec|]);
    end;
}