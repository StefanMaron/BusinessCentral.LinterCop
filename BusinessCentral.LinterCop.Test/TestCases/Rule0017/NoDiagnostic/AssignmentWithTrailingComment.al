table 50100 MyTable
{
    fields
    {
        field(1; MyField; Integer) { }
        field(2; MyField2; Boolean)
        {
            FieldClass = FlowField;
        }
    }

    procedure MyProcedure();
    begin
        [|Rec.MyField2|] := false; // Comment
    end;
}