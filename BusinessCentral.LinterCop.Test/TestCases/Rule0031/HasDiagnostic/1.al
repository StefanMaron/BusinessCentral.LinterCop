table 50100 MyTable
{
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

    local procedure MyProcedure()
    begin
        [|Rec.LockTable();|]
    end;
}