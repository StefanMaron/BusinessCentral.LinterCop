table 50100 "My Table"
{
    fields
    {
        field(1; "My Field"; Integer) { }
    }

    var
        MyTable: Record "My Table";

    procedure MyProcedure()
    begin
        Rec.TestField([|"MY FIELD"|]);
        MyTable.SetRange([|"MY FIELD"|]);
    end;
}